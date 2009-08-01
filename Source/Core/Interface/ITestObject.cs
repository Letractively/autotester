using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core.Interface
{
    public interface ITestObject : IProperty
    {
        String Domain { get; }
        String Type { get; }
        ITestWindow ParentWindow { get; }
        ITestPage ParentPage { get; }

        void SetIdenProperties(String[] idenProperties);
        List<TestProperty> GetIdenProperties();
    }
}
