/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: DataEngine.cs
*
* Description: manage datapool, return datapool to SubEngine.
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;

namespace Shrinerain.AutoTester.Framework
{
    public sealed class DataEngine
    {

        #region fields

        private static DataEngine _dataEngine = new DataEngine();

        private static Parser _parser = Parser.GetInstance();

        //load all datapool to this list.
        private List<TestDataPool> _allDataPool;

        private TestDataPool _currentDataPool;

        #endregion

        #region properties

        public static Parser Parser
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

        #endregion

        #region methods

        #region ctor

        private DataEngine()
        {

        }

        #endregion

        #region public methods

        public static DataEngine GetInstance()
        {
            return _dataEngine;
        }

        public static DataEngine GetInstance(Parser parser)
        {
            if (parser == null)
            {
                throw new CannotLoadDataPoolException("Parser can not be null.");
            }

            _parser = parser;

            return _dataEngine;
        }

        /* TestDataPool GetDataPoolByName(string dataPoolName)
         * return expected datapool by name.
         */
        public TestDataPool GetDataPoolByName(string dataPoolName)
        {
            if (String.IsNullOrEmpty(dataPoolName))
            {
                throw new CannotLoadDataPoolException("Datapool name can not be empty.");
            }

            if (_parser == null)
            {
                throw new CannotLoadDataPoolException("Parser can not be null.");
            }

            if (this._allDataPool == null)
            {
                this._allDataPool = Parser.GetAllTestDataPool();
            }

            foreach (TestDataPool d in this._allDataPool)
            {
                //if same name, return 
                if (String.Compare(d._datapoolName, dataPoolName, true) == 0)
                {
                    this._currentDataPool = d;
                    return this._currentDataPool;
                }
            }

            throw new CannotLoadDataPoolException("Can not load data pool by name:" + dataPoolName);
        }

        /* TestDataPool GetCurrentDataPool()
         * return datapool used currently.
         */
        public TestDataPool GetCurrentDataPool()
        {
            return this._currentDataPool;
        }

        #endregion

        #region private methods


        #endregion

        #endregion

    }
}
