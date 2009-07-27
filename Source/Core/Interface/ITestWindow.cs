using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core.Interface
{
    public interface ITestWindow
    {
        String Caption { get; }
        String ClassName { get; }
        IntPtr Handle { get; }
        ITestApp App { get; }
        ITestObjectMap Objects { get; }
    }
}
