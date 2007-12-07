using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.IO;
using Shrinerain.AutoTester.Function;
using Shrinerain.AutoTester.DataUtility;

namespace Shrinerain.AutoTester.Framework
{

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

        //the number of fields
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

    public struct TestSub
    {
        public string _subName;
        public List<TestStep> _subTestSteps;
    };

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

        //private string[] _testSteps;
        //private string[] _testSubs;
        //private string[] _testDataPool;

        //private int _currentStep;
        //private int _currentSubStep;
        //private int _currentDataPoolStep;

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

        public int MaxDataCount
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
                throw new CanNotLoadAutoConfigException("AutoConfig can not be null.");
            }

            _autoConfig = autoConfig;

            return _parser;
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_excelReader != null)
            {
                _excelReader.Close();
                _excelReader = null;
            }

            GC.SuppressFinalize(this);
        }

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

        public List<TestStep> GetAllMainTestSteps()
        {
            if (this._myTestStepList == null || this._myTestStepList.Count == 0)
            {
                throw new CanNotLoadTestStepsException();
            }

            return this._myTestStepList;
        }

        public List<TestSub> GetAllTestSubs()
        {
            if (this._myTestSubList == null || this._myTestSubList.Count == 0)
            {
                throw new CanNotLoadSubException();
            }

            return this._myTestSubList;
        }

        public List<TestDataPool> GetAllTestDataPool()
        {
            if (this._myTestDataPoolList == null || this._myTestDataPoolList.Count == 0)
            {
                throw new CanNotLoadDataPoolException();
            }

            return this._myTestDataPoolList;
        }

        #endregion


        #region private help methods
        private void GetTestSteps(string drivenFile)
        {
            if (String.IsNullOrEmpty(drivenFile) || !File.Exists(this._driveFile))
            {
                throw new DriveNotFoundException("Can not find driven file:" + drivenFile);
            }

            try
            {
                _excelReader.FileName = drivenFile;
                _excelReader.Sheet = "TestSteps";
                _excelReader.Open();

                while (_excelReader.MoveNext())
                {
                    TestStep tmp = new TestStep();
                    tmp._testCommand = _excelReader.ReadByIndex(0);
                    tmp._testControl = _excelReader.ReadByIndex(1);
                    tmp._testProperty = _excelReader.ReadByIndex(2);
                    tmp._testAction = _excelReader.ReadByIndex(3);
                    tmp._testData = _excelReader.ReadByIndex(4);
                    tmp._testVPProperty = _excelReader.ReadByIndex(5);
                    tmp._testExpectResult = _excelReader.ReadByIndex(6);

                    this._myTestStepList.Add(tmp);

                    if (tmp._testCommand.ToUpper() == "END")
                    {
                        break;
                    }
                }

            }
            catch (Exception e)
            {
                throw new BadFormatDrivenFileException("Can not parse driven file:" + drivenFile + " " + e.Message);
            }
            finally
            {
                _excelReader.Close();
            }
        }

        private void GetSubSteps(string drivenFile)
        {
            if (String.IsNullOrEmpty(drivenFile) || !File.Exists(this._driveFile))
            {
                throw new DriveNotFoundException("Can not find driven file:" + drivenFile);
            }

            try
            {
                _excelReader.FileName = drivenFile;
                _excelReader.Sheet = "Sub";
                _excelReader.Open();

                TestSub tmpSub = new TestSub();

                while (_excelReader.MoveNext())
                {
                    if (String.IsNullOrEmpty(_excelReader.ReadByIndex(0)))
                    {
                        continue;
                    }

                    if (_excelReader.ReadByIndex(0).ToUpper() == "SUB")
                    {
                        tmpSub._subName = null;
                        tmpSub._subTestSteps = new List<TestStep>(128);

                        if (String.IsNullOrEmpty(_excelReader.ReadByIndex(1)))
                        {
                            throw new BadFormatDrivenFileException("Sub must contain sub name.");
                        }
                        else
                        {
                            tmpSub._subName = _excelReader.ReadByIndex(1);
                        }

                        continue;
                    }

                    if (_excelReader.ReadByIndex(0).ToUpper() == "ENDSUB" || _excelReader.ReadByIndex(0).ToUpper() == "END SUB")
                    {
                        _myTestSubList.Add(tmpSub);
                        continue;
                    }

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
            catch (BadFormatDrivenFileException)
            {
                throw;
            }
            catch
            {
                throw new BadFormatDrivenFileException("Can not parse driven file:" + drivenFile);
            }
            finally
            {
                _excelReader.Close();
            }

        }

        private void GetDataPool(string drivenFile)
        {
            if (String.IsNullOrEmpty(drivenFile) || !File.Exists(this._driveFile))
            {
                throw new DriveNotFoundException("Can not find driven file:" + drivenFile);
            }
            try
            {
                _excelReader.FileName = drivenFile;
                _excelReader.Sheet = "Data";
                _excelReader.Open();


                TestDataPool tmpPool = new TestDataPool();
                bool started = false;

                while (_excelReader.MoveNext())
                {

                    string dataPoolName = _excelReader.ReadByIndex(0);

                    if (!String.IsNullOrEmpty(dataPoolName) && !started)
                    {
                        started = true;
                        tmpPool._datapoolName = dataPoolName;
                        tmpPool._data = new List<string[]>(128);

                        continue;
                    }

                    if (!String.IsNullOrEmpty(dataPoolName) && started && dataPoolName.ToUpper().IndexOf("END") == 0)
                    {
                        _myTestDataPoolList.Add(tmpPool);
                        started = false;
                    }

                    if (started)
                    {
                        string[] currentDataLine = new string[MaxDataCount];
                        for (int i = 0; i < MaxDataCount && i < _excelReader.ColCount - 1; i++)
                        {
                            currentDataLine[i] = _excelReader.ReadByIndex(i + 1);
                        }
                        tmpPool._data.Add(currentDataLine);
                    }


                }

            }
            catch
            {
                throw new BadFormatDrivenFileException("Can not parse driven file:" + drivenFile);
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
