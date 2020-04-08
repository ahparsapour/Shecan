using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;
using DNS.Server;
using DNS.Client;
using System.IO;

namespace Shecan
{
    public class DNSUtil
    {
        static DnsServer _server;
        public static NetworkInterface GetActiveEthernetOrWifiNetworkInterface()
        {
            var Nic = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(
                a => a.OperationalStatus == OperationalStatus.Up &&
                     (a.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                      a.NetworkInterfaceType == NetworkInterfaceType.Ethernet) &&
                     a.GetIPProperties().GatewayAddresses
                         .Any(g => g.Address.AddressFamily.ToString() == "InterNetwork"));

            return Nic;
        }
        // Method to prepare the WMI query connection options.
        public static void SetDNS(string nicName, string DnsSearchOrder)
        {
            if(DnsSearchOrder == null)
            {
                Process tool = new Process();
                tool.StartInfo.FileName = "netsh";
                tool.StartInfo.Arguments = $"interface ip delete dnsserver name={nicName} all";
                tool.StartInfo.RedirectStandardOutput = true;
                tool.StartInfo.CreateNoWindow = true;
                tool.StartInfo.UseShellExecute = false;
                tool.Start();
                string output;
                output = tool.StandardOutput.ReadToEnd();
                return;
            }
            int counter = 1;
            foreach (var address in DnsSearchOrder.Split(','))
            {
                Process tool = new Process();
                tool.StartInfo.FileName = "netsh";
                tool.StartInfo.Arguments = $"interface ip add dns name=\"{nicName}\" addr=\"{address}\" index={counter}";
                tool.StartInfo.RedirectStandardOutput = true;
                tool.StartInfo.CreateNoWindow = true;
                tool.StartInfo.UseShellExecute = false;
                tool.Start();
                string output;
                output = tool.StandardOutput.ReadToEnd();
                counter++;
            }
        }
        public static List<IPAddress> GetDnsAdresses()
        {
            List<IPAddress> dnsAddressesList = new List<IPAddress>();
            NetworkInterface networkInterface = GetActiveEthernetOrWifiNetworkInterface();

            if (networkInterface.OperationalStatus == OperationalStatus.Up)
            {
                IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                IPAddressCollection dnsAddresses = ipProperties.DnsAddresses;

                foreach (IPAddress dnsAdress in dnsAddresses)
                {
                    dnsAddressesList.Add(dnsAdress);
                }
            }

            if (dnsAddressesList.Count > 0)
            {
                return dnsAddressesList;
            }

            throw new InvalidOperationException("Unable to find DNS Address");
        }
    }
}