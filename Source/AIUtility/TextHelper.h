#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace Shrinerain
{
	namespace AutoTester
	{
		namespace AIUtility
		{
			public enum class CharClass
			{
				Alpha,
				Number,
				Space,
				Tab,
				NewLine,
				Brackets,
				Punctuation,
				Operator,
				Special,
				Chinese,
				Empty,
				Other
			};

			public enum class WordClass
			{
				Noun,
				Adj,
				Adv,
				Num,
				Conj
			};

			public value class WordStyle
			{
			public:
				CharClass _charClass;
				int _unicode;
				int _length;
				int _pos;
			};

			public value class SentenceStyle
			{
			public:
				array<WordStyle>^ _sentenceSytle;
			};

			public ref class TextHelper
			{
			private:
				static Dictionary<String^,int>^ _cache=gcnew  Dictionary<String^,int>();
				static initonly String^ _keySP="__WY__";
				static initonly int _defaultPercent=70;
			public:
				static CharClass GetCharClass(char ch);
				static array<String^>^ SplitWords(String^ text);
				static int CalSimilarPercent(String^ str1, String^ str2);
				static int CalSimilarPercent(String^ str1, String^ str2, bool ignoreCase, bool ignoreBlank);
				static int CalStyleSimPercent(String^ str1, String^ str2);
				static String^ GetShortWord(String^ word);

			};

		}
	}
}