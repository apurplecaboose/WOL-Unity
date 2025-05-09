using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
public class PacketSender : MonoBehaviour
{
    [SerializeField] string _mac;
    string _BroadcastAddress = "255.255.255.255";
    int _BroadcastPort = 9;
    [SerializeField] bool _advOptions;
    [SerializeField] string _pingip;
    [SerializeField] string[] _sitestoload;

    void Awake()
    {

        if (File.Exists(JSONFilePath.Path))
        {
            LoadFromJSON();
        }
        else
        {
            Debug.LogWarning("No save file found! Loading Scene 1...");
            SceneManager.LoadScene(1);
        }
    }

    void LoadFromJSON()
    {
        string json = File.ReadAllText(JSONFilePath.Path);
        DeviceData data = JsonUtility.FromJson<DeviceData>(json);

        if (data != null)
        {
            _mac = data.MAC;
            _BroadcastAddress = data.BroadcastAddress;
            _BroadcastPort = data.BroadcastPort;

            _advOptions = data.ADVANCEDOPTIONS;
            _pingip = data.Ping_IP;
            _sitestoload = data.SitesToLoad;

            Debug.Log("Data loaded successfully!");
        }
        else
        {
            Debug.LogError("JSON data null... loading Settings Scene");
            SceneManager.LoadScene(1);
        }
        if(data.WakeOnStart)
        {
            SendMagicPacket();
        }
    }
    public void LoadSettingsScene(int index)
    {
        SceneManager.LoadScene(index);
    }
    public void SendMagicPacket()
    {
        byte[] macBytes = ParseMacAddress(_mac);

        // Create the magic packet
        byte[] packet = new byte[102];
        for (int i = 0; i < 6; i++) packet[i] = 0xFF; // Start with 6 bytes of 0xFF
        for (int i = 0; i < 16; i++) macBytes.CopyTo(packet, 6 + i * 6); // Append MAC 16 times

        // Send the magic packet via UDP
        using (UdpClient udpClient = new UdpClient())
        {
            udpClient.EnableBroadcast = true;
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(_BroadcastAddress), _BroadcastPort);
            udpClient.Send(packet, packet.Length, endpoint);
        }
        Debug.Log("sent magic packet");
        if(_advOptions)
        {
            CheckServerStatusandOpenSites();
        }
        else
        {
            Application.Quit();
            return;
        }
        #region subfunctions
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
        #endregion
    }
    int _servercheckcounter = 0;
    async void CheckServerStatusandOpenSites()
    {
        int delay = 500;
        bool serveronlinestatus = false;
        do
        {
            if(_servercheckcounter >= 20)
            {
                Debug.LogError("No response from server timeout");
                return;
            }
            serveronlinestatus = SimplePing(_pingip, delay);
            _servercheckcounter += 1;
            await Task.Delay(delay + 5);
        }
        while (!serveronlinestatus);
        LaunchSitesToLoad();
        return;

        #region Subfunctions
        bool SimplePing(string host, int timeout)
        {
            System.Net.NetworkInformation.Ping newsimpleping = new System.Net.NetworkInformation.Ping();
            PingReply reply = newsimpleping.Send(host, timeout);
            if (reply.Status == IPStatus.Success) return true;
            else return false;
        }

        void LaunchSitesToLoad()
        {
            if (_sitestoload == null)
            {
                Application.Quit();
                return;
            }
            foreach (string site in _sitestoload)
            {
                Application.OpenURL(site);
            }
            Application.Quit();
            return;

        }
        #endregion
    }
}

