﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Vanara.PInvoke;
using static Vanara.PInvoke.Gdi32;
using static Vanara.PInvoke.UxTheme;

namespace Vanara.Drawing
{
	public static class BufferedPaint
	{
		private static readonly Dictionary<IntPtr, Tuple<object, object>> paintAnimationInstances = new Dictionary<IntPtr, Tuple<object, object>>();

		static BufferedPaint() { BufferedPaintInit(); }

		public delegate uint GetDuration<in TState>(TState oldState, TState newState);

		public delegate void PaintAction<in TState, in TParam>(Graphics graphics, Rectangle bounds, TState currentState, TParam data);

		public static void Paint<TState, TParam>(Graphics graphics, Rectangle bounds, PaintAction<TState, TParam> paintAction,
			TState currentState, TParam data)
		{
			using (var g = new SafeDCHandle(graphics))
			using (var bp = new BufferedPaintHandle(g, bounds))
				paintAction(bp.Graphics, bounds, currentState, data);
		}

		public static void PaintAnimation<TState>(Graphics graphics, IWin32Window ctrl, Rectangle bounds, PaintAction<TState, int> paintAction,
			TState currentState, TState newState, GetDuration<TState> getDuration)
			=> PaintAnimation<TState, int>(graphics, ctrl, bounds, paintAction, currentState, newState, getDuration, 0);

		public static void PaintAnimation<TState, TParam>(Graphics graphics, IWin32Window ctrl, Rectangle bounds,
			PaintAction<TState, TParam> paintAction, TState currentState, TState newState, GetDuration<TState> getDuration, TParam data)
		{
			try
			{
				if (System.Environment.OSVersion.Version.Major >= 6)
				{
					// If this handle is running with a different state, stop the animations
					Tuple<object, object> val;
					if (paintAnimationInstances.TryGetValue(ctrl.Handle, out val))
					{
						if (!Equals(val.Item1, currentState) || !Equals(val.Item2, newState))
						{
							BufferedPaintStopAllAnimations(new HandleRef(ctrl, ctrl.Handle));
							System.Diagnostics.Debug.WriteLine("BufferedPaintStop.");
							paintAnimationInstances[ctrl.Handle] = new Tuple<object, object>(currentState, newState);
						}
					}
					else
						paintAnimationInstances.Add(ctrl.Handle, new Tuple<object, object>(currentState, newState));

					using (var hdc = new SafeDCHandle(graphics))
					{
						if (hdc.IsInvalid) return;
						// see if this paint was generated by a soft-fade animation
						if (BufferedPaintRenderAnimation(new HandleRef(ctrl, ctrl.Handle), hdc))
						{
							paintAnimationInstances.Remove(ctrl.Handle);
							return;
						}

						var animParams = new BP_ANIMATIONPARAMS(BP_ANIMATIONSTYLE.BPAS_LINEAR, getDuration?.Invoke(currentState, newState) ?? 0);
						using (var h = new BufferedPaintHandle(ctrl, hdc, bounds, animParams, BP_PAINTPARAMS.NoClip))
						{
							if (!h.IsInvalid)
							{
								if (h.SourceGraphics != null)
									paintAction(h.SourceGraphics, bounds, currentState, data);
								if (h.Graphics != null)
									paintAction(h.Graphics, bounds, newState, data);
							}
							else
							{
								// hdc.Dispose();
								paintAction(graphics, bounds, newState, data);
							}
						}
					}
				}
				else
					paintAction(graphics, bounds, newState, data);
			}
			catch { }
			System.Diagnostics.Debug.WriteLine($"BufferedPaint state items = {paintAnimationInstances.Count}.");
		}
	}

	public class BufferedPaintHandle : SafeHandle
	{
		private readonly bool ani;

		public BufferedPaintHandle(SafeDCHandle hdc, Rectangle targetRectangle, BP_PAINTPARAMS paintParams = null,
			BP_BUFFERFORMAT fmt = BP_BUFFERFORMAT.BPBF_TOPDOWNDIB) : base(IntPtr.Zero, true)
		{
			RECT target = targetRectangle;
			IntPtr phdc;
			var hbp = BeginBufferedPaint(hdc, ref target, fmt, paintParams, out phdc);
			if (hbp == IntPtr.Zero) throw new Win32Exception();
			if (phdc != IntPtr.Zero) Graphics = Graphics.FromHdc(phdc);
			SetHandle(hbp);
		}

		public BufferedPaintHandle(IWin32Window wnd, SafeDCHandle hdc, Rectangle targetRectangle,
			BP_ANIMATIONPARAMS? animationParams = null,
			BP_PAINTPARAMS paintParams = null, BP_BUFFERFORMAT fmt = BP_BUFFERFORMAT.BPBF_TOPDOWNDIB)
			: base(IntPtr.Zero, true)
		{
			RECT rc = targetRectangle;
			var ap = animationParams ?? BP_ANIMATIONPARAMS.Empty;
			IntPtr hdcFrom, hdcTo;
			var hbp = BeginBufferedAnimation(new HandleRef(wnd, wnd.Handle), hdc, ref rc, fmt, paintParams, ref ap, out hdcFrom,
				out hdcTo);
			if (hbp == IntPtr.Zero) throw new Win32Exception();
			if (hdcFrom != IntPtr.Zero) SourceGraphics = Graphics.FromHdc(hdcFrom);
			if (hdcTo != IntPtr.Zero) Graphics = Graphics.FromHdc(hdcTo);
			SetHandle(hbp);
			ani = true;
		}

		public Graphics Graphics { get; }

		public override bool IsInvalid => handle == IntPtr.Zero;

		public Graphics SourceGraphics { get; }

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				SourceGraphics?.Dispose();
				Graphics?.Dispose();
			}
			base.Dispose(disposing);
		}

		protected override bool ReleaseHandle()
		{
			try
			{
				if (ani)
					EndBufferedAnimation(handle, true);
				else
					EndBufferedPaint(handle, true);
				return true;
			}
			catch { }
			return false;
		}
	}
}