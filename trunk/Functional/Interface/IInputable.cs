using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Function.Interface
{
    public interface IInputable : IVisible, IInteractive, IShowInfo
    {
        void Input(string values);
        void InputKeys(string keys);
        void Clear(); //clear text.
    }
}
