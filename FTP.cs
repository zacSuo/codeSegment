using System;
using System.IO;
using System.Net;

namespace ALDI.Core
{
    /// <summary>
    /// 上传下载FTP文件
    /// copyright:  zac (suoxd123@126.com)
    /// </summary>
    public class FTP
    {
        private readonly NetworkCredential _credential;
        private readonly string _server;
        private readonly string _ftpFolder;

        public FTP(string server, string useName, string password)
        {
            _credential = new NetworkCredential(useName, password);
            _server = "ftp://" + server + '/';
            _ftpFolder = DateTime.Now.ToString("yyyyMMdd");
        }

        public bool UploadFile(string fileName, ref string strError)
        {
            if (!MakeFolderByDate(ref strError)) return false;

            string filePath = _server + _ftpFolder + "/" + Path.GetFileName(fileName);
            FtpWebRequest request =  InitialRequest(filePath,WebRequestMethods.Ftp.UploadFile) ;

            try
            {
                //using (FileStream fs = File.OpenRead(fileName))
                //{
                //    Stream stream = request.GetRequestStream();
                //    fs.CopyTo(stream);

                //    stream.Close();
                //    fs.Close();
                //}

                using (FileStream fs = File.OpenRead(fileName))
                {
                    Stream stream = request.GetRequestStream();
                    byte[] buffer = new byte[1024];
                    int readIdx;
                    while ((readIdx = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, readIdx);
                    }

                    stream.Close();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return false;
            }
            finally
            {
                request.Abort();
            }

            return true;
        }

        public bool DownloadFile(string fileName, ref string strError)
        {
            string filePath = _server + fileName;
            FtpWebRequest request = InitialRequest(filePath, WebRequestMethods.Ftp.DownloadFile);

            try
            {

                //using (FileStream fs = File.Create( Path.GetFileName(fileName)))
                //{
                //    WebResponse response = request.GetResponse();
                //    Stream stream = response.GetResponseStream();
                //    stream.CopyTo(fs);

                //    stream.Close();
                //    fs.Close();
                //}

                using (FileStream fs = File.Create(Path.GetFileName(fileName)))
                {
                    WebResponse response = request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    byte[] buffer = new byte[1024];
                    int readIdx;
                    while ((readIdx = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, readIdx);
                    }

                    stream.Close();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return false;
            }
            finally
            {
                request.Abort();
            }

            return true;
        }

        private bool MakeFolderByDate(ref string strError)
        {
            bool result = false;
            FtpWebRequest requestMake = null;
            FtpWebRequest requestCheck = InitialRequest(_server, WebRequestMethods.Ftp.ListDirectory);

            try
            {
                WebResponse response = requestCheck.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string[] rootList = reader.ReadToEnd().Replace("\n", "").Split('\r');
                response.Close();

                bool findFolder = false;
                foreach (string strTmp in rootList)
                {
                    if (strTmp.Equals(_ftpFolder))
                    {
                        findFolder = true;
                        break;
                    }
                }

                if (!findFolder)
                {
                    requestMake = InitialRequest(_server + _ftpFolder, WebRequestMethods.Ftp.MakeDirectory);
                    response = requestMake.GetResponse();
                    response.Close();
                }
                result = true;
            }
            catch (System.Exception ex)
            {
                strError = ex.Message;
            }
            finally
            {
                requestCheck.Abort();
                if (requestMake != null) 
                    requestMake.Abort();
            }
            return result;
        }

        private FtpWebRequest InitialRequest(string path, string method)
        {

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);

            request.Method = method;
            request.Credentials = _credential;
            request.KeepAlive = true;
            request.EnableSsl = false;
            request.Proxy = null;
            request.Timeout = 120000;
            request.ReadWriteTimeout = 120000;

            return request;
        }

        /// <summary>
        /// 使用示例说明
        /// </summary>
        private void UserCase()
        {
            string strError = string.Empty;
            FTP ftp = new FTP("192.168.10.195", "suoxdftp", "suoxdftp");
            if (!ftp.UploadFile("F:\\VA_X_Setup.zip", ref strError)) Console.WriteLine(strError);

            if (!ftp.DownloadFile("20160408\\VA_X_Setup.zip", ref strError)) Console.WriteLine(strError);
        }
    }
}
