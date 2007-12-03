using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Function.Interface
{
    public interface ISelectable : IInteractive, IShowInfo
    {
        void Select(string str);
        void SelectByIndex(int index);
        String[] GetAllValues();
    }
}
