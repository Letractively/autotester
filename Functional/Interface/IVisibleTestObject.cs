using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

namespace Shrinerain.AutoTester.Function.Interface
{
    interface IVisibleTestObject
    {
        public Point GetCenterPoint();
        public Rectangle GetRect();
    }
}
