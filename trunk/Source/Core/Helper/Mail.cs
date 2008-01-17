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
*
* History: 2008/01/17 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

using Outlook = Microsoft.Office.Interop.Outlook;


namespace Shrinerain.AutoTester.Core
{
    public class Mail
    {

    }

    //OutlookMail will use Outlook com to send a mail.
    public class OutlookMail : IDisposable
    {
        private static Outlook.Application oApp;
        private static Outlook.NameSpace oNS;
        private static Outlook._MailItem oMsg;
        private static Outlook.Recipients oRecips;
        private static Outlook.Recipient oRecip;

        private static string _title = "";
        private static List<string> _mailAddrs = new List<string>(8);
        private static string _mailContent = "";


        public String Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public String Body
        {
            get { return _mailContent; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    string content = value.Trim();

                    //if the body starts with http:// or https://, we think it is the url for HTML mail. 
                    if (content.StartsWith("Http://", StringComparison.CurrentCultureIgnoreCase) ||
                        content.StartsWith("Https://", StringComparison.CurrentCultureIgnoreCase)
                        )
                    {
                        content = GetHTMLString(content);
                    }

                    _mailContent = content;

                }

            }
        }


        static OutlookMail()
        {
            try
            {
                //create outlook
                oApp = new Outlook.Application();
                oNS = oApp.GetNamespace("mapi");

                //logon
                oNS.Logon(null, null, false, false);
            }
            catch (Exception ex)
            {
                throw new CannotSendMailExpcetion("Can not attach Outlook: " + ex.Message);
            }

        }

        public void Dispose()
        {
            Close();
        }

        public void CreateNewMail()
        {
            try
            {
                oMsg = (Outlook.MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);
                _mailAddrs.Clear();
            }
            catch (Exception ex)
            {
                throw new CannotSendMailExpcetion("Can not create a new mail: " + ex.Message);
            }

        }

        public void AddMailAddr(string mailAddr)
        {
            if (!String.IsNullOrEmpty(mailAddr) && !_mailAddrs.Contains(mailAddr))
            {
                _mailAddrs.Add(mailAddr);
            }
        }

        public void Send()
        {
            if (String.IsNullOrEmpty(_title) || String.IsNullOrEmpty(_mailContent) || _mailAddrs.Count == 0)
            {
                throw new CannotSendMailExpcetion("Title, Body, To address can not be empty.");
            }

            try
            {
                if (oMsg == null)
                {
                    CreateNewMail();
                }

                //set title and content
                oMsg.Subject = _title;
                oMsg.HTMLBody = _mailContent;

                oRecips = (Outlook.Recipients)oMsg.Recipients;

                // add each To address
                foreach (string addr in _mailAddrs)
                {
                    oRecip = (Outlook.Recipient)oRecips.Add(addr);
                    oRecip.Resolve();

                }

                oMsg.Send();

            }
            catch (Exception ex)
            {
                throw new CannotSendMailExpcetion("Can not send a mail: " + ex.Message);
            }
        }


        public void Close()
        {
            try
            {
                if (oRecip != null)
                {
                    // Log off.
                    oNS.Logoff();

                    // Clean up.
                    oRecip = null;
                    oRecips = null;
                    oMsg = null;
                    oNS = null;
                    oApp = null;
                }
            }
            catch
            {
            }
        }

        /* string GetHTMLString(string url)
         * return the string of expected url.
         */
        private static string GetHTMLString(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new CannotSendMailExpcetion("Url can not be empty.");
            }
            try
            {
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;

                String html = webClient.DownloadString(url);

                int viewStatePosStart = html.IndexOf("__VIEWSTATE") - 27;

                //remove view state, make the mail smaller
                if (viewStatePosStart > 0)
                {
                    int viewStatePosEnd = html.IndexOf("/>", viewStatePosStart) + 2;
                    html = html.Remove(viewStatePosStart, viewStatePosEnd - viewStatePosStart);
                }

                return html;
            }
            catch (Exception ex)
            {
                throw new CannotSendMailExpcetion("Can not get HTML from " + url + ": " + ex.Message);
            }

        }
    }
}