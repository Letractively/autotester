
/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ITestBrowser.cs
*
* Description: This interface define the action provide by browser.
*              Automation tester can control browser by these methods.
*              Also, object pool must get an instance of test browser.
*
* History: 2007/09/04 wan,yu  Init version.
*          2007/12/26 wan,yu add void Find(object) method.
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Interface
{
    [CLSCompliant(true)]
    public interface ITestBrowser
    {
        //Browser actions
        void Start();

        //find an browser, sometimes we already start a browser, we can just use it, don't need to start a new one.
        void Find(object mark);
        void Close();
        void Back();
        void Forward();
        void Refresh();
        void Load(String url);
        void Wait(int seconds);  //wait for seconds, the default is 120s.
        void WaitForNextPage(); //wait for next page, in the same internet explorer window.
        void WaitForNewWindow(); // wait for a new internet explorer instance.
        void WaitForNewTab();    // in tab browser, like IE 7. wait for a new tab.
        void WaitForPopWindow(); // wait for pop up page or a dialog. in the same internet explorer window.

        void Move(int x, int y);
        void Resize(int width, int height);
        void MaxSize(); //set the internet explorer to max size.
        void Active();  //active browser, make sure when testing, it is topmost

        //Browser information
        string GetBrowserName(); //get the  browser name, eg: Internet Explorer
        string GetBrowserVersion(); //get the browser version, eg: 7.0

    }
}
