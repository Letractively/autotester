#pragma once

using namespace System;

namespace Shrinerain
{
	namespace AutoTester
	{
		namespace AIUtility
		{
			public ref class TextHelper
			{
			private:
				static initonly int _defaultPercent=70;
			public:
				static int CalSimilarPercent(String^ str1, String^ str2);
				static int CalSimilarPercent(String^ str1, String^ str2, bool ignoreCase, bool compressBlank);
				static int CalStyleSimPercent(String^ str1, String^ str2);
			};

			public enum CharClass
			{
				Alpha,
				Number,
				Brackets,
				Punctuation,
				Special,
				Compute,
				Chinese,
				Other
			};

			public value class WordStyle
			{
			public:
				CharClass _charClass;
				int _percent;
				int _pos;
			};

			public value class SentenceStyle
			{
			public:
				array<WordStyle>^ _sentenceSytle;
			};
		}
	}
}