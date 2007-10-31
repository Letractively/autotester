using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Function.Interface
{
    public interface IClickable : IInteractive
    {
        void Click();
        void DoubleClick();
        void RightClick();
        void MiddleClick();
    }
}
