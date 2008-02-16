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
            //Console.ReadLine();
        }


        static void FrameworkTest()
        {
            string projectConfigFile = @"E:\program\cs\AutoTester\Document\google.xml";//  @"E:\program\cs\AutoTester\Document\google.xml"; //  
            string frameworkConfigFile = @"E:\Program\CS\AutoTester\Framework\Framework.config";

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

            qidian();

            return;

            Google();

            return;


            //TestLog myLog = new TestLog();
            //myLog.TestlogTemplate = @"E:\program\cs\AutoTester\Document\log.template";
            //myLog.LogFile = @"G:\testlog.html";

            //myLog.ProjectName = "Google Test";
            //myLog.TestStep = "this is a test step";
            //myLog.TestResultInfo = "PASS!";

            //myLog.WriteLog();
            //myLog.Close();

            //return;


            // ImageHelper.SaveScreenArea(0, 0, 200, 200, @"G:\1111.jpg");

            HTMLTestBrowser myHost = new HTMLTestBrowser();
            myHost.Start();
            myHost.MaxSize();

            string url = @"www.google.com"; // @"https://www.google.com/accounts/Login?continue=http://www.google.cn/&hl=zh-CN";// @"http://127.0.0.1/AutoTester/";// 
            myHost.Load(url);


            HTMLTestObjectPool pool = new HTMLTestObjectPool(myHost);




            //ActionHelper ah = new ActionHelper();
            //ah.TestObjectPool = pool;

            //pool.GetObjectByType("textbox", "�����ʼ�:", 0);

            // ah.PerformAction("shrinerain@gmail.com");

            // pool.GetObjectByType("textbox", "����", 0);


            // pool.GetObjectByType("button", "��¼", 0);
            // ah.PerformAction();

            //   HTMLTestLabel label1 = (HTMLTestLabel)pool.GetObjectByType("label", "Make iGoogle your homepage?", 0);
            // label1.Hover();

            //HTMLTestTextBox obj1 = (HTMLTestTextBox)pool.GetObjectByProperty("name", "q"); //
            //obj1.Input("statestreetfayt");

            //HTMLTestButton obj2 = (HTMLTestButton)pool.GetObjectByType("button", "Google Searc", 0);
            //obj2.Click();

            //        myHost.WaitForNextPage();




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

        public static void Google()
        {

            string url = @"https://www.google.com/accounts/Login?continue=http://www.google.cn/&hl=zh-CN";//@"www.google.com"; //  @"http://127.0.0.1/AutoTester/";// 

            HTMLTest test = new HTMLTest();
            test.Browser.Load(url);
            test.Browser.Wait(5);

            Console.WriteLine("Date: {0:o}", DateTime.Now);
            test.Pool.GetObjectByType("textbox", "�����ʼ�:");
            Console.WriteLine("Date: {0:o}", DateTime.Now);
            Console.ReadLine();
        }


        public static void qidian()
        {
            string url = @"www.qidian.com";// @"http://mm.cmfu.com/vip/ebook_subscibe_forbuy.asp?ebookid=367736&bl_id=149430";//
            HTMLTest test = new HTMLTest();
            test.Browser.Load(url);
            test.Map.TextBox("�û���").Input("shrinerain");
            test.Map.TextBox("����").Input("jyoicq");
            test.Map.Button("ȷ��").Click();

            //test.Browser.WaitForNextPage();
            //test.Map.Link("�ҵ����").Click();

            //test.Pool.GetObjectByType("textbox", "�û���", 0);
            //test.Map.TextBox().Input("shrinerain");

            //test.Pool.GetObjectByType("textbox", "����", 0);
            //test.Map.TextBox().Input("jyoicq");

            //test.Pool.GetObjectByType("button", "��½", 0);
            //test.Map.Button().Click();



            Console.ReadLine();
        }
    }
}
