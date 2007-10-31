using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

namespace Shrinerain.AutoTester.Function.Interface
{
    public interface IVisible
    {
        Point GetCenterPoint();
        Rectangle GetRect();
        Bitmap GetControlPrint();

        void Hover(); //move mouse to the top of control
    }
}
