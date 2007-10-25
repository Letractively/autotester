using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Shrinerain.AutoTester.Function;

namespace Shrinerain.AutoTester.Framework
{
    public sealed class SubEngine
    {

        #region fields

        private static SubEngine _subEngine = new SubEngine();

        private static Parser _parser = Parser.GetInstance();
        private DataEngine _dataEngine = null;

        private List<TestSub> _allTestSubs;


        private Regex _dataPoolReg = new Regex(@"^{(\w\.)?value\d+}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex _numberReg = new Regex(@"\d+", RegexOptions.Compiled);

        #endregion

        #region properties

        public Parser Parser
        {
            get { return _parser; }
            set
            {
                if (value != null)
                {
                    _parser = value;
                }
            }
        }

        public DataEngine DataEngine
        {
            get { return _dataEngine; }
            set
            {
                if (value != null)
                {
                    _dataEngine = value;
                }
            }
        }

        #endregion

        #region methods

        #region ctor

        public SubEngine()
        {

        }

        #endregion

        #region public methods

        //singleton

        public static SubEngine GetInstance()
        {
            return _subEngine;
        }

        public static SubEngine GetInstance(Parser parser)
        {
            if (parser == null)
            {
                throw new CanNotLoadSubException("Parser can not be null.");
            }
            _parser = parser;

            return _subEngine;
        }

        public List<TestStep> BuildTestStepBySubName(string subName)
        {
            List<TestStep> subOriginSteps = GetTestSubByName(subName);

            string dataPoolName = GetDataPoolName(subOriginSteps);

            if (String.IsNullOrEmpty(dataPoolName))
            {
                return subOriginSteps;
            }
            else
            {
                return InsertDataValueToSub(subOriginSteps, dataPoolName);
            }

        }

        #endregion

        #region private methods

        private List<TestStep> GetTestSubByName(string subName)
        {
            if (_allTestSubs == null)
            {
                if (_parser == null)
                {
                    throw new CanNotLoadSubException("Parser can not be null.");
                }
                _allTestSubs = _parser.GetAllTestSubs();
            }

            if (String.IsNullOrEmpty(subName))
            {
                throw new CanNotLoadSubException("Sub name can not be empty.");
            }

            foreach (TestSub sub in _allTestSubs)
            {
                if (sub._subName.ToUpper() == subName.ToUpper())
                {
                    return sub._subTestSteps;
                }
            }

            throw new CanNotLoadSubException("Sub:" + subName + " not found.");
        }

        private string GetDataPoolName(List<TestStep> stepList)
        {
            if (stepList == null)
            {
                throw new CanNotLoadSubException("Sub step list can not be null.");
            }

            string dataPoolName;

            foreach (TestStep step in stepList)
            {
                for (int i = 0; i < step.Size; i++)
                {
                    if (GetDataPoolNameByReg(step[i], out dataPoolName))
                    {
                        return dataPoolName;
                    }
                }
            }

            return null;
        }

        private bool GetDataPoolNameByReg(string value, out string dataPoolName)
        {
            dataPoolName = null;

            if (String.IsNullOrEmpty(value))
            {
                return false;
            }

            if (_dataPoolReg.IsMatch(value))
            {
                string str1 = _dataPoolReg.Match(value).Value;
                try
                {
                    str1 = str1.Replace(" ", "");
                    str1 = str1.Replace("\t", "");
                    int dotPos = str1.IndexOf('.');
                    if (dotPos > 1)
                    {
                        dataPoolName = str1.Substring(1, dotPos - 1);
                        return true;
                    }
                }
                catch
                {
                    return false;
                }

            }
            return false;
        }

        private bool GetDataPoolDataIndex(string value, out int index)
        {
            index = -1;

            if (String.IsNullOrEmpty(value))
            {
                return false;
            }

            if (_dataPoolReg.IsMatch(value))
            {
                string str1 = _dataPoolReg.Match(value).Value;
                try
                {
                    str1 = str1.Replace(" ", "");
                    str1 = str1.Replace("\t", "");
                    int dotPos = str1.IndexOf('.');
                    if (dotPos > 1)
                    {
                        string valueX = str1.Substring(dotPos, str1.Length - dotPos);

                        if (_numberReg.IsMatch(valueX))
                        {
                            if (int.TryParse(_numberReg.Match(valueX).Value, out index))
                            {
                                if (index > 0 && index < _parser.MaxDataCount)
                                {
                                    return true;
                                }
                            }
                        }

                    }
                    else
                    {
                        if (_numberReg.IsMatch(str1))
                        {
                            if (int.TryParse(_numberReg.Match(str1).Value, out index))
                            {
                                if (index > 0 && index < _parser.MaxDataCount)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    return false;
                }

            }

            return false;
        }

        private List<TestStep> InsertDataValueToSub(List<TestStep> originSteps, string dataPoolName)
        {
            if (originSteps == null || String.IsNullOrEmpty(dataPoolName))
            {
                throw new CanNotLoadSubException("Sub steps and/or Data pool name can not be null.");
            }

            TestDataPool dataPool;

            try
            {
                if (this._dataEngine == null)
                {
                    this._dataEngine = DataEngine.GetInstance();
                }

                dataPool = this._dataEngine.GetDataPoolByName(dataPoolName);
            }
            catch
            {
                throw new CanNotLoadSubException("Can not load data pool by name:" + dataPoolName);
            }

            List<string[]> datas = dataPool._data;

            foreach (string[] sArr in datas)
            {
                foreach (TestStep t in originSteps)
                {
                    TestStep newStep = InsertDataToSingleTestStep(t, sArr);

                    originSteps.Remove(t);
                    originSteps.Add(newStep);
                }
            }

            originSteps.Reverse();

            return originSteps; ;
        }


        private TestStep InsertDataToSingleTestStep(TestStep testStep, string[] data)
        {

            return new TestStep();
        }

        #endregion

        #endregion


    }
}
