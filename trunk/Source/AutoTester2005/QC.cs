using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;

using Shrinerain.AutoTester.HTMLUtility;
using Shrinerain.AutoTester.MSAAUtility;
using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Framework;
using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Helper;

namespace Shrinerain.AutoTester
{
    public class QC
    {
        public static void Test()
        {
            LeftTree();

            //Login();

        }
        public static void Login()
        {
            TestBrowser browser = new TestBrowser();
            string url = @"http://cnst50063845:8080/qcbin/start_a.htm";
            browser.Load(url);
            string title = "Mercury Quality Center 9.0";
            //browser.Find(title);
            browser.Wait(10);

            MSAATestObjectPool pool = new MSAATestObjectPool();
            pool.SetTestBrowser(browser);

            MSAATestButton btn;
            btn = (MSAATestButton)pool.GetObjectByType("button", "Authenticate", 0);
            btn.Click();

            browser.Wait(1);

            btn = (MSAATestButton)pool.GetObjectByType("button", "Login", 0);
            btn.Click();

            browser.Wait(10);
        }

        public static void LeftTree()
        {
            TestBrowser browser = new TestBrowser();
            //string url = @"http://cnst50063845:8080/qcbin/start_a.htm";
            //browser.Load(url);
            string title = "Mercury Quality Center 9.0";
            browser.Find(title);
            browser.Wait(10);

            MSAATestObjectPool pool = new MSAATestObjectPool();
            pool.SetTestBrowser(browser);

            MSAATestGUIObject obj = (MSAATestGUIObject)pool.GetObjectByPoint(150, 265); //pool.GetObjectByName("Data_Bank");
            obj.Hover();

        }

    }
}
