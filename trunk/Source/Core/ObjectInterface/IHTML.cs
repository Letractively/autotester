using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    using mshtml;
    public interface IHTML
    {
        string GetTag();
        IHTMLElement GetHTMLElement();
    }
}
