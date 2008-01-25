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
*              we can send an email by Outlook or SMTP server.               
*
* History: 2008/01/17 wan,yu Init version
*
*********************************************************************/

//my laptop doesn't have Outlook installed, so I comments these codes when using my laptop
//#define OUTLOOK

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;

#if OUTLOOK
using Outlook = Microsoft.Office.Interop.Outlook;
#endif


namespace Shrinerain.AutoTester.Helper
{
    //we have two types, normal TEXT and HTML.
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

    public sealed class Mail : IDisposable
    {

        #region fields

        //for smtp mail
        private MailMessage _netMail;
        private SmtpClient _smtpServer;
        //default smtp server address
        private string _smtpServerAddr = "localhost";

#if OUTLOOK

        private static Outlook.Application _oApp = new Outlook.Application();
        private static Outlook.NameSpace _oNS = _oApp.GetNamespace("mapi");
        private Outlook._MailItem _oMsg;
        private Outlook.Recipients _oRecips;
        private Outlook.Recipient _oRecip;

        //flag to show whether we have logged to Outlook.
        private static bool _logged = false;
#endif

        //default, we will use Outlook
        private MailServerType _serverType = MailServerType.Outlook;

        //information of a mail.
        private string _subject;
        private string _from;
        private List<string> _to;
        private List<string> _cc;
        private string _body;

        //default mail type is TEXT
        private MailType _mailType = MailType.Text;

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
            }
        }

        public MailServerType ServerType
        {
            get { return _serverType; }
            set { _serverType = value; }
        }

        public string SmtpServerAddr
        {
            get { return _smtpServerAddr; }
            set
            {
                //when changing the smtp server address, we will use this address to create a new smtp server.
                if (!String.IsNullOrEmpty(value) && _serverType == MailServerType.SMTP)
                {
                    _smtpServerAddr = value;

                    try
                    {
                        _smtpServer = new SmtpClient(value);
                    }
                    catch
                    {

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

            Clear();
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
#if OUTLOOK
                    // Log off.
                    _oNS.Logoff();

                    // Clean up.
                    _oRecip = null;
                    _oRecips = null;
                    _oMsg = null;
                    _oNS = null;
                    _oApp = null;
#endif
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
                throw new Exception("Subject, Body and To address can not be empty.");
            }

            try
            {
                //init a new mail.
                CreateNewMail();

                //send a SMTP mail.
                if (_serverType == MailServerType.SMTP)
                {

                    if (String.IsNullOrEmpty(_from) || String.IsNullOrEmpty(_smtpServerAddr))
                    {
                        throw new Exception("From and Mail server can not be empty.");
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

                    _smtpServer.Send(_netMail);

                }
                else
                {
#if OUTLOOK
                    //send a outlook mail.
                    //set title and content
                    _oMsg.Subject = _subject;

                    //if format is HTML, we will try to get the HTML string.
                    if (_mailType == MailType.HTML)
                    {
                        _oMsg.HTMLBody = GetHTML(_body);
                    }
                    else
                    {
                        _oMsg.HTMLBody = _body;
                    }

                    _oRecips = (Outlook.Recipients)_oMsg.Recipients;

                    // add each To address
                    foreach (string to in _to)
                    {
                        _oRecip = (Outlook.Recipient)_oRecips.Add(to);
                        _oRecip.Resolve();
                    }

                    _oMsg.Send();
#endif
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Can not send email: " + ex.Message);
            }
            finally
            {
                Clear();
            }
        }

        #endregion

        #region private methods

        /* void CreateNewMail()
         * Init parameters, prepare to send a new mail.
         */
        private void CreateNewMail()
        {
            try
            {
                if (this._serverType == MailServerType.SMTP)
                {
                    _netMail = new MailMessage();

                    _netMail.IsBodyHtml = _mailType == MailType.HTML;

                    if (_smtpServer == null)
                    {
                        _smtpServer = new SmtpClient(_smtpServerAddr);
                    }

                }
                else
                {
#if OUTLOOK
                    if (!_logged)
                    {
                        _oNS.Logon(null, null, false, false);
                        _logged = true;
                    }

                    _oMsg = (Outlook.MailItem)_oApp.CreateItem(Outlook.OlItemType.olMailItem);
#endif
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Can not create a new mail: " + ex.Message);
            }

        }

        /* void Clear()
         * clean all information .
         */
        private void Clear()
        {
            try
            {
                _subject = null;
                _from = null;
                _body = null;

                _to.Clear();
                _cc.Clear();
            }
            catch
            {

            }
        }

        /* string GetHTML(string url)
         * return the string of expected url.
         */
        private static string GetHTML(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new Exception("Url can not be empty.");
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