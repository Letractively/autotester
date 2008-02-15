using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Helper
{
    public sealed class PropertiesParser
    {
        public static bool GetProperties(string str, out string[] properties, out string[] values)
        {
            properties = null;
            values = null;

            bool res = false;

            if (str != null && !String.IsNullOrEmpty(str.Trim()))
            {
                string[] propPairs = str.Split(';');
                properties = new string[propPairs.Length];
                values = new string[propPairs.Length];

                string prop = null;
                string val = null;

                for (int i = 0; i < propPairs.Length; i++)
                {
                    string p = propPairs[i];

                    if (!String.IsNullOrEmpty(p))
                    {
                        int ePos = p.LastIndexOf("=");

                        if (ePos > 0)
                        {
                            //get property name, before "="
                            prop = p.Substring(0, ePos);

                            if (ePos < p.Length - 1)
                            {
                                //get value , after "="
                                val = p.Substring(ePos + 1, p.Length - ePos - 1);
                            }
                            else
                            {
                                //if the "=" is the last char, we think value is empty.
                                val = "";
                            }

                            res = true;
                        }
                        else
                        {
                            continue;
                        }

                        properties[i] = prop;
                        values[i] = val;
                    }
                }
            }

            return res;
        }
    }
}
