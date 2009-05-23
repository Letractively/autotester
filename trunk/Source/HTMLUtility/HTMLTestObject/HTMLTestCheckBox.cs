/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestCheckBox.cs
*
* Description: This class defines the actions provide by CheckBox.
*              The important actions include "Check","UnCheck".
* 
* History: 2007/09/04 wan,yu Init version
*          2008/01/24 wan,yu update, add GetLabelForCheckBox(); 
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestCheckBox : HTMLTestGUIObject, ICheckable, IText, IStatus
    {
        #region fields

        //the HTML element of HTMLTestCheckBox.
        protected IHTMLInputElement _checkBoxElement;

        #endregion

        #region properties

        #endregion

        #region methods

        #region ctor

        public HTMLTestCheckBox(IHTMLElement element)
            : this(element, null)
        {
        }

        public HTMLTestCheckBox(IHTMLElement element, HTMLTestBrowser browser)
            : base(element, browser)
        {
            this._type = HTMLTestObjectType.CheckBox;
            try
            {
                this._checkBoxElement = (IHTMLInputElement)element;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not get IHTMLInputElement: " + ex.ToString());
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
                throw new CannotPerformActionException("Can not perform Check action on checkbox: " + ex.ToString());
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
                throw new CannotPerformActionException("Can not perform UnCheck action on checkbox: " + ex.ToString());
            }
        }

        public virtual bool IsChecked()
        {
            try
            {
                return _checkBoxElement.@checked;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not get status of checkbox: " + ex.ToString());
            }
        }

        #endregion

        #region IClickable Members

        public virtual void Click()
        {
            try
            {
                BeforeAction();

                Hover();
                if (_sendMsgOnly)
                {
                    this._checkBoxElement.@checked = !this._checkBoxElement.@checked;
                }
                else
                {
                    MouseOp.Click();
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not click on the checkbox: " + ex.ToString());
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

        public override string GetLabel()
        {
            return GetLabelForCheckBox(this._sourceElement);
        }

        /* string GetLabelForCheckBox(IHTMLElement element)
         * return the text around check box, firstly we will try current cell, then try right cell and up cell.
         */
        public static string GetLabelForCheckBox(IHTMLElement element)
        {
            try
            {
                //firstly, try to get text in the same cell/span/div/label
                string label = GetAroundText(element);

                //for checkbox, we think the text on the right is it's label.
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

        #endregion

        #endregion
    }
}
