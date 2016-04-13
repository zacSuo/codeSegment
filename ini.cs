using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ALDI
{
    /// <summary>
    /// 读写Ini配置文件
    /// copyright:  Zac (suoxd123@126.com)
    /// </summary>
    public class Ini
    {
        /// <summary>
        /// 当前目录下配置文件Config.ini格式
        /// [Info]
        /// username=zac
        /// password=zac321
        /// </summary>
        public void UserCase()
        {
            string fileName = Environment.CurrentDirectory + '\\' + "Config.ini";

            //读
            string userName = ReadIniData("Info", "username", "testName", fileName);
            string password = ReadIniData("Info", "password", "testPwd", fileName);

            //写
            userName = userName + "123";
            WriteIniData("Info", "username", userName, fileName);

        }


        #region API函数声明

        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);


        #endregion

        #region 读写Ini文件

        private static string ReadIniData(string Section, string Key, string NoText, string iniFilePath)
        {
            if (!File.Exists(iniFilePath)) return NoText;

            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(Section, Key, NoText, temp, 1024, iniFilePath);
            return temp.ToString();
        }

        private static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
        {
            if (!File.Exists(iniFilePath)) return false;

            long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
            //string strLog = string.Format("修改功能名称：板号[{0}]-序号[{1}]-名称[{2}],\t结果：{3}", Section, Key, Value, OpStation != 0);
            //Log.WriteLog(strLog);

            return OpStation == 0 ? false : true;
        }

        #endregion

    }
}