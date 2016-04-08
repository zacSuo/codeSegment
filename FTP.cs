using System;
using System.IO;
using System.Net;

namespace ALDI
{
    /// <summary>
    /// 上传下载FTP文件
    /// copyright:  Zac (suoxd123@126.com)
    /// </summary>
    public class FTP
    {
        private readonly NetworkCredential _credential;
        private readonly string _server;
        private readonly string _ftpFolder;

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

        /// <summary>
        /// 执行上传下载的实体
        /// </summary>
        /// <param name="server">FTP地址(uri)</param>
        /// <param name="useName">登录名</param>
        /// <param name="password">密码</param>
        public FTP(string server, string useName, string password)
        {
            _credential = new NetworkCredential(useName, password);
            _server = "ftp://" + server + '/';
            _ftpFolder = DateTime.Now.ToString("yyyyMMdd");
        }

        /// <summary>
        /// 上传FTP文件
        /// </summary>
        /// <param name="fileName">本地文件名（全路径）</param>
        /// <param name="strError">报错信息</param>
        /// <returns>是否出错</returns>
        public bool UploadFile(string fileName, ref string strError)
        {
            if (!MakeFolderByDate(ref strError)) return false;

            string filePath = _server + _ftpFolder + "/" + Path.GetFileName(fileName);
            FtpWebRequest request =  InitialRequest(filePath,WebRequestMethods.Ftp.UploadFile) ;

            try
            {
                //一种看起来简洁的实现方式，原理是一样的
                //对于文件超过1G在不稳定的网络环境中，没测试过，不知道怎样
                //using (FileStream fs = File.OpenRead(fileName))
                //{
                //    Stream stream = request.GetRequestStream();
                //    //直接将文件流拷贝到FTP数据流
                //    fs.CopyTo(stream);

                //    stream.Close();
                //    fs.Close();
                //}

                //打开要上传的文件流
                using (FileStream fs = File.OpenRead(fileName))
                {
                    //创建FTP写入流
                    Stream stream = request.GetRequestStream();
                    byte[] buffer = new byte[1024];
                    int readIdx;
                    while ((readIdx = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {//将文件按每次1K写入FTP
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

        /// <summary>
        /// 下载FTP文件
        /// </summary>
        /// <param name="fileName">FTP文件名（全路径）</param>
        /// <param name="strError">报错信息</param>
        /// <returns>是否出错</returns>
        public bool DownloadFile(string fileName, ref string strError)
        {
            string filePath = _server + fileName;
            FtpWebRequest request = InitialRequest(filePath, WebRequestMethods.Ftp.DownloadFile);

            try
            {

                //一种看起来简洁的实现方式，原理是一样的
                //对于文件超过1G在不稳定的网络环境中，没测试过，不知道怎样
                //using (FileStream fs = File.Create( Path.GetFileName(fileName)))
                //{
                //    WebResponse response = request.GetResponse();
                //    Stream stream = response.GetResponseStream();
                //    //直接将FTP文件流拷贝到本地文件
                //    stream.CopyTo(fs);

                //    stream.Close();
                //    fs.Close();
                //}

                //创建本地文件
                using (FileStream fs = File.Create(Path.GetFileName(fileName)))
                {
                    //读取FTP文件流
                    WebResponse response = request.GetResponse();
                    Stream stream = response.GetResponseStream();

                    byte[] buffer = new byte[1024];
                    int readIdx;
                    while ((readIdx = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {//将FTP文件流按每次1K写入本地文件
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

        /// <summary>
        /// 在FTP服务器创建文件夹
        /// </summary>
        /// <param name="strError">报错信息</param>
        /// <returns>是否出错</returns>
        private bool MakeFolderByDate(ref string strError)
        {
            bool result = false;
            FtpWebRequest requestMake = null;
            FtpWebRequest requestCheck = InitialRequest(_server, WebRequestMethods.Ftp.ListDirectory);

            try
            {
                //读取FTP服务器的文件目录
                WebResponse response = requestCheck.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string[] rootList = reader.ReadToEnd().Replace("\n", "").Split('\r');
                response.Close();

                bool findFolder = false;
                foreach (string strTmp in rootList)
                {//查看是否有当前要创建的文件夹
                    if (strTmp.Equals(_ftpFolder))
                    {//此处的文件夹默认是用当天日期命名
                        //如果已经存在则标记
                        findFolder = true;
                        break;
                    }
                }

                if (!findFolder)
                {//文件夹不存在则创建
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

        /// <summary>
        /// 初始化FTP网络实体
        /// </summary>
        /// <param name="path">要定位的FTP目录</param>
        /// <param name="method">要执行的操作</param>
        /// <returns>实例化后的实体</returns>
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
    }
}
