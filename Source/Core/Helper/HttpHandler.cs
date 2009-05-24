using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.IO.Compression;

namespace Shrinerain.AutoTester.Core
{
    public sealed class HttpHandler
    {
        public static string PostHTML(string url, string postData, Encoding encoding, string cookie)
        {
            if (!String.IsNullOrEmpty(url) && !String.IsNullOrEmpty(postData))
            {
                if (encoding == null)
                {
                    encoding = Encoding.Default;
                }

                try
                {
                    byte[] postByte = encoding.GetBytes(postData);
                    HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
                    myReq.Accept = @"image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/msword, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-silverlight, application/x-shockwave-flash, application/x-silverlight-2-b2, */*";
                    myReq.Referer = url;
                    myReq.KeepAlive = true;
                    myReq.ContentType = @"application/x-www-form-urlencoded";
                    myReq.UserAgent = @"Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; Maxthon; .NET CLR 1.1.4322; .NET CLR 2.0.50215)";
                    myReq.ContentLength = postData.Length;
                    myReq.Method = "POST";
                    if (!String.IsNullOrEmpty(cookie))
                    {
                        myReq.Headers.Add("Cookie", cookie);
                    }
                    myReq.AllowAutoRedirect = true;
                    myReq.Headers.Add("Accept-Language", "en-us");
                    myReq.Headers.Add("UA-CPU", "x86");
                    myReq.Headers.Add("Cache-Control", "no-cache");
                    myReq.Headers.Add("Accept-Encoding", "gzip, deflate");

                    Stream postStream = myReq.GetRequestStream();
                    postStream.Write(postByte, 0, postByte.Length);
                    postStream.Close();

                    HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
                    StreamReader respStream = new StreamReader(new GZipStream(myResp.GetResponseStream(), CompressionMode.Decompress), encoding);
                    string respStr = respStream.ReadToEnd();
                    respStream.Close();

                    return respStr;
                }
                catch
                {
                }
            }

            return null;
        }

        public static string GetHTML(string url)
        {
            return GetHTML(url, null);
        }

        public static string GetHTML(string url, string cookie)
        {
            if (String.IsNullOrEmpty(url))
            {
                return null;
            }

            url = HttpUtility.HtmlDecode(url);

            string html = null;
            string curCookie = null;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                    myRequest.AllowAutoRedirect = true;
                    myRequest.Timeout = 10 * 1000;
                    myRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                    if (!String.IsNullOrEmpty(cookie))
                    {
                        myRequest.Headers.Add("Cookie", cookie);
                    }
                    myRequest.KeepAlive = true;
                    HttpWebResponse myResp = (HttpWebResponse)myRequest.GetResponse();
                    curCookie = myResp.Headers["Set-Cookie"];
                    if (!String.IsNullOrEmpty(curCookie))
                    {
                        curCookie = curCookie.Substring(0, curCookie.IndexOf(";"));
                    }

                    StreamReader respStream = new StreamReader(myResp.GetResponseStream(), Encoding.Default);
                    html = respStream.ReadToEnd();
                    respStream.Close();

                    if (!String.IsNullOrEmpty(html))
                    {
                        break;
                    }
                }
                catch
                {
                    continue;
                }
            }

            if (html.IndexOf("Object moved") > 0)
            {
                int pos1 = html.IndexOf("http://");
                int pos2 = html.IndexOf("\">");
                url = html.Substring(pos1, pos2 - pos1);
                html = GetHTML(url, curCookie);
            }

            return html;
        }
    }
}
