using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Interface
{
    public interface ITestBrowser
    {
        //Browser actions
        void Start();
        void Close();
        void Back();
        void Forward();
        void Refresh();
        void Load(String url);
        void Wait(int seconds);
        void WaitForNewPage();
        void WaitForNewWindow();

        void Move(int x, int y);
        void Resize(int width, int height);

    }
}
