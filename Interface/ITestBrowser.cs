using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Interface
{
    public interface ITestBrowser
    {
        //Browser actions
        void Start();
        void Close();
        void Back();
        void Forward();
        void Refresh();
        void Load(String url);
        void Wait(int seconds);  //wait for seconds, the default is 120s.
        void WaitForNextPage(); //wait for next page, in the same internet explorer window.
        void WaitForNewWindow(); // wait for another internet explorer window, in IE 7, it is maybe a new tab.
        void WaitForPopWindow(); // wait for pop up page. in the same internet explorer window.

        void Move(int x, int y);
        void Resize(int width, int height);
        void MaxSize(); //set the internet explorer to max size.
        void Active();  //active browser, make sure when testing, it is topmost

        //Browser information
        string GetBrowserName(); //get the  browser name, eg: Internet Explorer
        string GetBrowserVersion(); //get the browser version, eg: 7.0

    }
}
