using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.Interface;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public class MSAATestPlugin : ITestPlugin
    {
        #region ITestPlugin Members

        public string GetPluginID()
        {
            return "340E2FF0-8BBD-4133-B4EE-B35B12855545";
        }

        public string GetPluginName()
        {
            return "MSAA";
        }

        public string GetPluginType()
        {
            return "All";
        }

        public string GetPluginVersion()
        {
            return "0.9";
        }

        public string GetPluginFileName()
        {
            return "MSAAUtility";
        }

        public string GetCompany()
        {
            return "Shrinerain";
        }

        public string GetAuthor()
        {
            return "Shrinerain";
        }

        public string GetHelpInfo()
        {
            return "";
        }

        public string GetWebsite()
        {
            return "http://code.google.com/p/autotester/";
        }

        public bool Update(out string updateInfo)
        {
            throw new NotImplementedException();
        }

        public bool IsSupportedApp(IntPtr handle)
        {
            return handle != IntPtr.Zero;
        }

        public ITestSession CreateTestSession()
        {
            return new MSAATestSession();
        }

        #endregion
    }
}
