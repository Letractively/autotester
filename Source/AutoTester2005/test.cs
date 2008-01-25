/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component:
*
* Description:
*
* History:
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;

using Shrinerain.AutoTester.HTMLUtility;
using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Framework;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester
{
    class Program
    {
        static void Main(string[] args)
        {
            //FrameworkEXE(args);
            FunctionTest();
            //FrameworkTest();
            Console.ReadLine();
        }


        static void FrameworkTest()
        {
            string projectConfigFile = @"D:\program\cs\AutoTester\Document\google.xml";//  @"E:\program\cs\AutoTester\Document\google.xml"; //  
            string frameworkConfigFile = @"D:\Program\CS\AutoTester2005\Framework\Framework.config";

            TestJob myTestJob = new TestJob();
            myTestJob.ProjectConfigFile = projectConfigFile;
            myTestJob.FrameworkConfigFile = frameworkConfigFile;

            myTestJob.StartTesting();
        }

        static void FrameworkEXE(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage:Framework.exe /p:projectConfig /f:frameworkConfig");
                return;
            }

            string frameworkConfigFile = null;
            string projectConfigFile = null;

            foreach (string p in args)
            {
                if (p.StartsWith("/p:", StringComparison.InvariantCultureIgnoreCase))
                {
                    projectConfigFile = p.Remove(0, 3);
                }
                else if (p.StartsWith("/f:", StringComparison.InvariantCultureIgnoreCase))
                {
                    frameworkConfigFile = p.Remove(0, 3);
                }
                else
                {
                    Console.WriteLine("Error: Unsupported Argument: " + p);
                    return;
                }
            }

            TestJob myTestJob = new TestJob();
            myTestJob.ProjectConfigFile = projectConfigFile;
            myTestJob.FrameworkConfigFile = frameworkConfigFile;

            myTestJob.StartTesting();

        }

        static void FunctionTest()
        {

            //TestLog myLog = new TestLog();
            //myLog.TestlogTemplate = @"E:\program\cs\AutoTester\Document\log.template";
            //myLog.LogFile = @"G:\testlog.html";

            //myLog.ProjectName = "Google Test";
            //myLog.TestStep = "this is a test step";
            //myLog.TestResultInfo = "PASS!";

            //myLog.WriteLog();
            //myLog.Close();

            //return;

            HTMLTestBrowser myHost = new HTMLTestBrowser();
            myHost.Start();
            myHost.MaxSize();

            string url = @"http://google.com";// @"https://www.google.com/accounts/Login?continue=http://www.google.cn/&hl=zh-CN";// @"http://passport.baidu.com/?login&tpl=mn&u=http%3A//www.baidu.com/";// @"D:\Program\CS\AutoTester2005\test.htm";//@"http://passport.baidu.com/?login&tpl=mn&u=http%3A//www.baidu.com/";// @"http://www.cnbeta.com";//  @"www.google.com"; // @"http://www.google.com/advanced_search?hl=en";// @"http://127.0.0.1/AutoTester/";// 
            myHost.Load(url);

            HTMLTestObjectPool pool = new HTMLTestObjectPool(myHost);

            //HTMLTestTable table = (HTMLTestTable)pool.GetObjectByType("table", "111", 0);

            //HTMLTestButton btn = (HTMLTestButton)table.GetObjectByCell(0, 0, HTMLTestObjectType.Button);
            //btn.Click();
            //pool.GetObjectByType("textbox", "�����ʼ�:", 0);

            //aHelper.PerformAction("test");

            HTMLTestTextBox obj1 = (HTMLTestTextBox)pool.GetObjectByRect(432, 285, 365, 22, "textbox"); //pool.GetObjectByProperty("name", "q"); //pool.GetObjectByName("q");
            obj1.Input("statestreet");

            //HTMLTestCheckBox cb = (HTMLTestCheckBox)pool.GetObjectByType("checkbox", "�ڴ˼�����ϱ����ҵ���Ϣ��", 0);

            //// HTMLTestCheckBox cb = (HTMLTestCheckBox)pool.GetObjectByType("checkbox", "��ס�ҵĵ�¼״̬", 0);
            //cb.UnCheck();

            //pool.GetObjectByType("button", "��½", 0);

            //aHelper.PerformAction();

            //HTMLTestTextBox userName = (HTMLTestTextBox)pool.GetObjectByType("textbox", "�û���:", 0);
            //userName.Hover();

            //   HTMLTestLink link = (HTMLTestLink)pool.GetObjectByType("link", "ooo", 0);


            //  HTMLTestTable table = (HTMLTestTable)pool.GetObjectByType("table", "111", 0);
            //HTMLTestButton btn = (HTMLTestButton)table.GetObjectByCell(0, 0);
            //btn.Click();

            // HTMLTestTextBox rbtn = (HTMLTestTextBox)pool.GetObjectByType("textbox", "1", 0);
            //rbtn.Hover();

            //HTMLTestComboBox cbBox = (HTMLTestComboBox)pool.GetObjectByName("num");
            // cbBox.Select("20 results");



            //HTMLTestButton obj2 = (HTMLTestButton)pool.GetObjectByType("button", "Google Sear", 0);
            //obj2.Click();

            //  myHost.WaitForNextPage();




            //HTMLTestLink linkObj =(HTMLTestLink)pool.GetObjectByID("link1");

            // linkObj.Click();

            //HTMLTestButton btn = (HTMLTestButton)pool.GetObjectByID("btn1");
            //btn.Click();
            //Thread.Sleep(1000);

            //myHost.WaitForPopWindow();

            //btn = (HTMLTestButton)pool.GetObjectByID("btn2");
            //btn.Click();


            //  HTMLTestListBox listBox = (HTMLTestListBox)pool.GetObjectByID("ListBox1");
            // listBox.SelectByIndex(5);
        }
    }
}
