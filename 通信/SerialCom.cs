using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;

namespace Core
{
    /// <summary>
    /// 读取串口数据
    /// </summary>
    public class SerialCom : SerialPort
    {
        protected Stopwatch sw = new Stopwatch();

        private int _tmout = 200;
        public int TimeOut
        {
            get
            {
                return _tmout;
            }
            set
            {
                ReadTimeout = value;
                _tmout = value;
            }
        }

        public int ReadBuffer(byte[] bt, int nCount)
        {
            sw.Reset();
            sw.Start();
            while (sw.ElapsedMilliseconds < TimeOut && BytesToRead < nCount)
            {
                Thread.Sleep(10);
            }
            sw.Stop();
            return Read(bt, 0, nCount);
        }


        public bool ReadBufferCount(byte[] bt, int nCount)
        {
            if (ReadBuffer(bt, nCount) < nCount)
            {
                Debug.Print("Red:Read=" + BytesToRead.ToString() + ":" + nCount.ToString());
                Debug.Print("Error Waste:" + sw.ElapsedMilliseconds.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 重复读缓存直到出现特定字符或超时
        /// </summary>
        /// <param name="bt">读取结果</param>
        /// <param name="checkBt">结束标志</param>
        /// <param name="len">读取长度</param>
        /// <returns>读取是否成功</returns>
        public bool ReadUntil(byte[] bt, byte[] checkBt,ref int len)
        {
            bool finish = false;
            int iIndex = 0, checkLen = checkBt.Length;
            sw.Reset();
            sw.Start();

            while (sw.ElapsedMilliseconds < TimeOut && !finish)
            {
                if (BytesToRead > 0)
                {
                    iIndex += Read(bt, iIndex, BytesToRead);
                }
                else
                {
                    for (int i = iIndex - 1; i > 0; i--)
                    {
                        if (bt[i] != checkBt[0]) continue;

                        int j = 1;
                        while (j < checkLen && bt[i + j] == checkBt[j]) j++;
                        if (j == checkLen)
                            finish = true;

                        break;
                    }
                    if (!finish)
                        Thread.Sleep(10);
                }
            }

            if (!finish)
            {
                Debug.Print("Red:Read=" + iIndex.ToString() + ":" + Encoding.GetString(bt));
                Debug.Print("Error Waste:" + sw.ElapsedMilliseconds.ToString());
            }
            sw.Stop();

            len = iIndex;
            return finish;
        }

    }
}