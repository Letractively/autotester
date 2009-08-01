using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core.Interface
{
    public interface ITestPlugin
    {
        String GetPluginID();
        String GetPluginName();
        TestPluginAppType GetPluginType();
        String GetPluginVersion();
        String GetPluginFileName();

        String GetCompany();
        String GetAuthor();
        String GetHelpInfo();
        String GetWebsite();

        bool Update(out String updateInfo);
        bool IsSupportedApp(IntPtr handle);

        ITestSession CreateTestSession();
    }
}
