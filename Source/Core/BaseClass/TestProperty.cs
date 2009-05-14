using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Shrinerain.AutoTester.Core
{
    //this class defines properties we may use in recording/playback.
    public class TestProperty : IComparable
    {
        #region fields

        private static Regex _testProRegex = new Regex("name=(?<name>.*),value=(?<value>.*),isregex=(?<isregex>.*),weight=(?<weight>.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private String _name;
        private Object _value;
        private bool _isRegex;
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
            this._isRegex = isReg;

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

        public bool IsNameMatch(TestProperty other)
        {
            return String.Compare(this._name, other._name, true) == 0;
        }

        public bool IsValueMatch(TestProperty other)
        {
            return this._value == other._value && this._isRegex == other._isRegex;
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

        public static bool TryGetIDValue(TestProperty[] properties, out string id)
        {
            id = "";
            if (properties != null && properties.Length > 0)
            {
                foreach (TestProperty tp in properties)
                {
                    if (String.Compare(tp._name, "id", true) == 0)
                    {
                        id = tp.Value.ToString();
                        return true;
                    }
                }
            }

            return false;
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

        /* GetProperties(string str, out string[] properties, out string[] values)
        * Parse the text like "id=btnG", return the properties array and values array.
        * eg: properties=new string[]{"id"} , values=new string[]{"btnG"}.
        */
        public static bool TryGetProperties(string str, out TestProperty[] properties)
        {
            bool res = false;
            properties = null;
            if (str != null && !String.IsNullOrEmpty(str.Trim()) && str.IndexOf("=") > 0)
            {
                //properties is splitted by ";"
                //eg: "id=btnG;name=google".
                string[] propPairs = str.Split(';');

                List<TestProperty> propertiesList = new List<TestProperty>(propPairs.Length);
                for (int i = 0; i < propPairs.Length; i++)
                {
                    string p = propPairs[i];
                    if (!String.IsNullOrEmpty(p))
                    {
                        int ePos = p.LastIndexOf("=");
                        if (ePos > 0)
                        {
                            //get property name, before "="
                            string prop = p.Substring(0, ePos);
                            string val = "";
                            if (ePos < p.Length - 1)
                            {
                                //get value , after "="
                                val = p.Substring(ePos + 1, p.Length - ePos - 1);
                            }

                            propertiesList.Add(new TestProperty(prop, val));

                            //we found property/value pair, set the result to true.
                            res = true;
                        }
                    }
                }

                properties = propertiesList.ToArray();
            }

            return res;
        }

        public static TestProperty[] GetProperties(string str)
        {
            if (str != null && !String.IsNullOrEmpty(str.Trim()) && str.IndexOf("=") > 0)
            {
                //properties is splitted by ";"
                //eg: "id=btnG;name=google".
                string[] propPairs = str.Split(';');

                List<TestProperty> properties = new List<TestProperty>(propPairs.Length);
                for (int i = 0; i < propPairs.Length; i++)
                {
                    string p = propPairs[i];
                    if (!String.IsNullOrEmpty(p))
                    {
                        int ePos = p.LastIndexOf("=");
                        if (ePos > 0)
                        {
                            //get property name, before "="
                            string prop = p.Substring(0, ePos);
                            string val = "";
                            if (ePos < p.Length - 1)
                            {
                                val = p.Substring(ePos + 1, p.Length - ePos - 1);
                            }

                            properties.Add(new TestProperty(prop, val));
                        }
                    }
                }

                return properties.ToArray();
            }

            return null;
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
