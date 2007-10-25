using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Function.Interface
{
    public interface IShowInfo : IVisible
    {
        string GetText();
        string GetFontStyle();
        string GetFontFamily();
    }
}
