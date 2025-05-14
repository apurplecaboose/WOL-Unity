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
    [SerializeField] bool _ExecuteWakeOnStart;
    [SerializeField] string _MAC;
    [SerializeField] string _BroadcastAddress;
    [SerializeField] int _BroadcastPort;
    [SerializeField] bool _LaunchSitesAfterWake;
    [SerializeField] bool _QuitApplicationAfterWake;
    [SerializeField] string _Ping_IP;
    [SerializeField] string[] _SitesToLoad;

    void Awake()
    {
        if (File.Exists(JSONFilePath.Path))
        {
            LoadFromJSON();
            Debug.Log("LOADED FROM JSON");
        }
        else
        {
            Debug.LogWarning("No JSON, Initializing");
            InitializeJSON();
            SceneManager.LoadScene(1);
        }
    }
    #region JSON
    public void SaveToJSON()
    {
        DeviceData data = new();
        //data.MAC = _mac;
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(JSONFilePath.Path, json);
        Debug.Log("Data saved successfully!" + "Saved JSON file at: " + UnityEngine.Application.persistentDataPath);
    }
    void InitializeJSON()
    {
        DeviceData data = new();
        string json = JsonUtility.ToJson(data, true); ;
        File.WriteAllText(JSONFilePath.Path, json);
        Debug.Log("JSON Initialized");
    }
    void LoadFromJSON()
    {
        string json = File.ReadAllText(JSONFilePath.Path);
        DeviceData data = JsonUtility.FromJson<DeviceData>(json);

        if (data != null)
        {
            _ExecuteWakeOnStart = data.ExecuteWakeOnStart;
            _MAC = data.MAC;
            _BroadcastAddress = data.BroadcastAddress;
            _BroadcastPort = data.BroadcastPort;
            _LaunchSitesAfterWake = data.LaunchSitesAfterWake;
            _QuitApplicationAfterWake = data.QuitApplicationAfterWake;
            _Ping_IP = data.Ping_IP;
            _SitesToLoad = data.SitesToLoad;
            Debug.Log("Data loaded successfully!");
        }
        else
        {
            Debug.LogWarning("No JSON / JSON load error, reInitializing");
            InitializeJSON();
            SceneManager.LoadScene(1);
        }
    }
    #endregion
    public void LoadSettingsScene(int index)
    {
        SceneManager.LoadScene(index);
    }
    public void SendMagicPacket()
    {
        byte[] macBytes = ParseMacAddress(_MAC);

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
        if (_LaunchSitesAfterWake)
        {
            CheckServerStatusandOpenSites();
        }
        else
        {
            FinishedTasks();
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
            if (_servercheckcounter >= 20)
            {
                Debug.LogError("No response from server timeout");
                return;
            }
            serveronlinestatus = SimplePing(_Ping_IP, delay);
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
            if (_SitesToLoad == null)
            {
                FinishedTasks();
                return;
            }
            foreach (string site in _SitesToLoad)
            {
                Application.OpenURL(site);
            }
            FinishedTasks();
            return;
        }
        #endregion
    }
    void FinishedTasks()
    {
        if(_QuitApplicationAfterWake)
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}