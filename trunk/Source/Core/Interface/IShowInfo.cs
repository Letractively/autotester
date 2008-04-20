/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: IShowInfo.cs
*
* Description: This interface defines the actions of an object which
*              will show some information to the end-users.
*              Like TextBox etc.
*
* History:  2007/09/04 wan,yu Init version.
*           2008/03/05 wan,yu update, add GetFontSize() and GetFontColor()
*
*********************************************************************/


namespace Shrinerain.AutoTester.Core
{
    public interface IShowInfo : IVisible
    {
        //get the text on the object
        string GetText();

        //get the font family, like comic font.
        string GetFontFamily();

        //get the font size.
        string GetFontSize();

        //get the font color.
        string GetFontColor();
    }
}
