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
* History: 2007/11/20 wan,yu Init version
*          2008/01/15 wan,yu update, add Wait() methods 
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Shrinerain.AutoTester.Interface
{
    [CLSCompliant(true)]
    public interface ITestApp
    {
        //operation on desktop applicatoin
        void Start(string appFullPath);
        void Start(string appFullPath, string[] parameters);
        void Close();

        void Move(int x, int y);
        void Resize(int left, int top, int width, int height);
        void Max();
        void Min();

        //sync methods
        void Wait(int seconds);
        void WaitForExist();
        void WaitForDisappear();

        // restore to the origin size.
        void Restore();
        void Active();

        //get status of desktop application
        bool IsActive();
        bool IsTopMost();
        bool IsVisible();
        bool IsMax();
        bool IsMin();
        bool IsIcon();
        bool IsBusy();
        bool IsVisualStyle();
        bool IsTaskbar();

        //get size
        int GetTop();
        int GetLeft();
        int GetHeight();
        int GetWidth();

        //get network information
        int[] GetPortNumber();
        bool IsConnected();

        //get process information
        int GetProcessID();
        Process GetProcess();
        int GetThreadCount();

        //get performance information
        long GetCPUTime(); //return the total CPU time, in Millisecond.
        long GetMemory();  //return the physical memory, in byte.
        long GetIORead();  //return the IO read, in byte.
        long GetIOWrite(); //return the IO write, in byte.

        //other information
        string GetAppName();
        string GetVersion();
        string GetCompany();
        string GetAuthor();

    }
}
