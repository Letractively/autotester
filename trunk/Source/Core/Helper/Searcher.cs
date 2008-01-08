
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
*          2008/01/08 wan,yu bug fix for GetSimilarPercent, may enter dead loop when
*                            two strings are 100% different.                 
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
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
                if (value > 0 && value <= 100)
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
            return IsStringLike(str1, str2, _defaultPercent);
        }

        /* static bool IsStringEqual(string str1, string str2, int percent)
         * return true if the two string match the percent of similarity.
         */
        public static bool IsStringLike(string str1, string str2, int percent)
        {
            if (String.IsNullOrEmpty(str1) && String.IsNullOrEmpty(str2))
            {
                return true;
            }
            else if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
            {
                return false;
            }

            if (String.Compare(str1, str2, true) == 0)
            {
                return true;
            }
            else
            {
                if (percent > 100)
                {
                    percent = 100;
                }
                else if (percent < 1)
                {
                    percent = 1;
                }

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
            //both are empty, they are the same
            if (String.IsNullOrEmpty(str1) && String.IsNullOrEmpty(str2))
            {
                return 100;
            }
            else if (String.IsNullOrEmpty(str1) || String.IsNullOrEmpty(str2))
            {
                return 0;
            }
            else
            {
                str1 = str1.Trim();
                str2 = str2.Trim();

                //if the two strings is a sentence(contains blank) not a single word, then split the sentence to words, check each word.
                if (str1.IndexOf(" ") > 0 && str2.IndexOf(" ") > 0)
                {
                    string[] str1Arr = str1.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] str2Arr = str2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (str1Arr.Length > 1 && str1Arr.Length == str2Arr.Length)
                    {
                        int totalSimPercent = 0;
                        float weighted = 0;

                        for (int i = 0; i < str1Arr.Length; i++)
                        {
                            weighted = (float)(str1Arr[i].Length + str2Arr[i].Length) / (float)(str1.Length + str2.Length);
                            totalSimPercent += Convert.ToInt32(GetSimilarPercent(str1Arr[i], str2Arr[i]) * weighted);
                        }

                        return totalSimPercent;
                    }

                }

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

                    if (sameCharCount > 0)
                    {
                        str1Index = maxStr1Index - sameCharCount;
                        str2Index = maxStr2Index - sameCharCount;
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
