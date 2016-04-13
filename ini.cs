using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ALDI
{
    /// <summary>
    /// ��дIni�����ļ�
    /// copyright:  Zac (suoxd123@126.com)
    /// </summary>
    public class Ini
    {   
    /// <summary>
    /// [Info]
    /// username=zac
    /// password=zac321
    /// </summary>
        public void UserCase(){
            string fileName = Environment.CurrentDirectory + '\\' + "Config.ini";

            //��
            string userName=ReadIniData("Info","username","testName",fileName);
            string password = ReadIniData("Info","password","testPwd",fileName);

            //д
            userName = userName + "123";
            WriteIniData("Info","username",userName,fileName);

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
            //string strLog = string.Format("�޸Ĺ������ƣ����[{0}]-���[{1}]-����[{2}],\t�����{3}", Section, Key, Value, OpStation != 0);
            //Log.WriteLog(strLog);

            return OpStation == 0 ? false : true;
        }

        #endregion

    }