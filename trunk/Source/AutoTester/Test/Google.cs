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
using System.Drawing;

using Shrinerain.AutoTester.HTMLUtility;
using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Framework;
using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Helper;
using Shrinerain.AutoTester.MSAAUtility;

namespace Shrinerain.AutoTester
{
    class Google
    {
        public static void Test()
        {
            string url = @"www.google.cn"; //@"https://www.google.com/accounts/Login?continue=http://www.google.cn/&hl=zh-CN";//  @"http://127.0.0.1/AutoTester/";// 

            HTMLTest test = new HTMLTest();
            test.Browser.Load(url);
            //test.Browser.Wait(5);

            //Console.WriteLine(test.Map.Label("中文网页"));
            //Console.WriteLine("Date: {0:o}", DateTime.Now);
            //test.Pool.GetObjectByType("textbox", "电子邮件:");
            //Console.WriteLine("Date: {0:o}", DateTime.Now);

            test.Map.TextBox().Input("shrinerain");


            test.Map.Button("Google 搜索").Click();
            Console.WriteLine(test.Browser.GetLoadSeconds());
            Console.ReadLine();

        }
    }
}