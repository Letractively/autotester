/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TestFactory.cs
*
* Description: TestFactory load DLL use reflecting.
*              TestFactory read config from AutoConfig, and load expected
*              dll. 
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Function;

namespace Shrinerain.AutoTester.Framework
{
    public sealed class TestFactory
    {

        #region fields

        private static Assembly _assembly;
        private static string _lastDLL;

        //interface 
        private static ITestBrowser _browser;
        private static ITestApp _app;
        private static ITestObjectPool _objPool;
        private static ITestAction _actionPool;
        private static ITestVP _testVP;

        //the dll path
        private static string _testAppDLL;
        private static string _testBrowserDLL;
        private static string _testObjPoolDLL;
        private static string _testVPDLL;
        private static string _testActionDLL;

        //class name for each interface.
        private static string _testAppClassName;
        private static string _testBrowserClassName;
        private static string _testObjectPoolClassName;
        private static string _testActionClassName;
        private static string _testVPClassName;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        private TestFactory()
        {

        }

        static TestFactory()
        {
            GetAllDLLPath();
        }

        #endregion

        #region public methods

        /* static ITestBrowser CreateTestBrowser()
         * return expected ITestBrowser.
         * This interface is used to control the browser.
         */
        public static ITestBrowser CreateTestBrowser()
        {
            if (String.IsNullOrEmpty(_testBrowserDLL))
            {
                throw new CanNotLoadDllException("Test browser dll can not be null.");
            }

            try
            {
                //load the dll, convert to expcted interface.
                _browser = (ITestBrowser)LoadPlugin(_testBrowserDLL, _testBrowserClassName);
            }
            catch (Exception e)
            {
                throw new CanNotLoadDllException("Can not create instance of Test Browser: " + e.Message);
            }

            if (_browser == null)
            {
                throw new CanNotLoadDllException("Can not create instance of test browser.");
            }
            else
            {
                return _browser;
            }


        }

        /*  ITestObjectPool CreateTestObjectPool()
         *  return the interface of ITestObjectPool.
         *  This interface is used to find the test object.
         */
        public static ITestObjectPool CreateTestObjectPool()
        {
            if (String.IsNullOrEmpty(_testObjPoolDLL))
            {
                throw new CanNotLoadDllException("Test object dll can not be null.");
            }
            try
            {
                _objPool = (ITestObjectPool)LoadPlugin(_testObjPoolDLL, _testObjectPoolClassName);
            }
            catch (Exception e)
            {
                throw new CanNotLoadDllException("Can not create instance of test object pool: " + e.Message);
            }
            if (_objPool == null)
            {
                throw new CanNotLoadDllException("Can not create instance of test object pool.");
            }
            else
            {
                return _objPool;
            }

        }

        /* ITestAction CreateTestAction()
         * return ITestAction interface.
         * This interface is used to perform actions on test object.
         */
        public static ITestAction CreateTestAction()
        {
            if (String.IsNullOrEmpty(_testActionDLL))
            {
                throw new CanNotLoadDllException("Test action dll can not be null.");
            }
            try
            {
                _actionPool = (ITestAction)LoadPlugin(_testActionDLL, _testActionClassName);
            }
            catch (Exception e)
            {
                throw new CanNotLoadDllException("Can not create instance of test action: " + e.Message);
            }

            if (_actionPool == null)
            {
                throw new CanNotLoadDllException("Can not create instance of test action.");
            }
            else
            {
                return _actionPool;
            }

        }

        /* ITestVP CreateTestVP()
         * Return ITestVP interface.
         * This interface is used to perform check point.
         */
        public static ITestVP CreateTestVP()
        {
            if (String.IsNullOrEmpty(_testVPDLL))
            {
                throw new CanNotLoadDllException("Test VP dll can not be null.");
            }
            try
            {
                _testVP = (ITestVP)LoadPlugin(_testVPDLL, _testVPClassName);
            }
            catch (Exception e)
            {
                throw new CanNotLoadDllException("Can not create instance of test VP: " + e.Message);
            }
            if (_testVP == null)
            {
                throw new CanNotLoadDllException("Can not create instance of test VP.");
            }
            else
            {
                return _testVP;
            }

        }

        #endregion

        #region private methods

        /* void GetAllDLLPath()
         * Get the dll path from framework config file.
         */
        private static void GetAllDLLPath()
        {
            if (String.IsNullOrEmpty(_testObjPoolDLL))
            {
                try
                {
                    AutoConfig config = AutoConfig.GetInstance();

                    string domain = config.ProjectDomain;

                    string currentPath = Application.StartupPath;// Assembly.GetExecutingAssembly().Location;
                    currentPath = Path.GetDirectoryName(currentPath);

                    PluginInfo tmp = config.GetTestPluginByDomain(domain);

                    _testAppDLL = tmp._testAppDLL;
                    _testAppClassName = tmp._testApp;

                    _testBrowserDLL = tmp._testBrowserDLL;
                    _testBrowserClassName = tmp._testBrowser;

                    _testObjPoolDLL = tmp._testObjectPoolDLL;
                    _testObjectPoolClassName = tmp._testObjectPool;

                    _testActionDLL = tmp._testActionDLL;
                    _testActionClassName = tmp._testAction;

                    _testVPDLL = tmp._testVPDLL;
                    _testVPClassName = tmp._testVP;

                    bool isApp = false;

                    //if the <TestApp> text in config file is not empty, we think it is desktop application.
                    if (!String.IsNullOrEmpty(_testAppDLL))
                    {
                        isApp = true;
                    }

                    bool allFound = true;

                    //search each dll
                    if (isApp)
                    {
                        if (!SearchDLL(currentPath, ref _testAppDLL))
                        {
                            allFound = false;
                        }
                    }
                    else
                    {
                        if (!SearchDLL(currentPath, ref _testBrowserDLL))
                        {
                            allFound = false;
                        }
                    }

                    if (!SearchDLL(currentPath, ref _testObjPoolDLL))
                    {
                        allFound = false;
                    }

                    if (!SearchDLL(currentPath, ref _testActionDLL))
                    {
                        allFound = false;
                    }

                    if (!SearchDLL(currentPath, ref _testVPDLL))
                    {
                        allFound = false;
                    }

                    if (!allFound)
                    {
                        throw new CanNotLoadDllException("Can not load the plugin dll.");
                    }
                }
                catch (CanNotLoadDllException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new CanNotLoadDllException("Can not get the dll path: " + e.Message);
                }
            }
        }

        /* SearchDLL(string dir, ref string dllFullPath)
         * Return true if dll exist.
         */
        private static bool SearchDLL(string dir, ref string dllFullPath)
        {

            if (File.Exists(dllFullPath))
            {
                return true;
            }

            string expectedPath = dir + "//" + dllFullPath;

            if (File.Exists(expectedPath))
            {
                dllFullPath = expectedPath;
                return true;
            }
            else
            {
                string[] dirs = Directory.GetDirectories(dir);

                foreach (string d in dirs)
                {
                    if (SearchDLL(d, ref dllFullPath))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /*  Object LoadPlugin(String dll, string className)
         *  Load dll, use reflecting.
         */
        private static Object LoadPlugin(String dll, string className)
        {
            if (_lastDLL == null)
            {
                _lastDLL = "";
            }

            if (dll.ToUpper() != _lastDLL.ToUpper())
            {
                try
                {
                    _assembly = Assembly.LoadFrom(dll);
                    _lastDLL = dll;
                }
                catch
                {
                    throw;
                }

            }

            //use reflecting to create instance for dll.
            Type type = _assembly.GetType(className);
            return Activator.CreateInstance(type);
        }

        #endregion

        #endregion

    }
}
