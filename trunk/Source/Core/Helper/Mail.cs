/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: Mail.cs
*
* Description: This compent provides functions to send an email.
*              After testing, we often need to send test result by email. 
*              we can send a mail by Outlook or SMTP server.               
*
* History: 2008/01/17 wan,yu Init version
*
*********************************************************************/

#undef OUTLOOK

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;

#if OUTLOOK
using Outlook = Microsoft.Office.Interop.Outlook;
#endif


namespace Shrinerain.AutoTester.Core
{
    public enum MailType : int
    {
        Text = 0,
        HTML
    }

    //use smtp or outlook to send a mail.
    public enum MailServerType : int
    {
        SMTP = 0,
        Outlook
    }

    public class Mail : IDisposable
    {

        #region fields

        //for smtp mail
        protected MailMessage _netMail;
        protected SmtpClient _smtpServer;
        //default mail server
        protected string _mailServer = "localhost";

#if OUTLOOK

#endif

        protected MailServerType _serverType = MailServerType.SMTP;

        //fields for a mail.
        protected string _subject;
        protected string _from;
        protected List<string> _to;
        protected List<string> _cc;
        protected string _body;

        //mail type
        protected MailType _mailType = MailType.Text;

        #endregion

        #region properties

        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        public string From
        {
            get { return _from; }
            set { _from = value; }
        }

        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        public MailType Type
        {
            get { return _mailType; }
            set
            {
                _mailType = value;
                if (value == MailType.HTML)
                {
                    if (_serverType == MailServerType.SMTP)
                    {
                        _netMail.IsBodyHtml = true;
                    }
                    else
                    {

                    }
                }
            }
        }

        public MailServerType ServerType
        {
            get { return _serverType; }
            set { _serverType = value; }
        }

        public string MailServer
        {
            get { return _mailServer; }
            set
            {
                if (!String.IsNullOrEmpty(value) && _serverType == MailServerType.SMTP)
                {
                    _mailServer = value;

                    try
                    {
                        _smtpServer = new SmtpClient(value);
                    }
                    catch (Exception ex)
                    {
                        throw new CannotSendMailExpcetion("Can not start smtp server: " + ex.Message);
                    }
                }
            }
        }



        #endregion

        #region methods

        #region ctor

        public Mail()
        {
            _to = new List<string>();
            _cc = new List<string>();
        }

        public void Dispose()
        {
            try
            {
                if (_serverType == MailServerType.SMTP)
                {
                    if (_netMail != null)
                    {
                        _netMail.Dispose();
                        _netMail = null;
                    }
                }
                else
                {

                }

                GC.Collect();

                GC.SuppressFinalize(this);
            }
            catch
            {
            }

        }

        #endregion

        #region public methods

        /* void CreateNewMail()
         * Init parameters, prepare to send a new mail.
         */
        public void CreateNewMail()
        {
            if (this._serverType == MailServerType.SMTP)
            {
                _netMail = new MailMessage();
                _netMail.IsBodyHtml = false;
            }
            else
            {
                //outlook
            }

            _subject = null;
            _from = null;
            _body = null;
            _to.Clear();
            _cc.Clear();
        }

        /* void AddToAddress(string addr)
         * add "to" mail address 
         */
        public void AddToAddress(string addr)
        {
            if (!String.IsNullOrEmpty(addr) && !_to.Contains(addr))
            {
                _to.Add(addr);
            }
        }

        /*  void AddCCAddress(string addr)
         *  add "CC" mail address
         */
        public void AddCCAddress(string addr)
        {
            if (!String.IsNullOrEmpty(addr) && !_cc.Contains(addr))
            {
                _cc.Add(addr);
            }
        }

        /* void Send()
         * send a mail.
         */
        public void Send()
        {

            //check information
            if (String.IsNullOrEmpty(_subject) || String.IsNullOrEmpty(_body)
                || _to.Count == 0)
            {
                throw new CannotSendMailExpcetion("Subject, Body and To address can not be empty.");
            }

            try
            {
                if (_serverType == MailServerType.SMTP)
                {
                    //send a SMTP mail.
                    if (String.IsNullOrEmpty(_from) || String.IsNullOrEmpty(_mailServer))
                    {
                        throw new CannotSendMailExpcetion("From and Mail server can not be empty.");
                    }

                    _netMail.Subject = _subject;
                    _netMail.From = new MailAddress(_from);

                    //if format is HTML, we will try to get the HTML string.
                    if (_mailType == MailType.HTML)
                    {
                        _netMail.Body = GetHTML(_body);
                    }
                    else
                    {
                        _netMail.Body = _body;
                    }

                    //add TO address
                    _netMail.To.Clear();
                    foreach (string to in _to)
                    {
                        _netMail.To.Add(to);
                    }

                    //add CC address
                    _netMail.CC.Clear();
                    foreach (string cc in _cc)
                    {
                        _netMail.CC.Add(cc);
                    }

                    if (_smtpServer == null)
                    {
                        _smtpServer = new SmtpClient(_mailServer);
                    }

                    _smtpServer.Send(_netMail);
                }
                else
                {
                    //send a outlook mail.
                }
            }
            catch (Exception ex)
            {
                throw new CannotSendMailExpcetion("Can not send email: " + ex.Message);
            }
        }

        #endregion

        #region private methods

        /* string GetHTML(string url)
         * return the string of expected url.
         */
        private static string GetHTML(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new CannotSendMailExpcetion("Url can not be empty.");
            }
            try
            {
                WebClient webClient = new WebClient();

                //webClient.Encoding = Encoding.UTF8;

                String html = webClient.DownloadString(url);

                int viewStatePosStart = html.IndexOf("__VIEWSTATE") - 27;

                //remove view state of Asp.net page, make the mail smaller
                if (viewStatePosStart > 0)
                {
                    int viewStatePosEnd = html.IndexOf("/>", viewStatePosStart) + 2;
                    html = html.Remove(viewStatePosStart, viewStatePosEnd - viewStatePosStart);
                }

                return html;
            }
            catch
            {
                return url;
            }

        }

        #endregion

        #endregion

    }
}