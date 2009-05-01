/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: IInputable.cs
*
* Description: This interface defines the actions of an object which 
*              you can input something to it.Like Button, Listbox.
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


namespace Shrinerain.AutoTester.Core
{
    public interface IInputable : IInteractive, IText
    {
        //input normal characters, like "asdf12345"
        void Input(string values);

        //input special keys, like "tab, ctrl"
        void InputKeys(string keys);

        //clear text.
        void Clear();
    }
}
