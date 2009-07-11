/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestRadioButton.cs
*
* Description: This class defines the actions provide by Radio Button.
*              The important actions include "Check" , and "IsChecked".
*
* History: 2007/09/04 wan,yu Init version.
*          2008/01/24 wan,yu update, GetLabelForRadioBox(). 
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestRadioBox : HTMLTestGUIObject, ICheckable, IText
    {
        #region fields

        protected IHTMLInputElement _radioElement;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public HTMLTestRadioBox(IHTMLElement element)
            : this(element, null)
        {
        }

        public HTMLTestRadioBox(IHTMLElement element, HTMLTestBrowser browser)
            : base(element, browser)
        {
            this._type = HTMLTestObjectTypeEnum.RadioBox;
            try
            {
                this._radioElement = (IHTMLInputElement)element;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not convert to IHTMLInputElement: " + ex.ToString());
            }
        }

        #endregion

        #region public methods

        public override List<TestProperty> GetIdenProperties()
        {
            List<TestProperty> properties = base.GetIdenProperties();
            properties.Add(new TestProperty(TestConstants.PROPERTY_LABEL, GetLabel()));
            properties.Add(new TestProperty(TestConstants.PROPERTY_ISCHECKED, IsChecked().ToString()));
            return properties;
        }

        #region ICheckable Members

        public virtual void Check()
        {
            try
            {
                if (!IsChecked())
                {
                    Click();
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform Check action on radio button: " + ex.ToString());
            }
        }

        public virtual void UnCheck()
        {
            try
            {
                if (IsChecked())
                {
                    Click();
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform UnCheck action on radio button: " + ex.ToString());
            }
        }

        public virtual bool IsChecked()
        {
            try
            {
                return _radioElement.@checked;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not get status of radio button: " + ex.ToString());
            }
        }

        #endregion

        #region IClickable Members

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
                throw new CannotPerformActionException("Can not click on the radio button: " + ex.ToString());
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
            return "Check";
        }

        public virtual void DoAction(object para)
        {
            Check();
        }

        #endregion

        #region IText Members

        public virtual string GetText()
        {
            return GetLabel();
        }

        /* string GetAroundText()
         * return the text around the radio button.
         */
        public override string GetLabel()
        {
            return GetLabelForRadioBox(this._sourceElement);
        }

        public virtual string GetFontFamily()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual string GetFontSize()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual string GetFontColor()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /* string GetLabelForRadioBox(IHTMLElement element)
         * return the text around radio button, firstly we will try current cell, then try right cell and up cell.
         */
        public static string GetLabelForRadioBox(IHTMLElement element)
        {
            try
            {
                //firstly, try to get text in the same cell/span/div/label
                string label = GetAroundText(element);

                //for radio button, we think the text on the right is it's label.
                if (!String.IsNullOrEmpty(label))
                {
                    if (label.Split(new string[] { _labelSplitter }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                    {
                        return label.Split(new string[] { _labelSplitter }, StringSplitOptions.RemoveEmptyEntries)[1];
                    }
                    else if (label.Split(new string[] { _labelSplitter }, StringSplitOptions.RemoveEmptyEntries).Length > 0)
                    {
                        return label.Split(new string[] { _labelSplitter }, StringSplitOptions.RemoveEmptyEntries)[0];
                    }
                }

                //we will search right cell and up cell.
                int[] searchDirs = new int[] { 1, 3, 0 };
                foreach (int currentDir in searchDirs)
                {
                    for (int deepth = 1; deepth < 4; deepth++)
                    {
                        label = GetAroundCellText(element, currentDir, deepth);
                        if (!String.IsNullOrEmpty(label.Trim()))
                        {
                            return label;//.Split(' ')[0];
                        }
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region private

        protected override void PerformClick()
        {
            Hover();
            if (_sendMsgOnly)
            {
                this._radioElement.@checked = true;
                FireEvent("onclick");
            }
            else
            {
                MouseOp.Click();
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
