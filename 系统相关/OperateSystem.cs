using System;
using System.Diagnostics;

namespace Core
{
    public class OperateSystem
    {
        /// <summary>
        /// 当前系统版本
        /// </summary>
        public static bool Is64Bits
        {
            get
            {
                return Environment.Is64BitOperatingSystem;
            }
        }

        [DllImport("user32.dll", EntryPoint = "ExitWindowsEx", CharSet = CharSet.Ansi)]
        private static extern int ExitWindowsEx(int uFlags, int dwReserved);

        /// <summary>
        /// 执行关机操作
        /// </summary>
        public static void ShutDown()
        {
            Process myProcess = new Process();
            myProcess.StartInfo.FileName = "cmd.exe";
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.RedirectStandardInput = true;
            myProcess.StartInfo.RedirectStandardOutput = true;
            myProcess.StartInfo.RedirectStandardError = true;
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.Start();
            myProcess.StandardInput.WriteLine("shutdown -s -t 0");
        }

        /// <summary>
        /// 执行第三方应用程序
        /// </summary>
        /// <param name="proFullPath">程序的绝对路径</param>
        /// <param name="proParams">程序输入参数</param>
        /// <param name="proNeedShow">是否需要显示程序执行过程</param>
        /// <param name="handlerAfterFinish">完成执行后程序需要做的事情</param>
        public static void ExecuteProcedure(string proFullPath,string proParams, bool proNeedShow, EventHandler handlerAfterFinish)
        {
            Process myProcess = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(proFullPath);
            startInfo.WindowStyle = proNeedShow ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden;
            startInfo.Arguments = proParams;
            myProcess.StartInfo = startInfo;
            myProcess.Exited += handlerAfterFinish;
            myProcess.Start();
        }


        /// <summary>
        /// 结束指定进程
        /// </summary>
        public void KillProcess(string proName)
        {
            Process[] myProcesses;
            DateTime startTime;
            myProcesses = Process.GetProcessesByName(proName);

            foreach (Process myProcess in myProcesses)
            {
                startTime = myProcess.StartTime;
                myProcess.Kill();
                
            }
        }
    }
}
