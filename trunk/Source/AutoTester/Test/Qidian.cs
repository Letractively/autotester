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
    class Qidian
    {
        public static void Test()
        {
            string url = @"www.qidian.com";// @"http://mm.cmfu.com/vip/ebook_subscibe_forbuy.asp?ebookid=367736&bl_id=149430";//
            HTMLTest test = new HTMLTest();
            //test.Browser.Find("小说原创门户");
            //int st = test.Browser.ScrollTop;

            //test.SendMsgOnly = true;
            //test.Browser.Start();

            test.Browser.Load(url);
            // test.Map.TextBox("用户名").Input("shrinerain");
            // test.Map.TextBox("密码").Input("jyoicq");
            // test.Map.Button("登录").Click();

            //test.Browser.WaitForNextPage();
            //test.Map.Link("我的书架").Click();

            //test.Map.Label("关于如何在新站操作书评加精华和置顶").GetFontColor();
            //test.Map.Link("搜书").Click();
            //test.Map.Link("小亨传说").Click();
            Console.WriteLine(test.Browser.GetLoadSeconds());

            //test.Browser.Load("www.google.com");

            test.Map.TextBox("关键字").Input("变身");
            //test.Map.Link("提交建议").Click();
            test.Map.Button("搜索").Click();
            test.Map.Link("下一页").DoAction(null);
            test.Map.Link("意乱情迷").Click();
            //test.Map.RadioBtn("好友可见").Check();
            //test.Map.CheckBox("[历史]").Check();
            //test.Map.Button("退出").Click();

            //test.Pool.GetObjectByType("textbox", "用户名", 0);
            //test.Map.TextBox().Input("shrinerain");

            //test.Pool.GetObjectByType("textbox", "密码", 0);
            //test.Map.TextBox().Input("jyoicq");

            //test.Pool.GetObjectByType("button", "登陆", 0);
            //test.Map.Button().Click();



            Console.ReadLine();
        }
    }
}