/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: IStatus.cs
*
* Description: This interface define the action of an object which will
*              change it's status at rumtime.
*              Like ListBox etc.
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


namespace Shrinerain.AutoTester.Core
{
    public interface IStatus
    {
        object GetCurrentStatus();

        //check if it is ready for testing.
        bool IsReady();
    }
}
