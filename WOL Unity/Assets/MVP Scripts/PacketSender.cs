using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine.TextCore.Text;
using UnityEngine.Events;
using System.Net.NetworkInformation;

public class PacketSender : MonoBehaviour
{
    public string TestMacAddress;
    public void SendMagicPacket(string macAddress, string broadcastAddress = "255.255.255.255", int port = 9)
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
    byte[] MacAddressinBytes(string[] processedMacAddress)
    {
        byte[] macBytes = new byte[6];
        return macBytes;
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
    public static bool PingHost(string host)
    {
        try
        {
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            PingReply reply = ping.Send(host, 1000); // Timeout set to 1000ms
            return reply.Status == IPStatus.Success;
        }
        catch
        {
            return false; // Handle exceptions (e.g., invalid host or network issues)
        }
    }
    void Start()
    {
        byte[] printoutput = ParseMacAddress(TestMacAddress);
        foreach (var something in printoutput)
        {
            Debug.Log(something);
        }

    }
}

