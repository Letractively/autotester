using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public interface ITestWindowMap
    {
        ITestObjectMap Page(int index);
        ITestObjectMap Page(string title, string url);
        ITestObjectMap NewPage();
        ITestObjectMap Window(int index);
        ITestObjectMap Window(string title, string className);
        ITestObjectMap NewWindow();
    }
}
