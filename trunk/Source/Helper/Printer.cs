/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: Printer.cs
*
* Description: This class control the printer dialog, we can choose which printer to print
*              how many copies.
*
* History: 2008/03/09 wan,yu Init version.
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Accessibility;

namespace Shrinerain.AutoTester.Helper
{
    public sealed class Printer
    {

        #region fields

        //MSAA interface for printer list.
        static IAccessible _paccPrintList;

        //handle for each control
        static IntPtr _printDialogHandle;
        static IntPtr _printBtnHandle;
        static IntPtr _cancelBtnHandle;
        static IntPtr _copyCountHandle;
        static IntPtr _printerListHandle;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        #endregion

        #region public methods

        static void GetActivePrintDialog()
        {

        }

        static void PressPrint()
        {

        }

        static void PressCancel()
        {

        }

        static String[] GetPrinterList()
        {
            return null;
        }

        static void ChoosePrinter(String name)
        {

        }

        static int GetCopyCount()
        {
            return -1;
        }

        static void SetCopyCount(int number)
        {

        }

        #endregion

        #region private methods


        #endregion

        #endregion

    }
}
