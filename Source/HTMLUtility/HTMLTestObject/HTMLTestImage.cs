/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestImage.cs
*
* Description: This class defines the actions provide by Image.
*              The important actions include "Download" and "Click". 
*
* History: 2007/09/04 wan,yu Init version
*          2008/01/10 wan,yu update, when downloading an image, we don't
*                                    have to wait for this action. 
*          2008/01/12 wan,yu update, add two methods, DownloadImageSync
*                                    and DownloadImageAsync          
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using mshtml;

using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestImage : HTMLGuiTestObject, IClickable
    {

        #region fields

        // the src of the image, eg: http://www.google.com/111.jpg
        protected string _src;

        //source HTML element
        protected IHTMLImgElement _imgElement;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public HTMLTestImage(IHTMLElement element)
            : base(element)
        {

            this._type = HTMLTestObjectType.Image;

            try
            {
                _imgElement = (IHTMLImgElement)element;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not convert to IHTMLImgElement: " + e.Message);
            }

            try
            {
                // get the image source
                this._src = _imgElement.src;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not get the source of image: " + e.Message);
            }
        }

        #endregion

        #region public methods

        /* void DownloadImageSync(string des)
         * Download source image, wait until the image is downloaded.
         */
        public void DownloadImageSync(string des)
        {
            try
            {
                DowloadImage(des);

                _actionFinished.WaitOne();

            }
            catch (Exception e)
            {
                throw new CannotPerformActionException("Can not download image: " + e.Message);
            }
        }

        /* void DownloadImageAsync(string des)
         * Download the image, DO NOT wait.
         */
        public void DownloadImageAsync(string des)
        {
            try
            {
                DowloadImage(des);
            }
            catch (Exception e)
            {
                throw new CannotPerformActionException("Can not download image: " + e.Message);
            }
        }

        #region IClickable Members

        /* void Click()
         * Click on the image.
         */
        public void Click()
        {
            try
            {
                _actionFinished.WaitOne();

                Hover();

                MouseOp.Click();

                _actionFinished.Set();
            }
            catch (Exception e)
            {
                throw new CannotPerformActionException("Can not click an image: " + e.Message);
            }
        }

        public void DoubleClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RightClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void MiddleClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IInteractive Members

        public void Focus()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object GetDefaultAction()
        {
            return "Click";
        }

        public void PerformDefaultAction(object para)
        {
            Click();
        }

        #endregion

        #endregion

        #region private methods

        /* void DowloadImage(string des)
         * use web client to download the image.
         * web client often NO use if cookie needed.
         */
        private void DowloadImage(string des)
        {
            if (String.IsNullOrEmpty(des))
            {
                des = @"C:\" + this._src.Trim();
            }

            WebClient client = new WebClient();
            client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(client_DownloadFileCompleted);

            client.DownloadFileAsync(new System.Uri(this._src), des);
        }

        /* private void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
         * callback function when the image is downloaded.
         */
        private void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            _actionFinished.Set();
        }

        #endregion

        #endregion

    }
}
