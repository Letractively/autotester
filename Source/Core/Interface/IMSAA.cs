/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: IMSAA.cs
*
* Description: This interface define the methods for MSAA object.
*              MSAA is short for Microsoft Active Accessbility. It is 
*              a technology to locate contorl and perform action on it. 
*              To use MSAA, you must use "oleacc.dll"
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/

namespace Shrinerain.AutoTester.Core
{
    using Accessibility;

    public interface IMSAA : IContainer
    {
        // this interface is a Windows Com interface.
        IAccessible GetIAccInterface();

        //self id is the CHILD_ID used in MSAA tech. 
        //0 means self, if >0, means the child.
        int GetSelfID();
    }
}
