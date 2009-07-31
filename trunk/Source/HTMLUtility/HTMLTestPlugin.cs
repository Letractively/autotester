using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.Interface;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestPlugin : ITestPlugin
    {
        #region ITestPlugin Members

        public string GetPluginID()
        {
            return "CD2B69EC-4DDA-4056-A5C3-ACC46826ADAE";
        }

        public string GetPluginName()
        {
            return "HTML";
        }

        public string GetPluginType()
        {
            return "Web";
        }

        public string GetPluginVersion()
        {
            return "0.9";
        }

        public string GetPluginFileName()
        {
            return "HTMLUtility";
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
            if (handle != IntPtr.Zero)
            {
                String windowClass = Win32API.GetClassName(handle);
                return String.Compare(windowClass, TestConstants.IE_Server_Class, true) == 0;
            }

            return false;
        }

        public ITestSession CreateTestSession()
        {
            return new HTMLTestSession();
        }

        #endregion
    }
}
