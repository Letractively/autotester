/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: IPath.cs
*
* Description: This interface defines the actions of an object which
*              is Tree-Like, eg: Tree and Menu.
*
* History: 2008/01/21 wan,yu Init version
*
*********************************************************************/

using System;

namespace Shrinerain.AutoTester.Core
{
    public interface IPath : IInteractive, IContainer
    {
        int GetDepth();

        //get objects at expected path, eg: "File->New->Project".
        Object[] GetObjectsAtPath(String path);
    }
}
