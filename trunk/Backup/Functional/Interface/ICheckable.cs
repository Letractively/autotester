using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Function.Interface
{
    public interface ICheckable : IClickable
    {
        void Check();
        void UnCheck();
        void IsChecked();
    }
}
