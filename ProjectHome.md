# AutoTester.Net #

## About ##
AutoTester.Net is a FREE Web/Windows App automation tool, written in C#/C++.


## Function ##
AutoTester.Net can record/playback user actions on webpages and windows applications.
You can use it to do automated testing, or you can use it to simulate user actions on webpages and windows applications.

For example, every day you may need to open gmail, you need to input your username and password, then write mail to someone. You can use AutoTester.Net to simulate these actions.

AutoTester.Net can compile the scripts to a single exe file. You don't have to install any software to run this scripts.


## Example ##
Open browser -> navigate to Google.com -> input some thing in the textbox -> click "Google Search".

```
using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.HTMLUtility;

namespace AutoTesterTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            HTMLTestSession ts = new HTMLTestSession();
            ts.Browser.Start(); //start browser
            ts.Browser.Load("google.com", true); //load google.com, wait until finish.
            ts.Objects.TextBox().Input("AutoTester shrinerain"); //input something.
            ts.Objects.Button("Google Search").Click(); //click search button.
        }
    }
}
```


## Contact me ##
MSN: shrinerain@hotmail.com


---


## 关于 ##
AutoTester.Net是一个开源自动化程序, 支持网页以及Windows应用程序用户操作的录制/回放, 采用C#/C++编写.


## 功能 ##
AutoTester.Net可用于录制用户在网页或者Windows应用程序上的操作, 并且回放. 当然也可以直接编写代码, 模拟用户对网页或者Windows应用程序的操作.

比如, 可能您每天都会打开gmail, 首先您得输入用户名密码, 然后写封email发给某人. 您可以采用AutoTester.Net来模拟这一系列动作. 然后, 您只需要运行这个程序, AutoTester.Net将会打开浏览器, 输入您的用户名密码, 撰写email并发送.


## 例子 ##
下面这个例子向您展示几个最基本的操作.
打开浏览器 -> 浏览google.com -> 在输入框中输入"shrinerain autotester" -> 点击"Google Search"按钮.

```
using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.HTMLUtility;

namespace AutoTesterTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            HTMLTestSession ts = new HTMLTestSession();
            ts.Browser.Start(); //start browser
            ts.Browser.Load("google.com", true); //load google.com, wait until finish.
            ts.Objects.TextBox().Input("AutoTester shrinerain"); //input something.
            ts.Objects.Button("Google Search").Click(); //click search button.
        }
    }
}
```


## 联系我 ##
MSN: shrinerain@hotmail.com

QQ群: 90799215