using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;

namespace SimuProteus
{
    public partial class FormSerial : Form
    {
        #region 初始化
        private const int BUFFER_SIZE = 1024;
        private const string SEND_LABEL = "==>>", RECE_LABEL = "<<==";
        private SerialCom serial = null;
        SerialInfo serialInfo;
        public FormSerial()
        {
            InitializeComponent();

            this.InitialSeiralInfo();
        }


        private void FormSerial_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.CloseSeiral();
        }

        private void InitialSeiralInfo()
        {
            this.serialInfo = new DBUtility(true).GetSerialInfo();
            this.btnFresh_Click(null, null);

            //用配置中的连接信息初始化界面展示
            this.cbPorts.Text = this.serialInfo.PortName;
            this.tbDatabits.Text = this.serialInfo.DataBits.ToString();
            this.cbParity.SelectedIndex = this.serialInfo.Parity; ;
            this.cbStopbits.SelectedIndex = this.serialInfo.StopBits;
            this.tbTimeout.Text = this.serialInfo.TimeOut.ToString();
            this.cbBaudrate.SelectedIndex = this.serialInfo.BaudRate;

            this.serial = new SerialCom();
            this.DisableSendComponent(true);
        }

        private void DisableSendComponent(bool flag)
        {
            this.btnSend.Enabled = !flag;
            this.btnFresh.Enabled = flag;
            this.tbDatabits.Enabled = flag;
            this.cbStopbits.Enabled = flag;
            this.tbTimeout.Enabled = flag;
            this.cbBaudrate.Enabled = flag;
            this.cbPorts.Enabled = flag;
            this.cbParity.Enabled = flag;
            this.btnConnect.Text = flag ? "连接" : "断开";
        }

        #endregion

        #region 窗口事件
        private void btnClearSend_Click(object sender, EventArgs e)
        {
            this.tbSend.Text = string.Empty;
        }

        private void btnClearHistory_Click(object sender, EventArgs e)
        {
            this.rtbHistory.Text = string.Empty;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string strInfo = this.tbSend.Text.Trim();
            byte[] buff = null;
            int count = 0;

            if (rbHex.Checked)
            {
                buff = new byte[strInfo.Length / 2];
                strInfo = strInfo.ToLower();
                count = this.CodeContentByHex(buff, strInfo);
                strInfo = this.DecodeByHex(buff, count);
            }
            else
            {
                buff = new byte[strInfo.Length];
                this.CodeContentByAscii(buff, strInfo);
                count = buff.Length;
                strInfo = this.DecodeByAscii(buff, count);
            }

            this.serial.Write(buff, 0, count);
            this.SetWordsColor(SEND_LABEL + strInfo, true);
        }

        private void SetWordsColor(string content, bool isSend)
        {
            if (this.InvokeRequired)
            {
                Action<string, bool> delegateChangeCursor = new Action<string, bool>(SetWordsColor);
                this.Invoke(delegateChangeCursor, new object[] { content, isSend });
                return;
            }
            Color color = Color.DarkViolet;
            if (isSend) color = Color.Blue;
            this.rtbHistory.AppendText("\r\n");
            int start = this.rtbHistory.Text.Length;
            int len = content.Length;
            this.rtbHistory.AppendText(content);
            this.rtbHistory.Select(start, len);
            this.rtbHistory.SelectionColor = color;
        }
        
        private void GetCurrentInfo()
        {
            this.serialInfo.PortName = this.cbPorts.Text;
            this.serialInfo.DataBits = Convert.ToInt32(this.tbDatabits.Text);
            this.serialInfo.Parity = this.cbParity.SelectedIndex;
            this.serialInfo.StopBits = this.cbStopbits.SelectedIndex;
            this.serialInfo.TimeOut = Convert.ToInt32(this.tbTimeout.Text);
            this.serialInfo.BaudRate = this.cbBaudrate.SelectedIndex;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (this.btnConnect.Text == "断开")
            {
                this.CloseSeiral();
                this.DisableSendComponent(true);
                return;
            }

            this.GetCurrentInfo();
            this.serial.PortName = this.serialInfo.PortName;
            this.serial.BaudRate = this.serialInfo.BaudRate;
            this.serial.Parity = (Parity)this.serialInfo.Parity;
            this.serial.DataBits = this.serialInfo.DataBits;
            this.serial.StopBits = (StopBits)this.serialInfo.StopBits;
            this.serial.TimeOut = this.serialInfo.TimeOut;
            this.serial.ReceivedBytesThreshold = 1;
            this.serial.DataReceived += new SerialDataReceivedEventHandler(ReceiveInfo);

            if (!serial.IsOpen)
            {
                serial.Open();
            }

            if (!serial.IsOpen)
            {
                MessageBox.Show(string.Format("打开串口{0}失败！", this.serial.PortName));
                return;
            }

            this.DisableSendComponent(false);
        }

        private void btnFresh_Click(object sender, EventArgs e)
        {
            cbPorts.Items.Clear();
            string[] portStringA = System.IO.Ports.SerialPort.GetPortNames();
            foreach (string portName in portStringA)
            {
                cbPorts.Items.Add(portName);
            }

            btnConnect.Enabled = (cbPorts.Items.Count > 0);
        }

        private void CloseSeiral()
        {
            if (this.serial.IsOpen)
            {
                this.serial.Close();
                this.serial.Dispose();
            }
        }

        private void rtbHistory_TextChanged(object sender, EventArgs e)
        {
            this.rtbHistory.SelectionStart = this.rtbHistory.Text.Length;
            this.rtbHistory.SelectionLength = 0;
            this.rtbHistory.Focus();
            //this.rtbHistory.ScrollToCaret();
        }

        #endregion

        #region 串口消息
        /// <summary>
        /// 串口消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiveInfo(Object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (this.serial.BytesToRead > 0)
                {
                    Byte[] readBuffer = new Byte[BUFFER_SIZE];
                    int count = this.serial.ReadBuffer(readBuffer, BUFFER_SIZE);
                    // Console.WriteLine(count);
                    string strReci = this.rbHex.Checked ? this.DecodeByHex(readBuffer, count) : this.DecodeByAscii(readBuffer, count);
                    this.SetWordsColor(RECE_LABEL + strReci, false);
                }
            }
            catch (TimeoutException timeEx)
            {
                Console.WriteLine("超时");
            }
            catch (Exception ex)
            {
                this.SetWordsColor("接收返回消息异常！具体原因：" + ex.Message, false);
            }
        }
        #endregion

        #region 编码解码
        private void CodeContentByAscii(byte[] buff, string content)
        {
            for (int i = 0; i < content.Length; i++)
            {
                buff[i] = (byte)(content[i]);
            }
        }

        private int CodeContentByHex(byte[] buff, string content)
        {
            int k = 0,high,low;
            for (int i = 0; i+1 < content.Length; i+=2)
            {
                while (i + 1 < content.Length && content[i] == ' ') i++;
                if (i + 1 >= content.Length) break;
                high = char2int(content[i]);
                low =  char2int(content[i+1]);
                buff[k++] = (byte)((high << 4) & 0X0F0 | low);
            }
            return k;
        }

        private int char2int(char pc)
        {
            return pc > '9' ? pc - 'a' + 10 : pc - '0';
        }

        private char int2char(int pi)
        {
            return (char)(pi > 9 ? pi + 'A' - 10 : pi + '0');
        }

        private string DecodeByAscii(byte[] buff, int count)
        {
            StringBuilder sbTmp = new StringBuilder();
            for (int i = 0; i < count; i ++)
            {
                sbTmp.Append((char)buff[i]);
            }
            return sbTmp.ToString();
        }

        private string DecodeByHex(byte[] buff, int count)
        {
            StringBuilder sbTmp = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                sbTmp.Append(int2char(buff[i] >> 4));
                sbTmp.Append(int2char(buff[i] & 0X0F));
                sbTmp.Append(' ');
            }
            return sbTmp.ToString();
        }
        #endregion
    }
}