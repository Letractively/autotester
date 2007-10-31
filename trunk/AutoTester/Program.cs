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
            // FrameworkEXE(args);
            FunctionTest();
            //   FrameworkTest();
            Console.ReadLine();
        }


        static void FrameworkTest()
        {
            string projectConfigFile = @"E:\Program\CS\ShrinerainTools\AutoTester\Framework\DriveFile\baidu.xml";
            string frameworkConfigFile = @"E:\Program\CS\ShrinerainTools\AutoTester\Framework\Framework.config";

            TestJob myTestJob = new TestJob();
            myTestJob.ProjectConfigFile = projectConfigFile;
            myTestJob.FrameworkConfigFile = frameworkConfigFile;

            myTestJob.StartTesting();
        }

        static void FrameworkEXE(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage:Framework.exe /p:projectConfig /f:frameworkconfig");
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
                    Console.WriteLine("Error: Unsupported Argument.");
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
            HTMLTestBrowser myHost = HTMLTestBrowser.GetInstance();
            myHost.Start();
            myHost.MaxSize();

            string url = @"http://127.0.0.1/AutoTester/";// @"www.google.cn"; // // 
            myHost.Load(url);


            HTMLTestObjectPool pool = new HTMLTestObjectPool(myHost);

            //   HTMLTestTextBox obj1 = (HTMLTestTextBox)pool.GetObjectByName("q");
            // obj1.Input("statestreet");

            // HTMLTestButton obj2 = (HTMLTestButton)pool.GetObjectByType("button", "Google ËÑË÷", 0);
            //obj2.Click();

            HTMLTestButton btn = (HTMLTestButton)pool.GetObjectByID("btn1");
            btn.Click();
            Thread.Sleep(1000);

            myHost.WaitForPopWindow();

            btn = (HTMLTestButton)pool.GetObjectByID("btn2");
            btn.Click();


            //  HTMLTestListBox listBox = (HTMLTestListBox)pool.GetObjectByID("ListBox1");
            // listBox.SelectByIndex(5);
        }
    }
}
