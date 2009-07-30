using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using mshtml;
using SHDocVw;
using Accessibility;

using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.Core.Helper
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

            return null;
        }

        public static IHTMLDocument GetFrameDocument(IHTMLWindow2 frameWindow)
        {
            if (frameWindow != null)
            {
                bool ex = false;
                try
                {
                    return frameWindow.document as IHTMLDocument;
                }
                catch
                {
                    ex = true;
                }

                if (ex)
                {
                    try
                    {
                        IWebBrowser2 browser = HTMLWindowToWebBrowser(frameWindow);
                        if (browser != null)
                        {
                            return browser.Document as IHTMLDocument;
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return null;
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

        public static IHTMLDocument[] GetFrames(IHTMLDocument doc)
        {
            if (doc != null)
            {
                try
                {
                    IHTMLDocument2 doc2 = doc as IHTMLDocument2;
                    if (doc2 != null)
                    {
                        IHTMLFramesCollection2 frames = doc2.frames;
                        if (frames != null && frames.length > 0)
                        {
                            List<IHTMLDocument> res = new List<IHTMLDocument>();
                            for (int i = 0; i < frames.length; i++)
                            {
                                object index = i;
                                IHTMLWindow2 frame = frames.item(ref index) as IHTMLWindow2;
                                IHTMLDocument temp = GetFrameDocument(frame);
                                if (temp != null)
                                {
                                    res.Add(temp);
                                }
                            }
                            return res.ToArray();
                        }
                    }
                }
                catch
                {
                }
            }

            return null;
        }

        public static IHTMLElement MSAAToIHTMLElement(IAccessible pacc)
        {
            if (pacc == null)
            {
                return null;
            }

            IServiceProvider spServiceProvider = pacc as IServiceProvider;
            if (spServiceProvider == null)
            {
                return null;
            }

            object spHtmlElement;
            Guid guid = typeof(IHTMLElement).GUID;
            int hRes = spServiceProvider.QueryService(ref guid, ref guid, out spHtmlElement);
            if (hRes != 0)
            {
                return null;
            }

            return spHtmlElement as IHTMLElement;
        }

        public static IAccessible IHTMLElementToMSAA(IHTMLElement element)
        {
            return ObjectToMSAA(element);
        }

        public static IAccessible ObjectToMSAA(object element)
        {
            if (element == null)
            {
                return null;
            }

            IServiceProvider spServiceProvider = element as IServiceProvider;
            if (spServiceProvider == null)
            {
                return null;
            }

            object spAccessible;
            Guid guid = typeof(IAccessible).GUID;
            int hRes = spServiceProvider.QueryService(ref guid, ref guid, out spAccessible);
            if (hRes != 0)
            {
                return null;
            }

            return spAccessible as IAccessible;
        }

        public static IMarkupContainer2 GetMarkupContainer(IHTMLDocument2 doc2)
        {
            if (doc2 != null)
            {
                try
                {
                    object oDocument = doc2;
                    IntPtr pDocument = Marshal.GetIUnknownForObject(oDocument);
                    IntPtr pMarkupContainer = IntPtr.Zero;
                    Guid IMarkupContainer2GUID = typeof(mshtml.IMarkupContainer2).GUID;
                    Marshal.QueryInterface(pDocument, ref IMarkupContainer2GUID, out pMarkupContainer);
                    if (pMarkupContainer != IntPtr.Zero)
                    {
                        object oMarkupContainer = Marshal.GetUniqueObjectForIUnknown(pMarkupContainer);
                        return oMarkupContainer as mshtml.IMarkupContainer2;
                    }
                }
                catch
                {
                }
            }

            return null;
        }
    }
}
