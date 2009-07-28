using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core.Interface
{
    public interface ITestDocument
    {
        Object GetElementByID(String id);
        Object GetElementByPoint(int x, int y);
        Object[] GetElementsByName(string name);
        Object[] GetElementsByTagName(String tag);
        Object[] GetAllElements();
        String GetHTMLContent();

        ITestDocument[] GetFrames();
        ITestDocument GetParent();

        String Title { get; }
        String URL { get; }
        ITestPage Page { get; }
    }
}
