using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public interface ITestSession
    {
        ITestApp App { get; }
        ITestBrowser Browser { get; }
        ITestObjectMap Objects { get; }
        ITestWindowMap Windows { get; }
        ITestPageMap Pages { get; }
        ITestEventDispatcher Event { get; }
        ITestCheckPoint CheckPoint { get; }
    }
}
