using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public interface IProperty
    {
        object GetProperty(string name);
        bool SetProperty(string propertyName, object value);
    }
}
