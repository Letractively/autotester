using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core.Interface
{
    public interface ITestObjectType
    {
        String[] GetValidTypes();
        bool IsValidType(String type);
        String GetImage(String type);
    }
}
