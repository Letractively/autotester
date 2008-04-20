/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: IContainer.cs
*
* Description: This interface defines the actions of a container object.
*              Container means this object can have child object.
*              Like Listbox, Table etc. 
*
* History:  2007/09/04 wan,yu Init version
*
*********************************************************************/



namespace Shrinerain.AutoTester.Core
{
    public interface IContainer
    {
        object[] GetChildren();
    }
}
