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
using System.Diagnostics;
using System.Drawing;

namespace Shrinerain.AutoTester.Core.Interface
{
    public interface ITestApp
    {
        #region event

        event TestAppEventHandler OnBeforeAppStart;
        event TestAppEventHandler OnAfterAppStart;
        event TestAppEventHandler OnBeforeAppClose;
        event TestAppEventHandler OnAfterAppClose;
        event TestAppEventHandler OnBeforeAppFound;
        event TestAppEventHandler OnAfterAppFound;

        #endregion

        //event dispatcher will fire event to recorder.
        ITestEventDispatcher GetEventDispatcher();
        ITestObjectPool GetObjectPool();
        ITestObjectMap GetObjectMap();
        ITestWindowMap GetWindowMap();

        //operation on desktop applicatoin
        void BeforeStart();
        void Start(string appFullPath);
        void Start(string appFullPath, string parameters);
        void AfterStart();
        void BeforeFound();
        void Find(IntPtr handle);
        void Find(String title);
        void Find(String title, String className);
        void Find(String processName, int index);
        void AfterFound();
        void BoforeClose();
        void Close();
        void AfterClose();

        //find sub windows.
        ITestApp[] GetChildren();
        ITestApp GetParent();
        ITestApp GetWindow(int index);
        ITestApp GetWindow(IntPtr handle);
        ITestApp GetWindow(String title);
        ITestApp GetWindow(String title, String className);
        ITestApp GetMostRecentWindow();

        void Move(int x, int y);
        void Resize(int left, int top, int width, int height);
        void Max();
        void Min();

        //sync methods
        void Wait(int seconds);
        void WaitForExist();

        // restore to the origin size.
        void Restore();
        void Active();
        void Hide(bool hide);

        //get status of desktop application
        bool IsActive();
        bool IsTopMost();
        bool IsVisible();

        //get size
        Rectangle GetRectOnScreen();
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
