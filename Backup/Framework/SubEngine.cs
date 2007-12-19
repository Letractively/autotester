/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: SubEngine.cs
*
* Description: SubEngine get subs from the 2nd tab in EXCEL driver file,
*              Load data pool if needed, return test steps to CoreEngine. 
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


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

        private DataEngine _dataEngine;

        //Load all test sub to this list.
        private List<TestSub> _allTestSubs;

        //regular expression to extract datapool name. eg: data1.value1
        private Regex _dataPoolReg = new Regex(@"^{(\w+\.)?value\d+}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        //regular expression to extract number
        //private Regex _numberReg = new Regex(@"\d+", RegexOptions.Compiled);

        //regular expression to extract value index, eg: {data1.value1}
        private Regex _valueIndexReg = new Regex(@"\d+}$", RegexOptions.Compiled);

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
                throw new CannotLoadSubException("Parser can not be null.");
            }
            _parser = parser;

            return _subEngine;
        }

        /* List<TestStep> BuildTestStepBySubName(string subName)
         * Return test step list by sub name.
         */
        public List<TestStep> BuildTestStepBySubName(string subName)
        {
            //get origin test steps.
            List<TestStep> subOriginSteps = GetTestSubByName(subName);

            //get datapool used by sub
            string dataPoolName = GetDataPoolName(subOriginSteps);

            if (String.IsNullOrEmpty(dataPoolName))
            {
                return subOriginSteps;
            }
            else
            {
                //if datapool is used in the sub, insert data.
                return InsertDataValueToSub(subOriginSteps, dataPoolName);
            }

        }

        #endregion

        #region private methods

        /* List<TestStep> GetTestSubByName(string subName)
         * return test step list by sub name.
         */
        private List<TestStep> GetTestSubByName(string subName)
        {
            if (_allTestSubs == null)
            {
                if (_parser == null)
                {
                    throw new CannotLoadSubException("Parser can not be null.");
                }

                //get all subs.
                _allTestSubs = _parser.GetAllTestSubs();
            }

            if (String.IsNullOrEmpty(subName))
            {
                throw new CannotLoadSubException("Sub name can not be empty.");
            }


            //check the name, if found, return .
            foreach (TestSub sub in _allTestSubs)
            {
                //find by name.
                if (String.Compare(sub._subName, subName, true) == 0)
                {
                    return sub._subTestSteps;
                }
            }

            throw new CannotLoadSubException("Sub:" + subName + " not found.");
        }

        /* string GetDataPoolName(List<TestStep> stepList)
         * return the datapool name used in the sub.
         * eg: {data1.value1} will return data1
         * 
         */
        private string GetDataPoolName(List<TestStep> stepList)
        {
            if (stepList == null)
            {
                throw new CannotLoadSubException("Sub step list can not be null.");
            }

            string dataPoolName;

            //check each field.
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

        /* bool GetDataPoolNameByReg(string value, out string dataPoolName)
         * check if the string include datapool name, if so, return the datapool name.
         */
        private bool GetDataPoolNameByReg(string value, out string dataPoolName)
        {
            dataPoolName = null;

            if (String.IsNullOrEmpty(value))
            {
                return false;
            }

            //use reg expression to check.
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
                        //get the datapool name.
                        dataPoolName = str1.Substring(1, dotPos - 1);
                        return true;
                    }
                }
                catch
                {
                }

            }

            return false;
        }

        ///* bool GetDataPoolDataIndex(string value, out int index)
        // * return the value index.
        // * eg: data1.value1 will return 1, data1.value2 will return 2.
        // * we use this index to load actual data.
        // */
        //private bool GetDataPoolDataIndex(string value, out int index)
        //{
        //    index = -1;

        //    if (String.IsNullOrEmpty(value))
        //    {
        //        return false;
        //    }

        //    //match datapool
        //    if (_dataPoolReg.IsMatch(value))
        //    {
        //        string str1 = _dataPoolReg.Match(value).Value;
        //        try
        //        {
        //            str1 = str1.Replace(" ", "");
        //            str1 = str1.Replace("\t", "");

        //            //dot means the string contains datapool name.
        //            //we must specific the datapool name at the first time, eg: {data1.value1}
        //            //after the 1st, we just need the value, eg:{value2}
        //            int dotPos = str1.IndexOf('.');
        //            if (dotPos > 1)
        //            {
        //                //find value string, eg: value1
        //                string valueX = str1.Substring(dotPos, str1.Length - dotPos);

        //                //check if number is found.
        //                if (_numberReg.IsMatch(valueX))
        //                {
        //                    //if found, get the number.
        //                    if (int.TryParse(_numberReg.Match(valueX).Value, out index))
        //                    {
        //                        if (index > 0 && index < _parser.MaxDataCount)
        //                        {
        //                            return true;
        //                        }
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                if (_numberReg.IsMatch(str1))
        //                {
        //                    if (int.TryParse(_numberReg.Match(str1).Value, out index))
        //                    {
        //                        if (index > 0 && index < _parser.MaxDataCount)
        //                        {
        //                            return true;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch
        //        {
        //        }

        //    }

        //    return false;
        //}

        /* List<TestStep> InsertDataValueToSub(List<TestStep> originSteps, string dataPoolName)
         * return the actual test steps.
         * will replace the datapool string with actual data.
         */
        private List<TestStep> InsertDataValueToSub(List<TestStep> originSteps, string dataPoolName)
        {
            if (originSteps == null || String.IsNullOrEmpty(dataPoolName))
            {
                throw new CannotLoadSubException("Sub steps and/or Data pool name can not be null.");
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
                throw new CannotLoadSubException("Can not load data pool by name:" + dataPoolName);
            }

            List<TestStep> stepsWithData = new List<TestStep>(originSteps.Count * 2);
            List<string[]> datas = dataPool._data;

            foreach (string[] sArr in datas)
            {
                for (int i = 0; i < originSteps.Count; i++)
                {
                    TestStep newStep = InsertDataToSingleTestStep(originSteps[i], sArr);
                    stepsWithData.Add(newStep);
                }
            }

            return stepsWithData;
        }

        /* TestStep InsertDataToSingleTestStep(TestStep testStep, string[] data)
         * insert data to a single test step.
         */
        private TestStep InsertDataToSingleTestStep(TestStep testStep, string[] data)
        {
            int index = 0; //index of data value, eg: value1, value2

            //check each field
            for (int i = 0; i < testStep.Size; i++)
            {
                //if datapool found
                if (GetDataIndex(testStep[i], out index))
                {
                    testStep[i] = data[index];
                }
            }

            return testStep;
        }

        /*  bool GetDataIndex(string value, out int index)
         *  get the data value index of a string.
         */
        private bool GetDataIndex(string value, out int index)
        {
            index = -1;
            if (_valueIndexReg.IsMatch(value))
            {
                //indexStr=value1}
                string indexStr = _valueIndexReg.Match(value).Value;

                //get the actual index, value2} -> 2
                //in drive file, the index start at 1. eg: value1, so we need to minus 1, make it start at 0
                index = int.Parse(indexStr.Remove(indexStr.Length - 1)) - 1;

                return true;
            }

            return false;
        }

        #endregion

        #endregion


    }
}
