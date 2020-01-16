using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace deploy
{
    class Program
    {
        static void Main(string[] args)
        {
            DepolyCode deploy = new DepolyCode();
            deploy.PushCode();
        }
    }

    public class CommandRunner
    {
        private string executablePath;
        private string workingDirectory;

        public CommandRunner(string executablePath, string workingDirectory = null)
        {
            this.executablePath = executablePath;
            this.workingDirectory = workingDirectory;
        }

        public string Run(string arguments = null)
        {
            string strResult = string.Empty;
            using (Process process = new Process()
                {
                    StartInfo = new ProcessStartInfo(executablePath)
                    {
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        WorkingDirectory = workingDirectory,
                        Arguments = (arguments == null) ? string.Empty : arguments.ToString()
                    }
                })
            {
                try
                {
                    process.Start();
                    strResult = process.StandardOutput.ReadToEnd();
                }
                catch (Exception e)
                {
                    strResult = e.Message;
                }
            }
            return strResult;
        }
    }

    public class DepolyCode
    {
        public void PushCode()
        {
            CommandRunner gitCommand = new CommandRunner (@"C:\Program Files\Git\bin\git.exe",@"E:\blog");
            CommandRunner hexoCommand = new CommandRunner(@"C:\Users\Administrator\AppData\Roaming\npm\hexo.cmd", @"E:\blog");
            {
                try
                {
                    Console.WriteLine(hexoCommand.Run("g"));
                    Console.WriteLine(hexoCommand.Run("d"));
                    Console.WriteLine(gitCommand.Run("add ."));
                    Console.WriteLine(gitCommand.Run(string.Format("commit -m \"{0}\"", DateTime.Now.ToString())));
                    Console.WriteLine(gitCommand.Run("push"));
                    Console.ReadKey();
                    this.ErrorLog("finish Deploy");
                }
                catch (Exception e)
                {
                    this.ErrorLog(e.Message);
                }
            }
        }

        public void ErrorLog(string str)
        {
            string FilePath = @"Logs.txt";

            StringBuilder msg = new StringBuilder();
            msg.Append("\r\n*************************************** \r\n");
            msg.AppendFormat(" 时间： {0} \r\n", DateTime.Now);
            msg.Append(str);

            try
            {
                if (File.Exists(FilePath))
                {
                    using (StreamWriter tw = File.AppendText(FilePath))
                    {
                        tw.WriteLine(msg.ToString());
                    }
                }
                else
                {
                    TextWriter tw = new StreamWriter(FilePath);
                    tw.WriteLine(msg.ToString());
                    tw.Flush();
                    tw.Close();
                    tw = null;
                }
            }
            catch (Exception)
            {

            }

        }

    }
}
