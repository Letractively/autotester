using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Function.Interface
{
    public interface IInteractive : IVisible
    {
        void Focus();
        object GetDefaultAction();
        void PerformDefaultAction();
    }
}
