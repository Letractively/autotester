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
    class MSAA
    {
        public static void Test()
        {

            TestBrowser tb = new TestBrowser();
            // string url = @"www.google.cn";
            //tb.Load(url);
            tb.Find("Google");

            MSAATestObjectPool pool = new MSAATestObjectPool();
            pool.SetTestBrowser(tb);


            MSAATestGUIObject obj = (MSAATestGUIObject)pool.GetObjectByType("button", "Google 搜索", 0);
            obj.Hover();

        }
    }
}