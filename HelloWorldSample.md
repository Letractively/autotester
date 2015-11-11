Here is a hello world sample to show you how to use AutoTesterLib.dll

1. Create a .Net project.

2. Add AutoTesterLib.dll to your reference.

3. Write code below and then execute.

```
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Drawing;

using Shrinerain.AutoTester.HTMLUtility;
using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester
{
    class Google
    {
        [STAThread]   
        public static void Main()
        {
            TestSession ts = new HTMLTestSession();
            ts.Browser.Start("http://google.com"); //load google
            ts.Objects.TextBox().Input("shrinerain autotester"); //input something
            ts.Objects.Button("Google Search").Click(); //click the button
        }
    }
}
```