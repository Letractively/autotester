/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TestDataPool.cs
*
* Description: TestDataPool support database operation for testing.
*              It support some popular database.
*              TestDataPool provide some methods to get/store data from 
*              different database. 
*
* History:   2007/09/04 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public enum TestDataPoolType
    {
        TXT,
        CSV,
        XML,
        Excel,
        Access,
        SqlServer,
        Oracle,
        DB2,
        Sybase,
        MySql,
        Memory,
        Network,
        Other,
    }

    public class TestDataPool : IDisposable
    {
        #region fields

        protected string _connStr;

        protected int _index;
        protected int _rowCount;
        protected int _colCount;

        protected string _splitter;

        protected string _fileName;
        protected string _serverAddr;
        protected string _databaseName;
        protected string _tableName;
        protected string _userName;
        protected string _password;

        protected TestDataPoolType _databaseType;

        #endregion

        #region properties

        public string ConnStr
        {
            get { return this._connStr; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    this._connStr = value;
                }
            }
        }
        public int Index
        {
            get
            {
                return this._index;
            }
            set
            {
                if (value > 0)
                {
                    this._index = value;
                }
            }
        }
        public int RowCount
        {
            get
            {
                return this._rowCount;
            }
        }
        public int ColCount
        {
            get
            {
                return this._colCount;
            }
        }
        public string Splitter
        {
            get
            {
                return this._splitter;
            }
            set
            {
                if (!String.IsNullOrEmpty(Splitter))
                {
                    this._splitter = value;
                }
                else
                {

                }
            }
        }

        public TestDataPoolType DatabaseType
        {
            get { return _databaseType; }
            set { _databaseType = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; }
        }

        public string ServerAddr
        {
            get { return _serverAddr; }
            set { _serverAddr = value; }
        }

        #endregion

        #region public methods

        public TestDataPool()
        {

        }

        public TestDataPool(TestDataPoolType type)
        {
            this._databaseType = type;
        }

        ~TestDataPool()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            throw new Exception();
        }

        public virtual void Open()
        {
            throw new Exception();
        }

        public virtual void Close()
        {
            throw new Exception();
        }

        public virtual bool MoveNext()
        {
            throw new Exception();
        }

        public virtual string GetValueByIndex(int index)
        {
            throw new Exception();
        }

        public virtual string[] GetValueByRow(int rowNumber)
        {
            throw new Exception();
        }

        public virtual string[] GetValueByCol(int colNumber)
        {
            throw new Exception();
        }

        public virtual string[][] GetAllValue()
        {
            throw new Exception();
        }

        #endregion

        #region private methods

        protected virtual string BuildConnStr(TestDataPoolType type)
        {
            switch (type)
            {
                case TestDataPoolType.TXT:
                    return "";
                case TestDataPoolType.CSV:
                    return "";
                case TestDataPoolType.XML:
                    return "";
                case TestDataPoolType.Excel:
                    return "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + this._fileName + ";" + "Extended Properties=Excel 8.0;";
                case TestDataPoolType.Access:
                    return "";
                case TestDataPoolType.Oracle:
                    return "";
                case TestDataPoolType.Sybase:
                    return "";
                case TestDataPoolType.DB2:
                    return "";
                case TestDataPoolType.MySql:
                    return "";
                case TestDataPoolType.SqlServer:
                    return "";
                case TestDataPoolType.Memory:
                    return "";
                case TestDataPoolType.Network:
                    return "";
                case TestDataPoolType.Other:
                    return "";

                default:
                    return null;
            }

        }

        #endregion
    }
}
