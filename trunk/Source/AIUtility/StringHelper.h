#pragma once

using namespace System;

namespace Shrinerain
{
	namespace AutoTester
	{
		namespace AIUtility
		{
			public ref class StringHelper
			{
			private:
				static initonly int _defaultPercent=70;
			public:
				static int CalSimilarPercent(String^ str1, String^ str2);
				static int CalSimilarPercent(String^ str1, String^ str2, bool ignoreCase, bool compressBlank);
			};
		}
	}
}