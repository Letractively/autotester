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
using System.Text;
using System.Threading;
using Accessibility;

using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.Helper
{
    public sealed class Printer
    {

        #region fields

        //MSAA interface for printer list.
        static IAccessible _paccPrintList;
        //MSAA interface for copy count edit box.
        static IAccessible _paccCopyCount;

        //handle for each control
        static IntPtr _printDialogHandle;
        static IntPtr _printBtnHandle;
        static IntPtr _cancelBtnHandle;
        static IntPtr _copyCountHandle;
        static IntPtr _printerListHandle;

        static int _maxWaitSeconds = 30;

        //CHILDID_SELF constant for MSAA.
        static object _selfID = 0;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        //no instance is allowed.
        private Printer()
        {
        }

        #endregion

        #region public methods

        /*  bool GetActivePrintDialog()
         * return true if we found the dialog , else return false.
         */
        public static bool GetActivePrintDialog()
        {
            GetAllHandles();

            //if the dialog handle is 0, means we didn't find it.
            return _printDialogHandle != IntPtr.Zero && _paccPrintList != null;
        }

        /* void PressPrint()
         * click "Print" button
         */
        public static void PressPrint()
        {
            bool lastStatus = MouseOp.SendMessageOnly;
            MouseOp.SendMessageOnly = true;

            MouseOp.Click(_printBtnHandle);

            MouseOp.SendMessageOnly = lastStatus;
        }

        /*  void PressCancel()
         *  click "Cancel" button
         */
        public static void PressCancel()
        {
            bool lastStatus = MouseOp.SendMessageOnly;
            MouseOp.SendMessageOnly = true;

            MouseOp.Click(_cancelBtnHandle);

            MouseOp.SendMessageOnly = lastStatus;
        }

        /* String[] GetPrinterList()
         * get the printers can be selected in the dialog.
         */
        public static String[] GetPrinterList()
        {
            if (_paccPrintList != null)
            {
                try
                {
                    long childrenCount = _paccPrintList.accChildCount;

                    String[] res = new string[childrenCount - 1];

                    //the first is "Add Printer", it is not an actual printer, so ignore it.
                    //start from the 2nd printer.
                    for (int i = 2; i <= childrenCount; i++)
                    {
                        object childID = (object)i;

                        //get name for each printer, add it to the array.
                        res[i - 2] = _paccPrintList.get_accName(childID);
                    }

                    return res;
                }
                catch
                {
                }
            }

            return null;
        }

        /* ChoosePrinter(String name)
         * choose the printer by name.
         * return true if we found the printer, else return false.
         */
        public static bool ChoosePrinter(String name)
        {
            if (_paccPrintList != null && name != null)
            {
                int childrenCount = _paccPrintList.accChildCount;
                object childID;

                for (int i = 1; i <= childrenCount; i++)
                {
                    childID = i;
                    string curPrinterName = _paccPrintList.get_accName(childID);

                    //name is the same, found it.
                    if (String.Compare(name.Trim(), curPrinterName.Trim(), true) == 0)
                    {
                        //select this printer.
                        _paccPrintList.accSelect(2, childID);

                        return true;
                    }
                }
            }

            return false;
        }

        /* int GetCopyCount()
         * get how many copies need to print currently
         */
        public static int GetCopyCount()
        {
            if (_copyCountHandle != IntPtr.Zero)
            {
                try
                {
                    //get MSAA interface for copy count.
                    Win32API.AccessibleObjectFromWindow(_copyCountHandle, (uint)Win32API.IACC.OBJID_CLIENT, Win32API.IACCUID, ref _paccCopyCount);

                    if (_paccCopyCount != null)
                    {
                        string countStr = _paccCopyCount.get_accValue(_selfID);

                        //if string is not null, convert to int, else return -1.
                        return countStr == null ? -1 : int.Parse(countStr);
                    }
                }
                catch
                {
                    return -1;
                }

            }

            return -1;
        }

        /* void SetCopyCount(int number)
         * Set how many copies we need to print.
         * return true is success, else return false.
         */
        public static bool SetCopyCount(int number)
        {
            if (_copyCountHandle != IntPtr.Zero && number >= 0)
            {
                try
                {
                    //get MSAA interface for copy count.
                    Win32API.AccessibleObjectFromWindow(_copyCountHandle, (uint)Win32API.IACC.OBJID_CLIENT, Win32API.IACCUID, ref _paccCopyCount);

                    if (_paccCopyCount != null)
                    {
                        _paccCopyCount.set_accValue(_selfID, number.ToString());
                        return true;
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        #endregion

        #region private methods

        /* void GetAllHandles()
         * Get handles for each control
         */
        private static void GetAllHandles()
        {
            int times = 0;

            //try to find the dialog in 30s.
            while (times < _maxWaitSeconds)
            {

                _printDialogHandle = Win32API.FindWindow(null, "Print");

                if (_printDialogHandle != IntPtr.Zero)
                {
                    //find "Print" and "Cancel" button.
                    _printBtnHandle = Win32API.FindWindowEx(_printDialogHandle, IntPtr.Zero, null, "&Print");
                    _cancelBtnHandle = Win32API.FindWindowEx(_printDialogHandle, IntPtr.Zero, null, "Cancel");

                    IntPtr generalHandle = Win32API.FindWindowEx(_printDialogHandle, IntPtr.Zero, null, "General");

                    //find printer list handle.
                    IntPtr shellDefHandle = Win32API.FindWindowEx(generalHandle, IntPtr.Zero, "SHELLDLL_DefView", null);
                    _printerListHandle = Win32API.FindWindowEx(shellDefHandle, IntPtr.Zero, "SysListView32", null);

                    //find copy count handle.
                    IntPtr findPrinterHandle = Win32API.FindWindowEx(generalHandle, IntPtr.Zero, null, "Fin&d Printer...");
                    IntPtr tmpHandle = Win32API.FindWindowEx(generalHandle, findPrinterHandle, null, "");
                    IntPtr numEditHandle = Win32API.FindWindowEx(tmpHandle, IntPtr.Zero, null, "Number of &copies:");
                    _copyCountHandle = Win32API.FindWindowEx(tmpHandle, numEditHandle, "Edit", null);

                    //get msaa interface for printer list.
                    _paccPrintList = GetIAccPrinterList();

                    return;

                }
                else
                {
                    //sleep for 1 seconds, then try again.
                    Thread.Sleep(1000 * 1);
                    times++;
                }
            }
        }

        /* IAccessible GetIAccPrinterList()
         * get the MSAA interface for printer list.
         */
        private static IAccessible GetIAccPrinterList()
        {
            if (_printerListHandle != IntPtr.Zero)
            {
                Win32API.AccessibleObjectFromWindow(_printerListHandle, (uint)Win32API.IACC.OBJID_CLIENT, Win32API.IACCUID, ref _paccPrintList);
                return _paccPrintList;
            }

            return null;
        }
        #endregion

        #endregion

    }
}
