/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ExcelDataPool.cs
*
* Description: This class defines methods for EXCEL operation.
*
* History: 2007/09/04 wan,yu Init version.
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.OleDb;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;

namespace Shrinerain.AutoTester.DataUtility
{
    public class ExcelDataPool : TestDataPool
    {

    }

    public sealed class ExcelReader : IDisposable
    {
        #region fileds

        private string _fileName;
        private string _sheet;

        private OleDbConnection _conn;
        private OleDbCommand _comm;
        private OleDbDataReader _myReader;

        private int _rowNum = 0;

        #endregion

        #region ctor
        public ExcelReader(string fileName, string sheet)
        {
            if (!File.Exists(fileName))
            {
                throw new DriverFileNotFoundException("Excel: " + fileName + " not found.");
            }
            if (String.IsNullOrEmpty(sheet))
            {
                throw new BadFormatDriverFileException("Sheet: " + sheet + " not found.");
            }
            this._fileName = fileName;
            this._sheet = sheet;

        }
        public ExcelReader()
        {
        }

        public void Dispose()
        {
            Close();
        }
        #endregion

        #region Property
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _fileName = value;
                }

            }
        }
        public string Sheet
        {
            get { return _sheet; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _sheet = value;
                }
            }
        }

        public int ColCount
        {
            get { return this._myReader.FieldCount; }
        }

        public int CurrentRow
        {
            get { return _rowNum; }
        }

        #endregion

        #region public methods
        public void Open()
        {
            if (String.IsNullOrEmpty(this._fileName) || !File.Exists(this._fileName) || String.IsNullOrEmpty(this._sheet))
            {
                throw new DriverFileNotFoundException("Excel file not found: " + this._fileName);
            }

            try
            {
                string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + this._fileName + ";" + "Extended Properties=Excel 8.0;";
                _conn = new OleDbConnection(strConn);
                _conn.Open();
                string sql = "select * from [" + this._sheet + "$]";
                _comm = new OleDbCommand(sql, _conn);
                _myReader = _comm.ExecuteReader();

            }
            catch (Exception ex)
            {
                throw new CannotOpenDriverFileException("Can not open " + this._fileName + " " + ex.ToString());
            }

        }

        public void Close()
        {
            if (this._comm != null)
            {
                this._comm.Dispose();
                this._comm = null;
            }
            if (this._conn != null)
            {
                this._conn.Close();
                this._conn = null;
            }
        }

        public bool MoveNext()
        {
            this._rowNum++;
            return this._myReader.Read();
        }

        public string ReadByIndex(int index)
        {
            return this._myReader.GetValue(index).ToString().Trim();
        }
        #endregion
    }
}
