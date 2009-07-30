using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core.Interface
{
    public interface ITestWindow
    {
        int Left { get; }
        int Top { get; }
        int Width { get; }
        int Height { get; }
        IntPtr Handle { get; }

        String Caption { get; }
        String ClassName { get; }
        ITestApp App { get; }
        ITestObjectMap Objects { get; }

        ITestObjectPool GetObjectPool();
        ITestObjectMap GetObjectMap();
    }
}
