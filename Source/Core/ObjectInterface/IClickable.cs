/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: IClickable.cs
*
* Description: This interface defines the actions of an Clickable
*              object. Like button, img, textbox, link etc.
*
* History:  2007/09/04 wan,yu Init version
*
*********************************************************************/


namespace Shrinerain.AutoTester.Core
{
    public interface IClickable : IInteractive
    {
        void Click();
        void DoubleClick();
        void RightClick();
        void MiddleClick();
    }
}
