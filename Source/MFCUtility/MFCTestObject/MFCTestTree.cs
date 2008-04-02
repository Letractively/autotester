/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: MFCTestTree.cs
*
* Description: This class defines actions provide by TreeView.
*
* History: 2008/01/14 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.MFCUtility
{
    public class MFCTestTree : MFCTestGUIObject, IPath
    {

        #region fields


        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        #endregion

        #region public methods

        #region IPath Members

        public virtual int GetDepth()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual object[] GetObjectsAtPath(string path)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IInteractive Members

        public virtual void Focus()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual string GetAction()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void DoAction(object parameter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IContainer Members

        public virtual object[] GetChildren()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #endregion

        #region private methods


        #endregion

        #endregion


    }
}
