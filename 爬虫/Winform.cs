using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace web2Excel
{
    class WebInfo
    {
        /// <summary>
        /// 每个页面读取后休息时间（1s）
        /// </summary>
        public static float SleepOnePage = 3;

        /// <summary>
        /// Get网页信息（UTF8编码）
        /// </summary>
        /// <param name="strUrl">网页地址</param>
        /// <returns>网页内容</returns>
        public static string GetPageInfo(string strUrl)
        {
            return GetPageInfo(strUrl, Encoding.UTF8);
        }
        /// <summary>
        /// Get网页信息
        /// </summary>
        /// <param name="strUrl">网页地址</param>
        /// <param name="codeType">文字编码</param>
        /// <returns>网页内容</returns>
        public static string GetPageInfo(string strUrl, Encoding codeType)
        {
            System.Net.HttpWebRequest request;
            // 创建一个HTTP请求
            request = (System.Net.HttpWebRequest)WebRequest.Create(strUrl);
            request.Method = "get";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/7.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.3; .NET4.0C; .NET4.0E)";
            request.Accept = "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, */*";

            ServicePointManager.ServerCertificateValidationCallback = ValidateServiceCertificate;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), codeType);
            string responseText = myreader.ReadToEnd();
            myreader.Close();

            System.Threading.Thread.Sleep((int)(SleepOnePage * 1000));
            return responseText;

        }

        /// <summary>
        /// 默认通过证书
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyError"></param>
        /// <returns></returns>
        private static bool ValidateServiceCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyError)
        {
            return true;
        }

        /// <summary>
        ///  Post 获取网页（UTF8编码）
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string PostPageInfo(string strUrl, IDictionary<string, string> parameters)
        {
            return PostPageInfo(strUrl, parameters, Encoding.UTF8);
        }

        /// <summary>
        /// Post 获取网页
        /// </summary>
        /// <param name="strUrl">网页地址</param>
        /// <param name="parameters">Post参数</param>
        /// <param name="codeType">文字编码</param>
        /// <returns></returns>
        public static string PostPageInfo(string strUrl,IDictionary<string,string> parameters, Encoding codeType)
        {
            System.Net.HttpWebRequest request;
            // 创建一个HTTP请求
            request = (System.Net.HttpWebRequest)WebRequest.Create(strUrl);
            request.Method = "post";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/7.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.3; .NET4.0C; .NET4.0E)";
            request.Accept = "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, */*";
            request.ContentType = "application/x-www-form-urlencoded";
            ServicePointManager.ServerCertificateValidationCallback = ValidateServiceCertificate;

            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                byte[] data = codeType.GetBytes(buffer.ToString());
                //byte[] data = codeType.GetBytes("__EVENTTARGET=ctl00%24ContentPlaceHolder1%24lnkBtnNext&__EVENTARGUMENT=&__VIEWSTATE=%2fwEPDwULLTIwNDA3MjA1NjEPFgYeCFNRTHdoZXJlZR4JUGFnZUNvdW50AlQeCVBhZ2VJbmRleAIBFgJmD2QWAgIDD2QWAgIDD2QWAgIRDw8WAh4EVGV4dAUu56ysMemhtS%2flhbE4NOmhtSzlhbE4NDDmnaHorrDlvZXvvIzmr4%2fpobUxMOadoWRkGAIFHl9fQ29udHJvbHNSZXF1aXJlUG9zdEJhY2tLZXlfXxYCBSZjdGwwMCRDb250ZW50UGxhY2VIb2xkZXIxJEltZ0J0blNlYXJjaAUiY3RsMDAkQ29udGVudFBsYWNlSG9sZGVyMSRJbWdidG5HbwUuY3RsMDAkQ29udGVudFBsYWNlSG9sZGVyMSRHcmlkVmlld1Byb2plY3RRdWVyeQ88KwAKAQgCAWSxTYsY8owvvZoc7TfqZh%2byMIE9tQ%3d%3d&__EVENTVALIDATION=%2fwEWDAKLzLXzBQL0jYbCAgKE%2bY66BwK5sODPAwLTyqyUCgL0j4yvDQKp8KfXDQLbpd2VCwLWl7irCAKJqeahCwKX1%2fTcAQLvn7%2fuCC4ZwVyjJND9DI4p2vLIyobfXmmd");
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            StreamReader myreader = new StreamReader(response.GetResponseStream(), codeType);
            string responseText = myreader.ReadToEnd();
            myreader.Close();

            System.Threading.Thread.Sleep((int)(SleepOnePage * 1000));
            return responseText;
        }
    }
}