/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ObjectEngine.cs
*
* Description: This ObjectEngine get the object from object pool.
*              It recieve the pararmeters from CoreEnging, and return 
*              the object to CoreEngine.
*       
* History: 2007/09/04 wan,yu Init version
*          2007/11/20 wan,yu add ITestApp interface. 
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.Framework
{
    public sealed class ObjectEngine
    {

        #region fields

        //interface we need.
        private ITestObjectPool _objPool;


        private ITestBrowser _testBrowser;
        private ITestApp _testApp;

        //get config 
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

        /* ITestBrowser GetTestBrowser()
         * return the test browser we used currently.
         */
        public ITestBrowser GetTestBrowser()
        {
            return (ITestBrowser)_testBrowser;
        }

        /* ITestApp GetTestApp()
         * return the test application.
         */
        public ITestApp GetTestApp()
        {
            return (ITestApp)_testApp;
        }

        /* TestObject GetTestObject(TestStep step)
         * return the test object we need.
         * will get object from actual test object pool.
         * eg: HTMLTestObjectPool.
         */
        public TestObject GetTestObject(TestStep step)
        {
            TestObject tmp = null;

            string item = step._testControl;
            string property = step._testProperty;

            if (String.IsNullOrEmpty(item))
            {
                throw new ObjectNotFoundException("Item can not be empty.");
            }

            if (!String.IsNullOrEmpty(property))
            {
                //if the property is started with ".", we think it is internal property.
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
                    //if not started with "." , treat it as a type.
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

        /* void LoadPlugin()
         * Get the dll we need.
         * use reflecting to load dll at runtime.
         */
        private void LoadPlugin()
        {
            _objPool = TestFactory.CreateTestObjectPool();

            if (TestFactory.AppType == TestAppType.Desktop)
            {
                _testApp = TestFactory.CreateTestApp();
                _objPool.SetTestApp(_testApp);
            }
            else if (TestFactory.AppType == TestAppType.Web)
            {
                _testBrowser = TestFactory.CreateTestBrowser();
                _objPool.SetTestBrowser(_testBrowser);
            }
            else
            {
                throw new CannotInitCoreEngineException("Unknow application type.");
            }

        }


        #endregion

        #endregion

    }
}
