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
using System.IO;
using System.Net;
using System.Drawing;
using System.Threading;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestImage : HTMLTestGUIObject, IClickable, IPicture
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
            : this(element, null)
        {
        }

        public HTMLTestImage(IHTMLElement element, HTMLTestBrowser browser)
            : base(element, browser)
        {
            this._type = HTMLTestObjectType.Image;
            try
            {
                _imgElement = (IHTMLImgElement)element;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not convert to IHTMLImgElement: " + ex.ToString());
            }

            try
            {
                // get the image source
                this._src = _imgElement.src;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not get the source of image: " + ex.ToString());
            }
        }

        #endregion

        #region public methods

        /*  Bitmap GetControlPrint()
         *  for image, we don't need to "capture" it's screen print, just get it's source image.
         */
        public override Bitmap GetControlPrint()
        {
            try
            {
                WebClient webc = new WebClient();
                Stream imgS = webc.OpenRead(this._src);
                return new Bitmap(imgS);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotSaveControlPrintException("Can not get image print: " + ex.ToString());
            }
        }

        #region IClickable Members

        /* void Click()
         * Click on the image.
         */
        public virtual void Click()
        {
            try
            {
                BeforeAction();

                Thread t = new Thread(new ThreadStart(PerformClick));
                t.Start();
                t.Join(ActionTimeout);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not click an image: " + ex.ToString());
            }
            finally
            {
                AfterAction();
            }
        }

        public virtual void DoubleClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void RightClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void MiddleClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IInteractive Members

        public virtual string GetAction()
        {
            return "Click";
        }

        public virtual void DoAction(object para)
        {
            Click();
        }

        #endregion

        public String GetSrc()
        {
            try
            {
                return this._src.Trim();
            }
            catch
            {
            }

            return null;
        }

        public void Download(string des)
        {
            BeforeAction();

            if (String.IsNullOrEmpty(des))
            {
                des = @"C:\" + this._src.Trim();
            }
            WebClient client = new WebClient();
            client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(OnDownloadFileCompleted);
            client.DownloadFileAsync(new System.Uri(this._src), des);
        }

        public override string GetLabel()
        {
            string label;
            if (HTMLTestObject.TryGetProperty(this._sourceElement, "alt", out label) && !String.IsNullOrEmpty(label))
            {
                return label;
            }
            else
            {
                return this._src;
            }
        }
        #endregion

        #region private methods

        protected override void PerformClick()
        {
            Hover();
            if (_sendMsgOnly)
            {
                //we can not click on an image without mouse action.
                //so click the link if have.
                HTMLAnchorElement linkElement = (HTMLAnchorElement)this._sourceElement.parentElement;
                if (linkElement != null)
                {
                    linkElement.click();
                    FireEvent("onclick");
                }
            }
            else
            {
                MouseOp.Click();
            }
        }

        /* private void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
         * callback function when the image is downloaded.
         */
        private void OnDownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            AfterAction();
        }

        #endregion

        #endregion
    }
}
