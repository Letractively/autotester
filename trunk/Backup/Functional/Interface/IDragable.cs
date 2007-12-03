using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Shrinerain.AutoTester.Function.Interface
{
    public interface IDragable : IVisible, IInteractive
    {
        void Drag(Point start, Point end);
    }
}
