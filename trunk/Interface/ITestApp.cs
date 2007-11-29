/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ITestApp.cs
*
* Description: This interface define the action provide by desktop 
*              application.
*              Automation tester can control application by these methods.
*              Also, object pool must get an instance of test application.
*
* History: 2007/11/20  wan,yu  Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Interface
{
    public interface ITestApp
    {
        void Start(string appFullPath);
        void Start(string appFullPath, string[] parameters);
        //topMost means we set the Z-level of this application to top. 
        void Start(string appFullPath, string[] parameters, bool topMost);
        void Close();

        void Move(int x, int y);
        void Resize(int left, int top, int width, int height);
        void Max();
        void Min();
        // restore to the origin size.
        void Restore();
        void Active();

        int GetProcessID();
        string GetAppName();
    }
}
