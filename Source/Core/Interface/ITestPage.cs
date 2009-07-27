using System;
using System.Collections.Generic;
using System.Text;

using mshtml;

namespace Shrinerain.AutoTester.Core.Interface
{
    public interface ITestPage
    {
        String Title { get; }
        String URL { get; }
        ITestObjectMap Objects { get; }
        ITestBrowser Browser { get; }
        IHTMLDocument Document { get; }

        IHTMLDocument GetDocument();
        //return all documents, include frames.
        IHTMLDocument[] GetAllDocuments();
        String GetAllHTML();

        ITestObjectPool GetObjectPool();
    }
}
