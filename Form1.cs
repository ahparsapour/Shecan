using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shecan
{
    public partial class Form1 : Form
    {
        private string DNS1 =  "178.22.122.100";
        private string DNS2 =  "185.51.200.2";
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
                MinimizeToTray();
            }
        }

        public void MinimizeToTray()
        {
            try
            {
                notifyIcon1.BalloonTipTitle = "Shecan";
                notifyIcon1.BalloonTipText = "is running on system tray";

                if (FormWindowState.Minimized == this.WindowState)
                {
                    notifyIcon1.Visible = true;
                    notifyIcon1.ShowBalloonTip(1000);
                    this.Hide();
                }
                else if (FormWindowState.Normal == this.WindowState)
                {
                    notifyIcon1.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var dnsS = DNSUtil.GetDnsAdresses();
            var dnsStrings = dnsS.Select(x => x.ToString().ToString()).ToList();
            if (dnsStrings.Contains(DNS1) || dnsStrings.Contains(DNS2))
            {
                btn_connect_disconnect.Text = "Disconnect";
            }
            else
            {
                btn_connect_disconnect.Text = "Connect";
            }
        }

        private void btn_connect_disconnect_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string NIC = DNSUtil.GetActiveEthernetOrWifiNetworkInterface().Name;
            switch (btn.Text)
            {
                case "Connect":
                    DNSUtil.SetDNS(NIC, DNS1+","+DNS2);
                    break;
                case "Disconnect":
                    DNSUtil.SetDNS(NIC, null);
                    break;
            }
            var dnsS = DNSUtil.GetDnsAdresses();
            var dnsStrings = dnsS.Select(x => x.ToString().ToString()).ToList();
            if (dnsStrings.Contains(DNS1) || dnsStrings.Contains(DNS2))
            {
                btn_connect_disconnect.Text = "Disconnect";
            }
            else
            {
                btn_connect_disconnect.Text = "Connect";
            }
        }
    }
}