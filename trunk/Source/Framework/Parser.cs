/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: Parser.cs
*
* Description: Parser will parse the EXCEL driver file. Get the test steps,
*              Subs and Datapools. 
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.IO;
using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.DataUtility;

namespace Shrinerain.AutoTester.Framework
{
    //test step, each test step is a single line in EXCEL.
    public struct TestStep
    {

        // test step mapped from excel drive file.
        public string _testCommand;
        public string _testControl;
        public string _testProperty;
        public string _testAction;
        public string _testData;
        public string _testVPProperty;
        public string _testExpectResult;

        //string for indexer
        private string _strBuf;

        //the number of fields, this is used for "for" and "foreach".
        public int Size
        {
            get { return 7; }
        }


        public override string ToString()
        {

            if (String.IsNullOrEmpty(_strBuf))
            {
                UpdateBuffer();
            }

            return _strBuf;

        }

        //used for "for" and "foreach"
        public string this[int index]
        {
            get
            {
                if (index >= 0 && index < Size)
                {
                    return this.ToString().Split('\t')[index];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        _testCommand = value;
                        break;
                    case 1:
                        _testControl = value;
                        break;
                    case 2:
                        _testProperty = value;
                        break;
                    case 3:
                        _testAction = value;
                        break;
                    case 4:
                        _testData = value;
                        break;
                    case 5:
                        _testVPProperty = value;
                        break;
                    case 6:
                        _testExpectResult = value;
                        break;
                    default:
                        break;
                }

                UpdateBuffer();

            }

        }

        public override bool Equals(object obj)
        {
            if (obj is TestStep)
            {
                return ((TestStep)obj)._strBuf == this._strBuf;
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==(TestStep ts1, TestStep ts2)
        {
            return ts1.Equals(ts2);
        }

        public static bool operator !=(TestStep ts1, TestStep ts2)
        {
            return !ts1.Equals(ts2);
        }

        public override int GetHashCode()
        {
            return this._strBuf.GetHashCode();
        }

        private void UpdateBuffer()
        {
            StringBuilder tmp = new StringBuilder();

            tmp.Append(_testCommand);
            tmp.Append("\t");
            tmp.Append(_testControl);
            tmp.Append("\t");
            tmp.Append(_testProperty);
            tmp.Append("\t");
            tmp.Append(_testAction);
            tmp.Append("\t");
            tmp.Append(_testData);
            tmp.Append("\t");
            tmp.Append(_testVPProperty);
            tmp.Append("\t");
            tmp.Append(_testExpectResult);

            _strBuf = tmp.ToString();
        }

    };

    //struct of test sub, we have a sub name, and test steps
    public struct TestSub
    {
        public string _subName;
        public List<TestStep> _subTestSteps;
    };

    //struct of test data pool, we have a datapool name, and data.
    public struct TestDataPool
    {
        public string _datapoolName;
        public List<string[]> _data;
    };


    public sealed class Parser : IDisposable
    {
        #region fileds

        private static Parser _parser = new Parser();

        private static AutoConfig _autoConfig;
        private ExcelReader _excelReader;

        private List<TestStep> _myTestStepList = new List<TestStep>(256);
        private List<TestSub> _myTestSubList = new List<TestSub>(10);
        private List<TestDataPool> _myTestDataPoolList = new List<TestDataPool>(10);

        private string _driveFile;

        //the max data item in datapool, that means in one line, we can not have more than 50 data.
        private const int _maxDataCount = 50;


        #endregion

        #region properties

        public AutoConfig AutoConfig
        {
            get { return _autoConfig; }
            set
            {
                if (value != null)
                {
                    _autoConfig = value;
                }
            }
        }

        public static int MaxDataCount
        {
            get { return _maxDataCount; }
        }


        #endregion

        #region methods

        #region ctor

        private Parser()
        {

        }

        private Parser(AutoConfig config)
        {
            _autoConfig = config;
        }

        ~Parser()
        {
            Dispose();
        }

        #endregion


        #region public methods

        public static Parser GetInstance()
        {
            return _parser;
        }

        public static Parser GetInstance(AutoConfig autoConfig)
        {
            if (autoConfig == null)
            {
                throw new CannotLoadConfigException("AutoConfig can not be null.");
            }

            _autoConfig = autoConfig;

            return _parser;
        }

        public void Close()
        {
            Dispose();
        }

        /* void Dispose()
         * when GC, close excel reader.
         */
        public void Dispose()
        {
            if (_excelReader != null)
            {
                _excelReader.Close();
                _excelReader = null;
            }

            GC.SuppressFinalize(this);
        }

        /* void ParseDriveFile()
         * Parse EXCEl driver file.
         * call three methods to prase different tab in EXCEL.
         */
        public void ParseDriveFile()
        {
            if (_autoConfig != null)
            {
                this._driveFile = _autoConfig.ProjectDriveFile;
            }

            _excelReader = new ExcelReader();

            GetTestSteps(this._driveFile); //get main test steps
            GetSubSteps(this._driveFile);  //get sub steps
            GetDataPool(this._driveFile);  //get data pool
        }

        /* return the list of each EXCEL tab.
         * 
         */
        public List<TestStep> GetAllMainTestSteps()
        {
            if (this._myTestStepList == null || this._myTestStepList.Count == 0)
            {
                throw new CannotLoadTestStepsException();
            }

            return this._myTestStepList;
        }

        public List<TestSub> GetAllTestSubs()
        {
            if (this._myTestSubList == null || this._myTestSubList.Count == 0)
            {
                throw new CannotLoadSubException();
            }

            return this._myTestSubList;
        }

        public List<TestDataPool> GetAllTestDataPool()
        {
            if (this._myTestDataPoolList == null || this._myTestDataPoolList.Count == 0)
            {
                throw new CannotLoadDataPoolException();
            }

            return this._myTestDataPoolList;
        }

        #endregion


        #region private help methods

        /* void GetTestSteps(string DriverFile)
         * Parse main test steps, the first tab in EXCEl, store test steps in a list.
         */
        private void GetTestSteps(string DriverFile)
        {
            if (String.IsNullOrEmpty(DriverFile) || !File.Exists(this._driveFile))
            {
                throw new DriveNotFoundException("Can not find driven file:" + DriverFile);
            }

            try
            {
                _excelReader.FileName = DriverFile;
                _excelReader.Sheet = "TestSteps";
                _excelReader.Open();

                //read execl line by line
                while (_excelReader.MoveNext())
                {
                    //each line is a test step
                    TestStep tmp = new TestStep();
                    tmp._testCommand = _excelReader.ReadByIndex(0);
                    tmp._testControl = _excelReader.ReadByIndex(1);
                    tmp._testProperty = _excelReader.ReadByIndex(2);
                    tmp._testAction = _excelReader.ReadByIndex(3);
                    tmp._testData = _excelReader.ReadByIndex(4);
                    tmp._testVPProperty = _excelReader.ReadByIndex(5);
                    tmp._testExpectResult = _excelReader.ReadByIndex(6);

                    //store each test step to a list.
                    this._myTestStepList.Add(tmp);

                    //meet "END", stop.
                    if (tmp._testCommand.ToUpper() == "END")
                    {
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                throw new BadFormatDriverFileException("Can not get main test steps: " + ex.Message);
            }
            finally
            {
                _excelReader.Close();
            }
        }

        /* void GetSubSteps(string DriverFile)
         * Parse the 2nd "Sub" tab, store subs to a list.
         */
        private void GetSubSteps(string DriverFile)
        {
            if (String.IsNullOrEmpty(DriverFile) || !File.Exists(this._driveFile))
            {
                throw new DriveNotFoundException("Can not find driven file:" + DriverFile);
            }

            try
            {
                _excelReader.FileName = DriverFile;
                _excelReader.Sheet = "Sub";
                _excelReader.Open();

                TestSub tmpSub = new TestSub();

                //line by line
                while (_excelReader.MoveNext())
                {
                    if (String.IsNullOrEmpty(_excelReader.ReadByIndex(0)))
                    {
                        continue;
                    }

                    //if the first column is "SUB", means we find a new sub.
                    if (_excelReader.ReadByIndex(0).ToUpper() == "SUB")
                    {
                        tmpSub._subName = null;
                        tmpSub._subTestSteps = new List<TestStep>(128);

                        if (String.IsNullOrEmpty(_excelReader.ReadByIndex(1)))
                        {
                            throw new BadFormatDriverFileException("Sub must contain sub name.");
                        }
                        else
                        {
                            tmpSub._subName = _excelReader.ReadByIndex(1);
                        }

                        continue;
                    }

                    //if the first column is "EndSub" or "End Sub", means current sub is end.
                    if (_excelReader.ReadByIndex(0).ToUpper() == "ENDSUB" || _excelReader.ReadByIndex(0).ToUpper() == "END SUB")
                    {
                        _myTestSubList.Add(tmpSub);
                        continue;
                    }

                    //test steps of this sub.
                    TestStep tmp = new TestStep();
                    tmp._testCommand = _excelReader.ReadByIndex(0);
                    tmp._testControl = _excelReader.ReadByIndex(1);
                    tmp._testProperty = _excelReader.ReadByIndex(2);
                    tmp._testAction = _excelReader.ReadByIndex(3);
                    tmp._testData = _excelReader.ReadByIndex(4);
                    tmp._testVPProperty = _excelReader.ReadByIndex(5);
                    tmp._testExpectResult = _excelReader.ReadByIndex(6);

                    if (tmpSub._subTestSteps == null)
                    {
                        tmpSub._subTestSteps = new List<TestStep>(128);
                    }

                    tmpSub._subTestSteps.Add(tmp);
                }

            }
            catch (Exception ex)
            {
                throw new BadFormatDriverFileException("Can not get test subs: " + ex.Message);
            }
            finally
            {
                _excelReader.Close();
            }

        }

        /* void GetDataPool(string DriverFile)
         * Parse the 3rd tab, datapool, store datapool to a list.
         */
        private void GetDataPool(string DriverFile)
        {
            if (String.IsNullOrEmpty(DriverFile) || !File.Exists(this._driveFile))
            {
                throw new DriveNotFoundException("Can not find driven file:" + DriverFile);
            }
            try
            {
                _excelReader.FileName = DriverFile;
                _excelReader.Sheet = "Data";
                _excelReader.Open();


                TestDataPool tmpPool = new TestDataPool();
                bool started = false;

                while (_excelReader.MoveNext())
                {

                    string dataPoolName = _excelReader.ReadByIndex(0);

                    //if the first column is not empty, means we meet a new datapool
                    if (!String.IsNullOrEmpty(dataPoolName) && !started)
                    {
                        started = true;
                        tmpPool._datapoolName = dataPoolName;
                        tmpPool._data = new List<string[]>(32);

                        continue;
                    }


                    //if the frist column is "END", means current datapool is end.
                    if (!String.IsNullOrEmpty(dataPoolName) && started && dataPoolName.ToUpper().IndexOf("END") == 0)
                    {
                        _myTestDataPoolList.Add(tmpPool);
                        started = false;
                    }


                    if (started)
                    {
                        //get the actual test data.
                        string[] currentDataLine = new string[MaxDataCount];
                        for (int i = 0; i < MaxDataCount && i < _excelReader.ColCount - 1; i++)
                        {
                            currentDataLine[i] = _excelReader.ReadByIndex(i + 1);
                        }
                        tmpPool._data.Add(currentDataLine);
                    }


                }

            }
            catch (Exception ex)
            {
                throw new BadFormatDriverFileException("Can not get test data pool: " + ex.Message);
            }
            finally
            {
                _excelReader.Close();
            }

        }
        #endregion

        #endregion

    }
}
