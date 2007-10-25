using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Interface
{
    public interface ITestObjectPool
    {
        Object GetObjectByIndex(int index);

        Object GetObjectByRegex(string property, string regex);

        Object GetObjectByProperty(string property, string value);

        Object GetObjectByID(string id);

        Object GetObjectByName(string name);

        Object GetObjectByType(string type, string values, int index);

        Object GetObjectByAI(string value);

        Object GetObjectByPoint(int x, int y);

        Object GetObjectByRect(int top, int left, int width, int height);

        Object GetObjectByColor(string color);

        Object GetObjectByCustomer(object value);

        Object[] GetAllObjects();

        void SetTestBrowser(ITestBrowser testBrowser);
    }
}
