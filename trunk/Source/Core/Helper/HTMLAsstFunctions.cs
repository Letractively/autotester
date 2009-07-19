using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using mshtml;
using SHDocVw;

namespace Shrinerain.AutoTester.Core.Helper
{
    public sealed class HTMLAsstFunctions
    {
        public static bool IsDocumentValid(IHTMLDocument2 doc)
        {
            try
            {
                return !String.IsNullOrEmpty(doc.url) && String.Compare(doc.url, TestConstants.IE_BlankPage_Url, true) != 0;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDocumentEqual(IHTMLDocument2 doc1, IHTMLDocument2 doc2)
        {
            try
            {
                return Marshal.GetIUnknownForObject(doc1) == Marshal.GetIUnknownForObject(doc2);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDocumentStateReady(IHTMLDocument2 doc)
        {
            if (doc != null)
            {
                try
                {
                    return doc.readyState == "complete" || doc.readyState == "interactive";
                }
                catch
                {
                }
            }

            return false;
        }
    }
}
