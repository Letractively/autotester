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

        private static ITestBrowser _browser;
        private static ITestObjectPool _objPool;
        private static ITestAction _actionPool;
        private static ITestVP _testVP;

        private static string _testBrowserDLL;
        private static string _testObjPoolDLL;
        private static string _testVPDLL;
        private static string _testActionDLL;

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

        public static ITestBrowser CreateTestBrowser()
        {
            if (String.IsNullOrEmpty(_testBrowserDLL))
            {
                throw new CanNotLoadDllException("Test browser dll can not be null.");
            }

            try
            {
                _browser = (ITestBrowser)LoadPlugin(_testBrowserDLL, _testBrowserClassName);
            }
            catch
            {
                throw new CanNotLoadDllException("Can not create instance of Test Browser");
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
            catch
            {
                throw new CanNotLoadDllException("Can not create instance of test object pool.");
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
            catch
            {
                throw new CanNotLoadDllException("Can not create instance of test action.");
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
            catch
            {
                throw new CanNotLoadDllException("Can not create instance of test VP.");
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

                    bool allFound = true;

                    _testBrowserDLL = tmp._testBrowserDLL;
                    _testBrowserClassName = tmp._testBrowser;

                    _testObjPoolDLL = tmp._testObjectPoolDLL;
                    _testObjectPoolClassName = tmp._testObjectPool;

                    _testActionDLL = tmp._testActionDLL;
                    _testActionClassName = tmp._testAction;

                    _testVPDLL = tmp._testVPDLL;
                    _testVPClassName = tmp._testVP;

                    if (!SearchDLL(currentPath, ref _testBrowserDLL))
                    {
                        allFound = false;
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
                catch
                {
                    throw new CanNotLoadDllException("Can not get the dll path");
                }
            }
        }


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
            Type type = _assembly.GetType(className);
            return Activator.CreateInstance(type);
        }

        #endregion

        #endregion

    }
}
