using Jupiter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mmacrobot
{
    public partial class Form1 : Form
    {
        static Socket soket1 = new Socket
   (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        MemoryModule memoryModule1;
        IntPtr hpAddr, mpAddr, npcAddr;
        string[] ip;
        int resDurum = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            resDurum = 1;
     
            await hpT();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            resDurum = 0;
            sgonder("3", soket1);
        }


        private void button3_Click(object sender, EventArgs e)
        {
            ip = System.IO.File.ReadAllLines(@"..\\Debug\ip.txt");
            MessageBox.Show(ip[0]);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string[] pattern = System.IO.File.ReadAllLines(@"..\\Debug\m1.txt");
            hpAddr = (IntPtr)Convert.ToInt64(pattern[0], 16) + 0x110;
            mpAddr = hpAddr+0x08;
            npcAddr = (IntPtr)Convert.ToInt64(pattern[1], 16);
            MessageBox.Show(pattern[0].ToString() + "\n" + pattern[1].ToString());
        }

        private  void button5_Click(object sender, EventArgs e)
        {
            memoryModule1 = new MemoryModule(Convert.ToInt32(textBox1.Text));
            MessageBox.Show("Connected");

        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {

                soket1.Connect(new IPEndPoint(IPAddress.Parse(ip[0]), 12345));
            }
            catch (Exception ex)
            {

                MessageBox.Show("Hata");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            soket1.Close();
            soket1 = new Socket
                    (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }


        Task hpT()
        {
            return Task.Run(async () =>
            {
                while (resDurum==1)
                {
                    await Task.Delay(200);
                    var hp1 = memoryModule1.ReadVirtualMemory<int>(hpAddr);
                    var mp1 = memoryModule1.ReadVirtualMemory<int>(mpAddr);
                    label1.Invoke((MethodInvoker)(() => label1.Text = (hp1).ToString()));
                    label2.Invoke((MethodInvoker)(() => label2.Text = (mp1).ToString()));

                    if(hp1<5000 || mp1<1000)
                    {
                        memoryModule1.WriteVirtualMemory<int>(npcAddr + 0xC, 1);
                        memoryModule1.WriteVirtualMemory<int>(npcAddr + 0x250, 0);
                        memoryModule1.WriteVirtualMemory<int>(npcAddr + 0x224, 38);

                        sgonder("1", soket1);
                        await Task.Delay(500);
                        memoryModule1.WriteVirtualMemory<int>(npcAddr + 0xC, 0);
                        await Task.Delay(100);
                        sgonder("2", soket1);
                    }
                    else
                    {
                        sgonder("2", soket1);
                    }
                   
                }
            });
        }

        public void sgonder(string deger, Socket s)
        {
            if (s.Connected)
            {
                string gonder = deger;
                s.Send(Encoding.UTF8.GetBytes(gonder));
            }
        }
    }
}
