using System;
using System.Collections.Generic;
using System.Text;

using XDICTGRB;

namespace Shrinerain.AutoTester.ScreenReader
{
    class Program
    {
        static void Main(string[] args)
        {

            KingTest kt = new KingTest();

            string lastWord = null;

            while (true)
            {
                //System.Threading.Thread.Sleep(1000 * 1);

                if (!String.IsNullOrEmpty(lastWord) && lastWord == kt.WordString)
                {
                    Console.WriteLine(kt.WordString);

                    lastWord = null;
                }

                lastWord = kt.WordString;

            }
        }

    }

    class KingTest : IXDictGrabSink
    {
        string wordString;

        public string WordString
        {
            get { return wordString; }
            set { wordString = value; }
        }

        GrabProxy gp;

        public KingTest()
        {
            gp = new GrabProxy();

            gp.GrabWord(10, 10);

            gp.GrabInterval = 1;//指抓取时间间隔
            gp.GrabMode = XDictGrabModeEnum.XDictGrabMouse;//设定取词的属性
            gp.GrabEnabled = true;//是否取词的属性
            gp.GrabFlag = XDictGrabFlagEnum.XDictGrabOnlyEnglish;
            gp.AdviseGrab(this);

        }


        #region IXDictGrabSink Members

        public int QueryWord(string WordString, int lCursorX, int lCursorY, string SentenceString, ref int lLoc, ref int lStart)
        {
            wordString = SentenceString;

            return 1;
        }

        #endregion
    }
}
