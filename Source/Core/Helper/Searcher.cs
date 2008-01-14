
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
* History: 2007/12/24 wan,yu Init version.
*          2008/01/08 wan,yu bug fix for GetSimilarPercent, may enter dead loop when
*                            two strings are 100% different.            
*          2008/01/10 wan,yu update, rename IsStringEqual to IsStringLike.  
*          2008/01/12 wan,yu update, modify GetSimilarPercent, use unsafe code to 
*                            improve performance.          
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

        private Searcher()
        {

        }

        #endregion

        #region public methods

        /* bool IsStringEqual(string str1, string str2)
         * Check if two string is "equal".
         */
        public static bool IsStringLike(string str1, string str2)
        {
            return IsStringLike(str1, str2, _defaultPercent);
        }

        /* static bool IsStringEqual(string str1, string str2, int percent)
         * return true if the two string match the percent of similarity.
         */
        public static bool IsStringLike(string str1, string str2, int percent)
        {
            if (percent > 100 || percent < 1)
            {
                throw new FuzzySearchException("Similar percent must between 1 and 100.");
            }

            if (String.Compare(str1, str2, true) == 0)
            {
                return true;
            }
            else
            {
                try
                {
                    return GetSimilarPercent(str1, str2) >= percent;
                }
                catch (Exception e)
                {
                    throw new FuzzySearchException("Fuzzy search error: " + e.Message);
                }
            }

        }

        #endregion

        #region private methods

        /* int GetSimilarity(string str1, string str2)
        * return the similarity bewteen 2 string, use dynamic programming.
        * the similarity = the count of same chracters *2 /(length of str1 + length of str2)
        * eg: test1, test2, they have 4 same chracters, so the similarity = 4*2/(5+5)=0.8=80%
        * for performance issue, use unsafe code to access the array. 
        */
        private unsafe static int GetSimilarPercent(string str1, string str2)
        {
            //check if they are equal
            if (String.Compare(str1, str2, true) == 0)
            {
                return 100;
            }
            else if (String.IsNullOrEmpty(str1) || String.IsNullOrEmpty(str2))
            {
                //one string is null, return 0
                return 0;
            }
            else
            {

                //both two strings are not empty, then we can start to check the similar percent.
                str1 = str1.Trim();
                str2 = str2.Trim();

                //if the two strings is a sentence(contains blank) not a single word, then split the sentence to words, check each word.
                if (str1.IndexOf(" ") > 0 && str2.IndexOf(" ") > 0)
                {
                    string[] str1Arr = str1.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] str2Arr = str2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    //if the two strings have the same number of words, check each word.
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
                //to improve performance, we use 1 dim stack arrary and unsafe code.
                int* dpArr = stackalloc int[(len1 + 1) * (len2 + 1)];

                //init array
                int curIndex = 0;

                for (int i = 0; i < len1; i++)
                {
                    for (int j = 0; j < len2; j++)
                    {
                        curIndex = (j + 1) * (len1 + 1) + i + 1;

                        if (str1[i] == str2[j])
                        {
                            if (i == 0 || j == 0)
                            {
                                dpArr[curIndex] = 1;
                            }
                            else
                            {
                                dpArr[curIndex] = dpArr[j * (len1 + 1) + i] + 1;
                            }
                        }
                        else
                        {
                            dpArr[curIndex] = 0;
                        }
                    }
                }

                int currentSameCharCount = 0;
                int totalSameCharCount = 0;
                int totalLen = len1 + len2;

                int str1Index = len1;
                int str2Index = len2;

                //the max number's position in the array of sepcific line and row.
                int maxStr1Index = str1Index;
                int maxStr2Index = str2Index;

                while (str1Index > 0 && str2Index > 0)
                {
                    currentSameCharCount = 0;

                    for (int i = str1Index; i > 0; i--)
                    {
                        curIndex = str2Index * (len1 + 1) + i;

                        if (dpArr[curIndex] > currentSameCharCount)
                        {
                            currentSameCharCount = dpArr[curIndex];

                            maxStr1Index = i;
                            maxStr2Index = str2Index;
                        }

                        if (currentSameCharCount >= i)
                        {
                            break;
                        }
                    }

                    for (int j = str2Index; j > 0; j--)
                    {
                        curIndex = j * (len1 + 1) + str1Index;

                        if (dpArr[curIndex] > currentSameCharCount)
                        {
                            currentSameCharCount = dpArr[curIndex];

                            maxStr1Index = str1Index;
                            maxStr2Index = j;
                        }

                        if (currentSameCharCount >= j)
                        {
                            break;
                        }
                    }

                    totalSameCharCount += currentSameCharCount;

                    if (currentSameCharCount > 0)
                    {
                        str1Index = maxStr1Index - currentSameCharCount;
                        str2Index = maxStr2Index - currentSameCharCount;
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
