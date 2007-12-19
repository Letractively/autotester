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
using Shrinerain.AutoTester.Function;
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
            string projectConfigFile = @"E:\program\cs\AutoTester\Document\PE\PE.xml";//   @"E:\program\cs\AutoTester\Document\google.xml"; //  
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

            //TestLog myLog = new TestLog();
            //myLog.TestlogTemplate = @"E:\program\cs\AutoTester\Document\log.template";
            //myLog.LogFile = @"G:\testlog.html";

            //myLog.ProjectName = "Google Test";
            //myLog.TestStep = "this is a test step";
            //myLog.TestResultInfo = "PASS!";

            //myLog.WriteLog();
            //myLog.Close();

            //return;

            HTMLTestBrowser myHost = HTMLTestBrowser.GetInstance();
            myHost.Start();
            myHost.MaxSize();

            string url = @"http://127.0.0.1/AutoTester/test.htm";// @"http://192.168.17.111:9081/wps/portal/!ut/p/.scr/Login"; //@"www.google.com"; // 
            myHost.Load(url);

            myHost.Wait(5);

            Console.WriteLine(TestBrowser.ScrollTop);


            HTMLTestObjectPool pool = new HTMLTestObjectPool(myHost);

            HTMLTestTextBox userName = (HTMLTestTextBox)pool.GetObjectByType("Textbox", "1", 0);
            userName.Input("wpsadmin");
            

            HTMLTestTextBox password = (HTMLTestTextBox)pool.GetObjectByType("Textbox", "2", 0);
            password.Input("wpsadmin");

            HTMLTestButton login = (HTMLTestButton)pool.GetObjectByType("button", "Log in", 0);
            login.PerformDefaultAction(new object());

            myHost.Wait(8);//.WaitForNextPage();

            HTMLTestLink peSecurity = (HTMLTestLink)pool.GetObjectByType("link", "PESecurityPortal", 0);// .GetObjectByProperty(".class", "wpsUnSelectedPageLink");
            peSecurity.Click();

            myHost.Wait(2);

            HTMLTestLink addNewUserLink = (HTMLTestLink)pool.GetObjectByType("link", "Add New User", 0);
            addNewUserLink.Click();

            myHost.Wait(2);

            HTMLTestTextBox userID = (HTMLTestTextBox)pool.GetObjectByName("userId");
            userID.Input("peautomationframework2");

            HTMLTestTextBox firstName = (HTMLTestTextBox)pool.GetObjectByName("userFName");
            firstName.Input("pe auto");

            HTMLTestTextBox lastName = (HTMLTestTextBox)pool.GetObjectByName("userLName");
            lastName.Input("smoke test");


            HTMLTestComboBox dept = (HTMLTestComboBox)pool.GetObjectByID("depSelect"); //GetObjectByType("combobox", "Select Dept", 0);
            dept.Select("PRIVTEDGE");

            HTMLTestComboBox client = (HTMLTestComboBox)pool.GetObjectByID("clients");
            client.Select("Auto2007CLT");

            myHost.Wait(2);

            HTMLTestImage addAll = (HTMLTestImage)pool.GetObjectByType("Image", "addAll.gif", 0);
            addAll.Click();

            HTMLTestImage next = (HTMLTestImage)pool.GetObjectByType("Image", "next.gif", 0);
            next.Click();

            myHost.Wait(2);

            HTMLTestLink viewLink = (HTMLTestLink)pool.GetObjectByType("link", "view", 0);
            viewLink.Click();

            HTMLTestImage next2 = (HTMLTestImage)pool.GetObjectByType("Image", "next.gif", 0);
            next2.Click();

            myHost.Wait(2);

            HTMLTestLink viewLink2 = (HTMLTestLink)pool.GetObjectByType("link", "view", 0);
            viewLink2.Click();

            HTMLTestImage next3 = (HTMLTestImage)pool.GetObjectByType("Image", "next.gif", 0);
            next3.Click();

            myHost.Wait(2);

            HTMLTestImage submit = (HTMLTestImage)pool.GetObjectByType("Image", "submit.gif", 0);
            submit.Click();


            //   HTMLTestTextBox obj1 = (HTMLTestTextBox)pool.GetObjectByName("q");
            // obj1.Input("statestreet");

            //HTMLTestButton obj2 = (HTMLTestButton)pool.GetObjectByType("button", "Google Search", 0);
            //obj2.Click();

            //myHost.WaitForNextPage();




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
