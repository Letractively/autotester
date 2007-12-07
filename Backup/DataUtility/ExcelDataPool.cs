using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.OleDb;
using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Function;

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
                throw new FileNotFoundException("Excel: " + fileName + " not found.");
            }
            if (String.IsNullOrEmpty(sheet))
            {
                throw new ArgumentNullException(sheet, "Sheet: " + sheet + " not found.");
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
                throw new FileNotFoundException("Excel file not found.");
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
            catch (Exception e)
            {
                throw new IOException("Can not open " + this._fileName + " " + e.Message);
            }

        }

        public void Close()
        {
            try
            {

                if (this._conn != null)
                {
                    this._comm.Dispose();
                    this._comm = null;
                    this._conn.Close();
                    this._conn = null;
                }

            }
            catch
            {
                throw;
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
