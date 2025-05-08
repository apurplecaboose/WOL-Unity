using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

//public class WOL_DeviceInstance
//{
//    public string MAC;
//    public string Ping_IP;
//    public string[] SitesToLoad;
//}
public class PacketSender : MonoBehaviour
{
    [SerializeField] string _pingip, _mac;
    [SerializeField] string[] _sitestoload;
    [SerializeField] bool _quitafterlaunch;
    void Start()
    {
        //check if there is json data
        //if is load as devices
        //if not load settings scene

    }
    private void Update()
    {

    }
    void SendMagicPacket(string macAddress, string broadcastAddress = "255.255.255.255", int port = 9)
    {
        byte[] macBytes = ParseMacAddress(macAddress);

        // Create the magic packet
        byte[] packet = new byte[102];
        for (int i = 0; i < 6; i++) packet[i] = 0xFF; // Start with 6 bytes of 0xFF
        for (int i = 0; i < 16; i++) macBytes.CopyTo(packet, 6 + i * 6); // Append MAC 16 times

        // Send the magic packet via UDP
        using (UdpClient udpClient = new UdpClient())
        {
            udpClient.EnableBroadcast = true;
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(broadcastAddress), port);
            udpClient.Send(packet, packet.Length, endpoint);
        }
    }
    byte[] ParseMacAddress(string macAddress)
    {
        string[] macParts = macAddress.Split(':');//splits strings and removes all colons
        byte[] macBytes = new byte[6];
        for (int i = 0; i < 6; i++)
        {
            macBytes[i] = Convert.ToByte(macParts[i], 16);
        }
        return macBytes;
    }
    bool SimplePing(string host, int timeout)
    {
        System.Net.NetworkInformation.Ping newsimpleping = new System.Net.NetworkInformation.Ping();
        PingReply reply = newsimpleping.Send(host, timeout);
        if (reply.Status == IPStatus.Success) return true;
        else return false;
    }
    async void CheckServerStatus()
    {
        int delay = 500;
        bool serveronlinestatus = false;
        do
        {
            serveronlinestatus = SimplePing(_pingip, delay);
            await Task.Delay(delay + 5);
        }
        while (!serveronlinestatus);
        LaunchSitesToLoad(_quitafterlaunch);
        return;
    }
    void LaunchSitesToLoad(bool quitoncompletion)
    {
        foreach (string site in _sitestoload)
        {
            Application.OpenURL(site);
        }
        if(quitoncompletion)
        {
            Application.Quit();
            return;
        }
        else
        {
            //instantiate confirmation
        }
    }

}

