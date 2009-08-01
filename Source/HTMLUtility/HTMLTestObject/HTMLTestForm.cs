using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestForm : HTMLTestGUIObject, IInteractive
    {
        #region fields

        protected String _method;
        protected String _action;
        protected String _target;

        protected IHTMLFormElement _formElement;

        public String Action
        {
            get { return _action; }
        }

        public String Target
        {
            get { return _target; }
        }

        public String Method
        {
            get { return _method; }
        }

        #endregion

        #region methods

        #region public

        public HTMLTestForm(IHTMLElement element)
            : this(element, null)
        {
        }

        public HTMLTestForm(IHTMLElement element, HTMLTestPage page)
            : base(element, page)
        {
            this._isDelayAfterAction = true;
            this._type = HTMLTestObjectType.Form;
            try
            {
                _formElement = element as IHTMLFormElement;
                _action = _formElement.action;
                _method = _formElement.method;
                _target = _formElement.target;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build test form: " + ex.ToString());
            }
        }

        public void Submit()
        {
            try
            {
                BeforeAction();

                Thread t = new Thread(new ThreadStart(PerformSubmit));
                t.Start();
                t.Join(ActionTimeout);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform submit action: " + ex.ToString());
            }
            finally
            {
                AfterAction();
            }
        }

        public void Reset()
        {
            try
            {
                BeforeAction();

                Thread t = new Thread(new ThreadStart(PerformReset));
                t.Start();
                t.Join(ActionTimeout);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform reset action: " + ex.ToString());
            }
            finally
            {
                AfterAction();
            }
        }

        public override bool IsVisible()
        {
            return false;
        }

        public override bool IsReadyForAction()
        {
            return IsEnable() && IsReadonly() && IsReady();
        }

        #region IInteractive Members

        public string GetAction()
        {
            return "Submit";
        }

        public void DoAction(object parameter)
        {
            Submit();
        }

        #endregion

        #endregion

        #region private

        protected void PerformSubmit()
        {
            this._formElement.submit();
        }

        protected void PerformReset()
        {
            this._formElement.reset();
        }

        #endregion

        #endregion
    }
}
