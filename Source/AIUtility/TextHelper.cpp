/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TextHelper.cs
*
* Description: This class provide functions to handle strings.
*
* History: 2008/03/18 wan,yu Init version.
*
*********************************************************************/

#include "Stdafx.h"

#include "TextHelper.h"

using namespace System;
using namespace System::Text;
using namespace System::Collections;
using namespace System::Collections::Generic;

using namespace Shrinerain::AutoTester::AIUtility;

/* int CalSimilarPercent(string str1, string str2)
* return the similarity of 2 strings, use dynamic programming.
* the similarity = the count of same chracters *2 /(length of str1 + length of str2)
* eg: test1, test2, they have 4 same chracters, so the similarity = 4*2/(5+5)=0.8=80%
* for performance issue, use unsafe code to access the dynamic array. 
*/
int TextHelper::CalSimilarPercent(String ^str1,String ^str2, bool ignoreCase, bool ignoreBlank)
{
	//check if they are equal
	if (String::Compare(str1, str2, ignoreCase) == 0)
	{
		return 100;
	}
	else if (String::IsNullOrEmpty(str1) || String::IsNullOrEmpty(str2))
	{
		//one string is null, return 0
		return 0;
	}
	else
	{
		//both two strings are not empty, then we can start to check the similar percent.
		int len1 = str1->Length;
		int len2 = str2->Length;

		//dynamic programming array.
		int dpArr[256*256];

		//init array
		int curIndex = 0;

		bool same=false;

		for (int i = 0; i < len1; i++)
		{
			if(ignoreBlank && str1[i]==' ')
			{
				continue;
			}

			for (int j = 0; j < len2; j++)
			{
				if(ignoreBlank && str2[j]==' ')
				{
					continue;
				}

				same=false;

				curIndex = (j + 1) * (len1 + 1) + i + 1;

				if (str1[i] == str2[j])
				{
					same=true;
				}
				else if(ignoreCase&& (str1[i]-str2[j]==32 || str1[i]-str2[j]==-32)
					&& str1[i]>=65 && str1[i]<=122
					&& str2[j]>=65 && str2[j]<=122)
				{
					same=true;
				}

				if(same)
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

		//the max number's position in the array of sepcific col and row.
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

		return Convert::ToInt32((float)(totalSameCharCount * 2 * 100) / (float)totalLen);
	}
}

int TextHelper::CalSimilarPercent(String ^str1, String ^str2)
{
	return CalSimilarPercent(str1,str2,true,true);
}

int TextHelper::CalStyleSimPercent(String^ str1,String ^str2)
{
	return 1;
}

CharClass TextHelper::GetCharClass(char ch)
{
	if(ch>='A' && ch<='z')
	{
		return CharClass::Alpha;
	}
	else if(ch>='0' && ch<='9')
	{
		return CharClass::Number;
	}
	else if(ch==' ')
	{
		return CharClass::Space;
	}
	else if(ch=='\t')
	{
		return CharClass::Tab;
	}
	else if(ch=='\n')
	{
		return CharClass::NewLine;
	}
	else if(ch=='(' || ch==')' || ch=='[' ||ch==']' || ch=='{' || ch=='}')
	{
		return CharClass::Brackets;
	}
	else if(ch=='+' ||ch=='-' ||ch=='*' || ch=='/')
	{
		return CharClass::Operator;
	}
	else if(ch==',' ||ch=='.' ||ch=='?' ||ch=='!' ||ch==';' || ch==':' ||ch=='"'|| ch=='\'')
	{
		return CharClass::Punctuation;
	}
	else if(ch=='~' ||ch=='@'||ch=='#' ||ch=='$' || ch=='%' ||ch=='^' ||ch=='&')
	{
		return CharClass::Special;
	}
	else
	{
		return CharClass::Other;
	}
}

/* String[] SplitWords
 * return a string array which contains the words included in the text.
 */
array<String^>^ TextHelper::SplitWords(String^ text)
{
	array<String^>^ res=gcnew array<String^>(1){text};

	if(!String::IsNullOrEmpty(text))
	{
		List<String^> wordsList;
		int startPos=0;
		int endPos=0;

		for(int i=0;i<text->Length;i++)
		{
			if(i>0 && (text[i]<='Z' && text[i]>='A') && (text[i-1]<'A' || text[i-1]>'Z'))
			{
				startPos=i;
			}
			else if(i==text->Length-1 || (i>0 && ( 
				                         (text[i]>='a' && text[i]<='z')&& (text[i+1]<'a' ||text[i+1]>'z')||
				                         (text[i]>='A' && text[i]<='Z' && text[i-1]>='A' && text[i-1]<='Z' && (text[i+1]<'A' || text[i+1]>'Z')) )))
			{
				endPos=i;
			}

			if(endPos>startPos)
			{
				String^ curWords=text->Substring(startPos,endPos-startPos+1);

				if(!String::IsNullOrEmpty(curWords->Trim()))
				{
					wordsList.Add(curWords->Trim());
				}

				startPos=endPos+1;
			}
		}

		res=gcnew array<String^>(wordsList.Count);

		for(int j=0;j<wordsList.Count;j++)
		{
			res[j]=wordsList[j];
		}
	}

	return res;
}

String^ TextHelper::GetShortWord(String^ word)
{
	if(!String::IsNullOrEmpty(word) && word->Length>3)
	{
		return word->Substring(0,3);
	}
	else
	{
		return word;
	}
}