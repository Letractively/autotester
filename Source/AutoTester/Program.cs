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

namespace Shrinerain.AutoTester
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Diagnostics;
    using System.Threading;
    using System.Drawing;

    using Shrinerain.AutoTester.HTMLUtility;
    using Shrinerain.AutoTester.Framework;
    using Shrinerain.AutoTester.Core;
    using Shrinerain.AutoTester.Win32;
    using Shrinerain.AutoTester.MSAAUtility;

    class Program
    {
        [STAThread]
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
            //Test1.Test();
            //MSAA.Test();
            //Baidu.Test();
            //Google.Test();
            //Other.Test();
            //Qidian.Test();
            //CSDN.Test();
            // QQ.Test();
            //FCKEditor.Test();
            //Netyi.Test();
            gougou.Test();
        }
    }
}
