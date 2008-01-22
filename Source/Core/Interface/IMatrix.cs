/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: IMatrix.cs
*
* Description: This interface defines the actions of an object which
 *             is Matirx_like, eg: table
*
* History:  2008/01/21 wan,yu Init version.
*           2008/01/22 wan,yu update, add GetTextByCell.
*
*********************************************************************/

using System;

namespace Shrinerain.AutoTester.Core
{
    public interface IMatrix : IInteractive, IContainer, IShowInfo
    {
        //get total row or col count.
        int GetRowCount(int col);
        int GetColCount(int row);

        Object[] GetObjectsByCell(int row, int col);

        //return the text in the cell.
        string GetTextByCell(int row, int col);
    }
}
