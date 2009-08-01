using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core.Interface;

namespace Shrinerain.AutoTester.Core
{
    public enum TestPluginAppType
    {
        Web,
        Windows
    }

    public class TestPluginManager
    {
        #region fields

        protected static String _configFile = "";

        #endregion

        #region properties

        public static String ConfigFile
        {
            get { return TestPluginManager._configFile; }
            set { TestPluginManager._configFile = value; }
        }

        #endregion

        #region methods

        public static ITestPlugin GetAllPlugins()
        {
            return null;
        }

        public static ITestPlugin GetEnabledPlugins()
        {
            return null;
        }

        public static ITestPlugin GetPluginByID(String id)
        {
            return null;
        }

        public static ITestPlugin GetPluginByName(String name)
        {
            return null;
        }

        #endregion
    }
}
