
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

namespace Shrinerain.AutoTester.Core
{
    public interface ITestBrowser : ITestApp
    {
        #region

        event TestAppEventHandler OnBrowserStart;
        event TestAppEventHandler OnBrowserNavigate;
        event TestAppEventHandler OnBrowserTabChange;
        event TestAppEventHandler OnBrowserBack;
        event TestAppEventHandler OnBrowserForward;
        event TestAppEventHandler OnBrowserRefresh;
        event TestAppEventHandler OnBrowserClose;

        #endregion

        //Browser actions
        void Start();
        void Load(String url, bool waitForPage);

        void Back();
        void Forward();
        void Refresh();
        void WaitForPage(); //wait for next page, in the same internet explorer window.

        //Browser information
        string GetBrowserName(); //get the  browser name, eg: Internet Explorer
        string GetBrowserVersion(); //get the browser version, eg: 7.0

        //find sub pages/tabs
        ITestBrowser GetPage(int index);
        ITestBrowser GetPage(String title, String url);
        ITestBrowser GetMostRecentPage();
    }
}
