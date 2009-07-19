using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;

namespace Shrinerain.AutoTester.Core
{
    //this class defines properties we may use in recording/playback.
    public class TestProperty : IComparable
    {
        #region fields

        private static Regex _testProRegex = new Regex("name=(?<name>.*),value=(?<value>.*),isregex=(?<isregex>.*),weight=(?<weight>.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private const string RegFlag = "@";
        public const char PropertySeparator = ';';

        private String _name;
        private Object _value;
        private Regex _regValue;
        private bool _isRegex = false;
        //weight of the property, should between 0 and 100.
        //more bigger, more important.
        private int _weight;

        public int Weight
        {
            get { return _weight; }
        }
        public bool IsRegex
        {
            get { return _isRegex; }
        }
        public Object Value
        {
            get { return _value; }
        }
        public String Name
        {
            get { return _name; }
        }

        #endregion

        #region methods

        #region ctor

        public TestProperty(String name, String value)
            : this(name, value, false)
        {
        }

        public TestProperty(String name, String value, bool isReg)
            : this(name, value, isReg, 100)
        {
        }

        public TestProperty(String name, object value, bool isReg, int weight)
        {
            this._name = name;
            this._value = value;

            if (value is String)
            {
                string valueStr = value.ToString();
                this._isRegex = isReg || valueStr.StartsWith(RegFlag);
                if (this._isRegex)
                {
                    if (valueStr.StartsWith(RegFlag))
                    {
                        valueStr = valueStr.Remove(0, 1);
                        this._value = valueStr;
                    }
                    _regValue = new Regex(valueStr, RegexOptions.Compiled);
                }
            }

            if (weight < 0)
            {
                this._weight = 0;
            }
            else if (weight > 100)
            {
                this._weight = 100;
            }
            else
            {
                this._weight = weight;
            }
        }

        public TestProperty(String str)
        {
            if (String.IsNullOrEmpty(str))
            {
                throw new CannotParseTestPropertyException("String can not be empty.");
            }

            try
            {
                Match m = _testProRegex.Match(str);
                if (m.Success)
                {
                    this._name = m.Groups["name"].Value;
                    this._value = m.Groups["value"].Value;
                    this._isRegex = Convert.ToBoolean(m.Groups["isregex"].Value);

                    if (_isRegex)
                    {
                        _regValue = new Regex(this._value.ToString(), RegexOptions.Compiled);
                    }

                    this._weight = int.Parse(m.Groups["weight"].Value);
                    if (this._weight < 0)
                    {
                        this._weight = 0;
                    }
                    else if (this._weight > 100)
                    {
                        this._weight = 100;
                    }
                }
                else
                {
                    throw new CannotParseTestPropertyException("Can not parse string: " + str);
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotParseTestPropertyException("Can not parse string: " + str + ", " + ex.ToString());
            }
        }

        #endregion

        public bool IsMatch(TestProperty other)
        {
            return String.Compare(this._name, other._name) == 0 && this._value == other._value &&
                   this._isRegex == other._isRegex && this._weight == other._weight;
        }

        public bool IsMatch(TestObject obj)
        {
            if (obj != null)
            {
                object actualValObj = obj.GetProperty(this._name);
                if (actualValObj != null)
                {
                    return IsValueMatch(actualValObj.ToString());
                }
            }

            return false;
        }

        public bool IsNameMatch(TestProperty other)
        {
            return String.Compare(this._name, other._name, true) == 0;
        }

        public bool IsValueMatch(TestProperty other)
        {
            return this._value == other._value && this._isRegex == other._isRegex;
        }

        public bool IsValueMatch(String actualValue)
        {
            return IsValueMatch(actualValue, 100);
        }

        public bool IsValueMatch(String actualValue, int simPercent)
        {
            if (actualValue != null)
            {
                if (!_isRegex)
                {
                    return Searcher.IsStringLike(actualValue, this._value.ToString(), simPercent);
                }
                else
                {
                    return this._regValue.IsMatch(actualValue);
                }
            }

            return false;
        }

        //compare wether the source list match the des condition.
        public static bool IsListMatch(List<TestProperty> source, List<TestProperty> des, bool needSort)
        {
            //test wether the other object's identify properties is the same with this object's identify properties.
            int totalWeight = 0;

            if (source != null && des != null && source.Count == des.Count)
            {
                //sort the list before compare.
                if (needSort)
                {
                    source.Sort();
                    des.Sort();
                }

                for (int i = 0; i < source.Count; i++)
                {
                    try
                    {
                        TestProperty thisPro = source[i];
                        TestProperty otherPro = des[i];

                        //name not match, break, return false.
                        if (!thisPro.IsNameMatch(otherPro))
                        {
                            totalWeight -= thisPro.Weight;
                        }
                        else if (thisPro.IsValueMatch(otherPro))
                        {
                            totalWeight += thisPro.Weight;
                        }

                        if (thisPro.Weight == 100)
                        {
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return totalWeight > 0;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Name=" + this._name);
            sb.Append(",Value=" + this._value);
            sb.Append(",IsRegex=" + this._isRegex);
            sb.Append(",Weight=" + this._weight);

            return sb.ToString();
        }

        public static bool TryGetProperties(string str, out TestProperty[] properties)
        {
            bool res = false;
            properties = null;
            if (str != null && !String.IsNullOrEmpty(str.Trim()) && str.IndexOf("=") > 0)
            {
                try
                {
                    properties = GetProperties(str);
                    return properties != null;
                }
                catch
                {
                    res = false;
                }
            }

            return res;
        }

        //parse string.
        //properties is splitted by ";"
        //eg: "id=btnG;name=google".
        public static TestProperty[] GetProperties(string str)
        {
            if (str != null && !String.IsNullOrEmpty(str.Trim()))
            {
                List<TestProperty> properties = new List<TestProperty>();
                int startIndex = 0;
                int endIndex = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    char c = str[i];
                    if (c == PropertySeparator || i == str.Length - 1)
                    {
                        if (!IsEscaped(str, i))
                        {
                            endIndex = (c == PropertySeparator ? i : i + 1);
                            string propertyStr = str.Substring(startIndex, endIndex - startIndex);
                            int nameEndPos = 0;
                            int valueStartPos = 0;
                            for (int j = 0; j < propertyStr.Length; j++)
                            {
                                if (propertyStr[j] == '=' && !IsEscaped(propertyStr, j))
                                {
                                    nameEndPos = j;
                                    valueStartPos = j + 1;
                                    break;
                                }
                            }

                            string name = propertyStr.Substring(0, nameEndPos).Trim();
                            string value = propertyStr.Substring(valueStartPos, propertyStr.Length - valueStartPos);
                            bool isReg = false;
                            if (String.IsNullOrEmpty(name))
                            {
                                name = TestConstants.PROPERTY_VISIBLE;
                            }
                            else
                            {
                                if (name.StartsWith(RegFlag))
                                {
                                    name = name.Remove(0, 1);
                                    isReg = name.StartsWith(RegFlag);
                                }
                            }

                            TestProperty p = new TestProperty(name, value, isReg);
                            properties.Add(p);

                            startIndex = endIndex + 1;
                        }
                    }
                }

                return properties.ToArray();
            }

            return null;
        }

        private static bool IsEscaped(String str, int index)
        {
            if (index <= 0)
            {
                return false;
            }
            else
            {
                return str[index - 1] == '\\' && !IsEscaped(str, index - 2);
            }
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj != null && obj is TestProperty)
            {
                TestProperty other = (TestProperty)obj;
                //compare name
                if (String.Compare(this._name, other._name, true) != 0)
                {
                    return String.Compare(this._name, other._name, true);
                }
                else
                {
                    String thisValue = this._value.ToString();
                    String otherValue = other._value.ToString();

                    //compare value
                    if (String.Compare(thisValue, otherValue) != 0)
                    {
                        return String.Compare(thisValue, otherValue);
                    }
                    else
                    {
                        //compare weight
                        if (this._weight != other._weight)
                        {
                            return this._weight.CompareTo(other._weight);
                        }
                        else
                        {
                            //compare regex flag.
                            return this._isRegex.CompareTo(other._isRegex);
                        }
                    }
                }
            }

            return 1;
        }

        #endregion
    }
}
