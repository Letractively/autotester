using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public interface IHighLight
    {
        //high light the object.
        void HighLight();
        void HighLight(int mseconds);
        void Restore();
    }
}
