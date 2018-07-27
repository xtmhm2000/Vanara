﻿using System;
using System.Runtime.InteropServices;

namespace Vanara.PInvoke
{
	public static partial class Shell32
	{
		/// <summary>These flags are used with IExplorerBrowser::GetOptions and IExplorerBrowser::SetOptions.</summary>
		[Flags]
		[PInvokeData("Shobjidl.h", MSDNShortId = "4e2983bc-cad2-4bcc-8169-57b5274b2142")]
		public enum EXPLORER_BROWSER_OPTIONS
		{
			/// <summary>No options.</summary>
			EBO_NONE = 0x00000000,

			/// <summary>Do not navigate further than the initial navigation.</summary>
			EBO_NAVIGATEONCE = 0x00000001,

			/// <summary>
			/// Use the following standard panes: Commands Module pane, Navigation pane, Details pane, and Preview pane. An implementer of
			/// IExplorerPaneVisibility can modify the components of the Commands Module that are shown. For more information see,
			/// IExplorerPaneVisibility::GetPaneState. If EBO_SHOWFRAMES is not set, Explorer browser uses a single view object.
			/// </summary>
			EBO_SHOWFRAMES = 0X00000002,

			/// <summary>Always navigate, even if you are attempting to navigate to the current folder.</summary>
			EBO_ALWAYSNAVIGATE = 0x00000004,

			/// <summary>Do not update the travel log.</summary>
			EBO_NOTRAVELLOG = 0x00000008,

			/// <summary>
			/// Do not use a wrapper window. This flag is used with legacy clients that need the browser parented directly on themselves.
			/// </summary>
			EBO_NOWRAPPERWINDOW = 0x00000010,

			/// <summary>Show WebView for sharepoint sites.</summary>
			EBO_HTMLSHAREPOINTVIEW = 0x00000020,

			/// <summary>Introduced in Windows Vista. Do not draw a border around the browser window.</summary>
			EBO_NOBORDER = 0x00000040,

			/// <summary>Introduced in Windows Vista. Do not persist the view state.</summary>
			EBO_NOPERSISTVIEWSTATE = 0x00000080,
		}

		/// <summary>These flags are used with IExplorerBrowser::FillFromObject.</summary>
		[Flags]
		[PInvokeData("Shobjidl.h", MSDNShortId = "5be62600-147d-4625-8e6c-aa6687da2168")]
		public enum EXPLORER_BROWSER_FILL_FLAGS
		{
			/// <summary>No flags.</summary>
			EBF_NONE = 0x0000000,

			/// <summary>
			/// Causes IExplorerBrowser::FillFromObject to first populate the results folder with the contents of the parent folders of the
			/// items in the data object, and then select only the items that are in the data object.
			/// </summary>
			EBF_SELECTFROMDATAOBJECT = 0x0000100,

			/// <summary>
			/// Do not allow dropping on the folder. In other words, do not register a drop target for the view. Applications can then
			/// register their own drop targets.
			/// </summary>
			EBF_NODROPTARGET = 0x0000200,
		}

		/// <summary>
		/// IExplorerBrowser is a browser object that can be either navigated or that can host a view of a data object. As a full-featured
		/// browser object, it also supports an automatic travel log.
		/// <para>
		/// The Shell provides a default implementation of IExplorerBrowser as CLSID_ExplorerBrowser.Typically, a developer does not need to
		/// provide a custom implementation of this interface.
		/// </para>
		/// </summary>
		[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("dfd3b6b5-c10c-4be9-85f6-a66969f402f6"), CoClass(typeof(ExplorerBrowser))]
		[PInvokeData("Shobjidl.h", MSDNShortId = "da2cf5d4-5a68-4d18-807b-b9d4e2712c10")]
		public interface IExplorerBrowser
		{
			/// <summary>Prepares the browser to be navigated.</summary>
			/// <param name="hwndParent">A handle to the owner window or control.</param>
			/// <param name="prc">
			/// A pointer to a RECT that contains the coordinates of the bounding rectangle that the browser will occupy. The coordinates are
			/// relative to hwndParent.
			/// </param>
			/// <param name="pfs">A pointer to a FOLDERSETTINGS structure that determines how the folder will be displayed in the view.</param>
			void Initialize([In] HandleRef hwndParent, [In, MarshalAs(UnmanagedType.LPStruct)] RECT prc, [In, MarshalAs(UnmanagedType.LPStruct)] FOLDERSETTINGS pfs);

			/// <summary>Destroys the browser.</summary>
			void Destroy();

			/// <summary>Sets the size and position of the view windows created by the browser.</summary>
			/// <param name="phdwp">A pointer to a DeferWindowPos handle. This parameter can be NULL.</param>
			/// <param name="rcBrowser">The coordinates that the browser will occupy.</param>
			void SetRect(IntPtr phdwp, RECT rcBrowser);

			/// <summary>Sets the name of the property bag.</summary>
			/// <param name="pszPropertyBag">
			/// A pointer to a constant, null-terminated, Unicode string that contains the name of the property bag. View state information
			/// that is specific to the application of the client is stored (persisted) using this name.
			/// </param>
			void SetPropertyBag([In, MarshalAs(UnmanagedType.LPWStr)] string pszPropertyBag);

			/// <summary>Sets the default empty text.</summary>
			/// <param name="pszEmptyText">A pointer to a constant, null-terminated, Unicode string that contains the empty text.</param>
			void SetEmptyText([In, MarshalAs(UnmanagedType.LPWStr)] string pszEmptyText);

			/// <summary>Sets the folder settings for the current view.</summary>
			/// <param name="pfs">A pointer to a FOLDERSETTINGS structure that contains the folder settings to be applied.</param>
			void SetFolderSettings([In, MarshalAs(UnmanagedType.LPStruct)] FOLDERSETTINGS pfs);

			/// <summary>Initiates a connection with IExplorerBrowser for event callbacks.</summary>
			/// <param name="psbe">A pointer to the IExplorerBrowserEvents interface of the object to be advised of IExplorerBrowser events.</param>
			/// <param name="pdwCookie">
			/// When this method returns, contains a token that uniquely identifies the event listener. This allows several event listeners
			/// to be subscribed at a time.
			/// </param>
			void Advise([In] IExplorerBrowserEvents psbe, out uint pdwCookie);

			/// <summary>Terminates an advisory connection.</summary>
			/// <param name="dwCookie">
			/// A connection token previously returned from IExplorerBrowser::Advise. Identifies the connection to be terminated.
			/// </param>
			void Unadvise([In] uint dwCookie);

			/// <summary>Sets the current browser options.</summary>
			/// <param name="dwFlag">One or more EXPLORER_BROWSER_OPTIONS flags to be set.</param>
			void SetOptions([In] EXPLORER_BROWSER_OPTIONS dwFlag);

			/// <summary>Gets the current browser options.</summary>
			/// <returns>When this method returns, contains the current EXPLORER_BROWSER_OPTIONS for the browser.</returns>
			EXPLORER_BROWSER_OPTIONS GetOptions();

			/// <summary>Browses to a pointer to an item identifier list (PIDL)</summary>
			/// <param name="pidl">
			/// A pointer to a const ITEMIDLIST (item identifier list) that specifies an object's location as the destination to navigate to.
			/// This parameter can be NULL. For more information, see Remarks.
			/// </param>
			/// <param name="uFlags">A flag that specifies the category of the pidl. This affects how navigation is accomplished.</param>
			void BrowseToIDList([In] PIDL pidl, [In] SBSP uFlags);

			/// <summary>Browses to an object.</summary>
			/// <param name="punk">A pointer to an object to browse to. If the object cannot be browsed, an error value is returned.</param>
			/// <param name="uFlags">A flag that specifies the category of the pidl. This affects how navigation is accomplished.</param>
			void BrowseToObject([In, MarshalAs(UnmanagedType.IUnknown)] object punk, [In] SBSP uFlags);

			/// <summary>Creates a results folder and fills it with items.</summary>
			/// <param name="punk">
			/// An interface pointer on the source object that will fill the IResultsFolder. This can be an IDataObject or any object that
			/// can be used with INamespaceWalk.
			/// </param>
			/// <param name="dwFlags">One of the EXPLORER_BROWSER_FILL_FLAGS values.</param>
			void FillFromObject([In, MarshalAs(UnmanagedType.IUnknown)] object punk, [In] EXPLORER_BROWSER_FILL_FLAGS dwFlags);

			/// <summary>Removes all items from the results folder.</summary>
			void RemoveAll();

			/// <summary>Gets an interface for the current view of the browser.</summary>
			/// <param name="riid">A reference to the desired interface ID.</param>
			/// <returns>
			/// When this method returns, contains the interface pointer requested in riid. This will typically be IShellView, IShellView2,
			/// IFolderView, or a related interface.
			/// </returns>
			[return: MarshalAs(UnmanagedType.IUnknown)]
			object GetCurrentView([In, MarshalAs(UnmanagedType.LPStruct)] Guid riid);
		}

		/// <summary>Exposes methods for notification of Explorer browser navigation and view creation events.</summary>
		[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("361bbdc7-e6ee-4e13-be58-58e2240c810f"), CoClass(typeof(ExplorerBrowser))]
		[PInvokeData("Shobjidl.h", MSDNShortId = "802d547f-41c2-4c4a-9f07-be615d7b86eb")]
		public interface IExplorerBrowserEvents
		{
			/// <summary>Notifies clients of a pending Explorer browser navigation to a Shell folder.</summary>
			/// <param name="pidlFolder">A PIDL that specifies the folder.</param>
			void OnNavigationPending([In] PIDL pidlFolder);

			/// <summary>Notifies clients that the view of the Explorer browser has been created and can be modified.</summary>
			/// <param name="psv">A pointer to an IShellView.</param>
			void OnViewCreated([In] IShellView psv);

			/// <summary>Notifies clients that the Explorer browser has successfully navigated to a Shell folder.</summary>
			/// <param name="pidlFolder">A PIDL that specifies the folder.</param>
			void OnNavigationComplete([In] PIDL pidlFolder);

			/// <summary>Notifies clients that the Explorer browser has failed to navigate to a Shell folder.</summary>
			/// <param name="pidlFolder">A PIDL that specifies the folder.</param>
			void OnNavigationFailed([In] PIDL pidlFolder);
		}


		/// <summary>The ExplorerBrowser class is the base CoClass for all I ExplorerBrowser interfaces.</summary>
		[ComImport, Guid("71f96385-ddd6-48d3-a0c1-ae06e8b055fb"), ClassInterface(ClassInterfaceType.None)]
		[PInvokeData("Shobjidl.h", MSDNShortId = "da2cf5d4-5a68-4d18-807b-b9d4e2712c10")]
		public class ExplorerBrowser { }
	}
}