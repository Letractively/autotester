/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: AccessDataPool.cs
*
* Description: This class provide data operation for Microsoft Access.
*
* History: 2008/04/24 wan,yu Init verion.
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace DataUtility
{
    class AccessDataPool
    {
    }

    class AccessReader : IDisposable
    {
        private String _dbFile;
        private String _tblName;
        private String _sql;
        private const String _oledbStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
        private String _connStr = _oledbStr;
        private OleDbConnection _myConn = null;
        private OleDbCommand _myCmd = null;
        private OleDbDataReader _myDataReader = null;
        public String DbFile
        {
            get { return _dbFile; }
            set
            {
                _dbFile = value;
                _connStr = _oledbStr + value;
            }
        }
        public String TableName
        {
            get { return _tblName; }
            set
            {
                _tblName = value;
                _sql = "Select * from " + _tblName;
            }
        }
        public String Sql
        {
            get { return _sql; }
            set { _sql = value; }
        }
        public AccessReader()
        {
        }
        public bool Open()
        {
            if (String.IsNullOrEmpty(_dbFile))
            {
                throw new Exception("Access file not found.");
            }
            try
            {
                _myConn = new OleDbConnection(_connStr);
                _myCmd = new OleDbCommand();
                _myCmd.Connection = _myConn;
                _myConn.Open();
                return true;
            }
            catch
            {
                Close();
                return false;
            }
        }
        public int Execute(String sql)
        {
            if (_myConn != null && !String.IsNullOrEmpty(sql))
            {
                _myCmd.CommandText = sql;
                return _myCmd.ExecuteNonQuery();
            }
            return -1;
        }
        public bool ReadNext()
        {
            if (_myDataReader == null)
            {
                _myCmd.CommandText = _sql;
                _myDataReader = _myCmd.ExecuteReader();
            }
            return _myDataReader.Read();
        }
        public String Field(int index)
        {
            try
            {
                return _myDataReader[index].ToString();
            }
            catch
            {
                return null;
            }
        }
        public String Field(String column)
        {
            try
            {
                return _myDataReader[column].ToString();
            }
            catch
            {
                return null;
            }
        }
        public void Close()
        {
            if (_myConn != null)
            {
                try
                {
                    _myDataReader = null;
                    _myCmd = null;
                    _myConn.Close();
                    _myConn = null;
                }
                catch
                {
                }
            }
        }
        #region IDisposable Members
        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

