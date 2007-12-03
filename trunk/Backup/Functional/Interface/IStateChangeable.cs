using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Function.Interface
{
    public interface IStateChangeable
    {
        object GetCurrentState();
        bool IsReady();
    }
}
