using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Function;

namespace Shrinerain.AutoTester.Framework
{
    public sealed class ObjectEngine
    {

        #region fields

        private ITestObjectPool _objPool;
        private ITestBrowser _testBrowser;

        private AutoConfig _autoConfig = AutoConfig.GetInstance();

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public ObjectEngine()
        {
            LoadPlugin();
        }

        #endregion

        #region public methods

        public TestBrowser GetTestBrowser()
        {
            return (TestBrowser)_testBrowser;
        }

        public TestObject GetTestObject(TestStep step)
        {
            TestObject tmp = null;

            string item = step._testItem;
            string property = step._testProperty;

            if (String.IsNullOrEmpty(item))
            {
                throw new ObjectNotFoundException("Item can not be empty.");
            }

            if (!String.IsNullOrEmpty(property))
            {
                if (property.StartsWith("."))
                {
                    if (property.ToUpper() == ".ID")
                    {
                        tmp = (TestObject)_objPool.GetObjectByID(item);
                    }
                    else if (property.ToUpper() == ".NAME")
                    {
                        tmp = (TestObject)_objPool.GetObjectByName(item);
                    }
                    else
                    {
                        tmp = (TestObject)_objPool.GetObjectByProperty(property, item);
                    }
                }
                else
                {
                    tmp = (TestObject)_objPool.GetObjectByType(property, item, 0);
                }

            }
            else
            {
                tmp = (TestObject)_objPool.GetObjectByAI(item);
            }

            return tmp;


        }

        #endregion

        #region private methods

        private void LoadPlugin()
        {
            _testBrowser = TestFactory.CreateTestBrowser();
            _objPool = TestFactory.CreateTestObjectPool();
            _objPool.SetTestBrowser(_testBrowser);
        }


        #endregion

        #endregion

    }
}
