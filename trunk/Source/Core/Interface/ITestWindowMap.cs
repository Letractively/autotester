/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ITestWindowMap.cs
*
* Description: This interface defines methods to find a sub window/sub page.
*              An application may have more than 1 windows, and a website
*              may also contains more than 1 page, you may have main page,
*              popup page, use this interface to find these sub pages.              
*    
*
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public interface ITestWindowMap
    {
        ITestObjectMap Page(int index);
        ITestObjectMap Page(string title, string url);
        ITestObjectMap NewPage();
        ITestObjectMap Window(int index);
        ITestObjectMap Window(string title, string className);
        ITestObjectMap NewWindow();
    }
}
