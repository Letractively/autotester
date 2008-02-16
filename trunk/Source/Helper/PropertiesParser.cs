/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: PropertiesParser
*
* Description: Parse the text
*
* History: 2008/02/15 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Helper
{
    public sealed class PropertiesParser
    {
        #region fields


        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        private PropertiesParser()
        {

        }

        #endregion

        #region public methods

        /* GetProperties(string str, out string[] properties, out string[] values)
         * Parse the text like "id=btnG", return the properties array and values array.
         * eg: properties=new string[]{"id"} , values=new string[]{"btnG"}.
         */
        public static bool GetProperties(string str, out string[] properties, out string[] values)
        {

            properties = null;
            values = null;

            bool res = false;

            if (str != null && !String.IsNullOrEmpty(str.Trim()))
            {
                //properties is splitted by ";"
                //eg: "id=btnG;name=google".
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
                                //if the "=" is the last char, we think the value is empty.
                                val = "";
                            }

                            //we found property/value pair, set the result to true.
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

        #endregion

        #region private methods


        #endregion

        #endregion



    }
}
