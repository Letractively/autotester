using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core.Interface
{
    public interface ITestPageMap
    {
        ITestObjectMap Page(int index);
        ITestObjectMap Page(string title, string url);
        ITestObjectMap NewPage();
    }
}
