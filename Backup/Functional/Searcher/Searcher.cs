
/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: Searcher.cs
*
* Description: this class defines some helper methods for search an object.
*
* History: 2007/12/24 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Function
{
    public sealed class Searcher
    {

        #region fields

        //default percent of similarity
        private static int _defaultPercent = 100;

        #endregion

        #region properties

        public static int DefaultPercent
        {
            get { return Searcher._defaultPercent; }
            set
            {
                if (value >= 0 && value <= 100)
                {
                    Searcher._defaultPercent = value;
                }
            }
        }

        #endregion

        #region methods

        #region ctor

        #endregion

        #region public methods

        /* bool IsStringEqual(string str1, string str2)
         * Check if two string is "equal".
         */
        public static bool IsStringEqual(string str1, string str2)
        {
            return IsStringEqual(str1, str2, _defaultPercent);
        }

        /* static bool IsStringEqual(string str1, string str2, int percent)
         * return true if the two string match the percent of similarity.
         */
        public static bool IsStringEqual(string str1, string str2, int percent)
        {
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
            {
                return false;
            }

            if (percent > 100)
            {
                percent = 100;
            }
            else if (percent < 1)
            {
                percent = 1;
            }

            if (String.Compare(str1, str2, true) == 0)
            {
                return true;
            }
            else
            {
                if (percent == 100)
                {
                    return String.Compare(str1, str2, true) == 0;
                }
                else
                {
                    return GetSimilarPercent(str1, str2) >= percent ? true : false;
                }

            }

        }

        #endregion

        #region private methods

        /* int GetSimilarity(string str1, string str2)
        * return the similarity bewteen 2 string, use dynamic programming.
        * the similarity = the count of same chracters *2 /(length of str1 + length of str2)
        * eg: test1, test2, they have 4 same chracters, so the similarity = 4*2/(5+5)=0.8=80%
        */
        private static int GetSimilarPercent(string str1, string str2)
        {
            if (String.IsNullOrEmpty(str1) || String.IsNullOrEmpty(str2))
            {
                return 0;
            }
            else
            {
                int len1 = str1.Length;
                int len2 = str2.Length;

                //dynamic programming array.
                int[,] dpArray = new int[len1 + 1, len2 + 1];

                //init array
                for (int i = 0; i < len1; i++)
                {
                    for (int j = 0; j < len2; j++)
                    {
                        if (str1[i] == str2[j])
                        {
                            if (i == 0 || j == 0)
                            {
                                dpArray[i + 1, j + 1] = 1;
                            }
                            else
                            {
                                dpArray[i + 1, j + 1] = dpArray[i, j] + 1;
                            }
                        }
                        else
                        {
                            dpArray[i + 1, j + 1] = 0;
                        }
                    }
                }

                int sameCharCount = 0;
                int totalSameCharCount = 0;
                int totalLen = len1 + len2;

                int str1Index = len1;
                int str2Index = len2;

                //the max number's position in the array of sepcific line and row.
                int maxStr1Index = str1Index;
                int maxStr2Index = str2Index;

                while (str1Index > 0 && str2Index > 0)
                {
                    sameCharCount = 0;

                    for (int i = str1Index; i > 0; i--)
                    {
                        if (dpArray[i, str2Index] > sameCharCount)
                        {
                            sameCharCount = dpArray[i, str2Index];

                            maxStr1Index = i;
                            maxStr2Index = str2Index;
                        }

                        if (sameCharCount >= i)
                        {
                            break;
                        }
                    }

                    for (int j = str2Index; j > 0; j--)
                    {
                        if (dpArray[str1Index, j] > sameCharCount)
                        {
                            sameCharCount = dpArray[str1Index, j];

                            maxStr1Index = str1Index;
                            maxStr2Index = j;
                        }

                        if (sameCharCount >= j)
                        {
                            break;
                        }
                    }

                    totalSameCharCount += sameCharCount;

                    str1Index = maxStr1Index;
                    str2Index = maxStr2Index;

                    if (sameCharCount > 0)
                    {
                        str1Index -= sameCharCount;
                        str2Index -= sameCharCount;
                    }
                    else
                    {
                        str1Index--;
                        str2Index--;
                    }

                }

                float percent = (float)(totalSameCharCount * 2) / (float)totalLen;
                return Convert.ToInt32(percent * 100);
            }

        }

        #endregion

        #endregion

    }
}
