using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public interface IPicture : IVisible
    {
        void Click();
        String GetSrc();
        void Download(string path);
    }
}
