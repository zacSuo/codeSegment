using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace SimuProteus
{
    /// <summary>
    /// ��дIni�����ļ�
    /// copyright:  Zac (suoxd123@126.com)
    /// </summary>
    public class Ini
    {
        private static string fileName = Constants.CurrentDirectory + '\\' + "Config.ini";
        
        public static string GetItemValue(string strSection, string strKey)
        {
            return ReadIniData(strSection, strKey, "", fileName);
        }

        public static bool SetItemValue(string strSection, string strKey, string strValue)
        {
            return WriteIniData(strSection, strKey, strValue, fileName);
        }

        #region API��������

        [DllImport("kernel32")]//����0��ʾʧ�ܣ���0Ϊ�ɹ�
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        [DllImport("kernel32")]//����ȡ���ַ����������ĳ���
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);


        #endregion

        #region ��дIni�ļ�

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

            return OpStation == 0 ? false : true;
        }

        #endregion

    }
}