using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core.Interface
{
    public interface ITestPage
    {
        int Left { get; }
        int Top { get; }
        int Width { get; }
        int Height { get; }
        IntPtr Handle { get; }

        String Title { get; }
        String URL { get; }
        ITestObjectMap Objects { get; }
        ITestBrowser Browser { get; }
        ITestDocument Document { get; }

        ITestDocument GetDocument();
        //return all documents, include frames.
        ITestDocument[] GetAllDocuments();
        String GetAllHTMLContent();

        ITestObjectPool GetObjectPool();
        ITestObjectMap GetObjectMap();
    }
}
