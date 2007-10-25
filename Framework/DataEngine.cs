using System;
using System.Collections.Generic;
using System.Text;
using Shrinerain.AutoTester.Function;

namespace Shrinerain.AutoTester.Framework
{
    public sealed class DataEngine
    {

        #region fields

        private static DataEngine _dataEngine = new DataEngine();

        private static Parser _parser = Parser.GetInstance();

        private List<TestDataPool> _allDataPool;

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
                throw new CanNotLoadDataPoolException("Parser can not be null.");
            }

            _parser = parser;

            return _dataEngine;
        }

        public TestDataPool GetDataPoolByName(string dataPoolName)
        {
            if (String.IsNullOrEmpty(dataPoolName))
            {
                throw new CanNotLoadDataPoolException("Datapool name can not be empty.");
            }

            if (_parser == null)
            {
                throw new CanNotLoadDataPoolException("Parser can not be null.");
            }

            if (this._allDataPool == null)
            {
                this._allDataPool = Parser.GetAllTestDataPool();
            }

            foreach (TestDataPool d in this._allDataPool)
            {
                if (d._datapoolName.ToUpper() == dataPoolName.ToUpper())
                {
                    return d;
                }
            }

            throw new CanNotLoadDataPoolException("Can not load data pool by name:" + dataPoolName);
        }

        #endregion

        #region private methods


        #endregion

        #endregion

    }
}
