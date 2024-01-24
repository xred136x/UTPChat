using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UTPChat
{
    public partial class Form1 : Form
    {
        //IPAddress remoteIP;
        IPEndPoint remoteEndPoint;
        public Form1()
        {
            InitializeComponent();
            //btnConnect_Click(null, null);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            UdpClient udpClient = new UdpClient();
            byte[] buffer = Encoding.Unicode.GetBytes(txtMes.Text);
            udpClient.Send(buffer, buffer.Length, remoteEndPoint);
            udpClient.Close();
            //txtAllMes.Lines.Append("--"+txtMes.Text);
            txtMes.Text = "";
        }
        Thread threadRec;
        CancellationTokenSource cts;
        private void btnConnect_Click(object sender, EventArgs e)
        {
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(mtIPAddress.Text),(int)numPort.Value);
            /*threadRec = new Thread(new ThreadStart(ThreadReceive));
            threadRec.IsBackground = true;*/
            cts = new CancellationTokenSource();
            Task.Run(() => ThreadReceive(cts.Token));
            //threadRec.Start();
            
        }
        void ThreadReceive(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    UdpClient udpClient = new UdpClient((int)numPort.Value);
                    IPEndPoint remEndPoint = new IPEndPoint(IPAddress.Any, (int)numPort.Value);
                    byte[] response = udpClient.Receive(ref remEndPoint); //
                    Invoke((MethodInvoker)delegate
                    {
                        //txtAllMes.Lines.Append(Encoding.Unicode.GetString(response));
                        txtAllMes.Text += Encoding.Unicode.GetString(response)+"\r\n";
                    });
                    udpClient.Close();
                }
            }
            catch { }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //threadRec.Abort();
            cts.Cancel();
        }
    }
}
