using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public interface IProperty
    {
        object GetProperty(string name);
        bool TryGetProperty(String name, out object value);
        bool SetProperty(string propertyName, object value);
    }
}
