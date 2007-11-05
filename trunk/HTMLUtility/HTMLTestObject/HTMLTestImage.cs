using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using mshtml;

using Shrinerain.AutoTester.Function;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestImage : HTMLGuiTestObject
    {

        #region fields

        protected string _src;

        protected IHTMLImgElement _imgElement;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public HTMLTestImage(IHTMLElement element)
            : base(element)
        {
            try
            {
                _imgElement = (IHTMLImgElement)element;
            }
            catch (Exception e)
            {
                throw new CanNotBuildObjectException("Can not convert to IHTMLImgElement: " + e.Message);
            }

            try
            {
                this._src = _imgElement.src;
            }
            catch (Exception e)
            {
                throw new CanNotBuildObjectException("Can not get SRC of image: " + e.Message);
            }
        }

        #endregion

        #region public methods

        public void DownloadImage(string des)
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(client_DownloadFileCompleted);
                
                client.DownloadFileAsync(this._src, des);

                _actionFinished.WaitOne();

            }
            catch (Exception e)
            {
                _actionFinished.Set();

                throw new CanNotPerformActionException("Can not download image: " + e.Message);
            }
        }

        #endregion

        #region private methods

        private void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            _actionFinished.Set();
        }

        #endregion

        #endregion

    }
}
