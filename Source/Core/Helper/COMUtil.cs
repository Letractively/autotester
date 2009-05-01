using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using mshtml;
using SHDocVw;

using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.Core
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
    public interface _IServiceProvider
    {
        void QueryService(
            ref Guid guid,
            ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out Object Obj);
    }

    [ComImport(), ComVisible(true), Guid("6D5140C1-7436-11CE-8034-00AA006009FA"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IServiceProvider
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int QueryService(ref Guid guidService, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppvObject);
    }

    public class COMUtil
    {
        public static readonly Guid IID_IWebBrowserApp = new Guid("{0002DF05-0000-0000-C000-000000000046}");
        public static readonly Guid IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");
        public static readonly Guid IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");

        /*  HTMLDocument GetHTMLDomFromHandle(IntPtr ieServerHandle)
       *  return HTMLDocument from a handle.
       *  When we get a pop up window, we need to get it's HTML Dom to get HTML object.
       *  So we need to convert it's handle to HTML Document.
       */
        public static HTMLDocument GetHTMLDocFromHandle(IntPtr ieServerHandle)
        {
            if (ieServerHandle != IntPtr.Zero && Win32API.GetClassName(ieServerHandle) == TestConstants.IE_Server_Class)
            {
                int nMsg = Win32API.RegisterWindowMessage("WM_HTML_GETOBJECT");
                UIntPtr lRes;
                if (Win32API.SendMessageTimeout(ieServerHandle, nMsg, 0, 0, Win32API.SMTO_ABORTIFHUNG, 1000, out lRes) == 0)
                {
                    return null;
                }
                return (HTMLDocument)Win32API.ObjectFromLresult(lRes, typeof(HTMLDocument).GUID, IntPtr.Zero);
            }
            else
            {
                return null;
            }
        }

        public static HTMLDocument GetFrameDocument(IHTMLWindow2 frameWindow)
        {
            HTMLDocument doc = null;
            try
            {
                doc = frameWindow.document as HTMLDocument;
            }
            catch
            {
            }

            if (doc == null)
            {
                try
                {
                    IWebBrowser2 browser = HTMLWindowToWebBrowser(frameWindow);
                    if (browser != null)
                    {
                        doc = browser.Document as HTMLDocument;
                    }
                }
                catch
                {
                }
            }

            return doc;
        }

        public static IWebBrowser2 HTMLWindowToWebBrowser(IHTMLWindow2 spWindow)
        {
            if (spWindow == null) return null;

            IServiceProvider spServiceProvider = spWindow as IServiceProvider;
            if (spServiceProvider == null)
            {
                return null;
            }

            object spWebBrws;
            Guid guid = IID_IWebBrowserApp;
            Guid riid = IID_IWebBrowser2;
            int hRes = spServiceProvider.QueryService(ref guid, ref riid, out spWebBrws);

            if (hRes != 0)
            {
                return null;
            }

            return spWebBrws as IWebBrowser2;
        }
    }
}
