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
using TMPro;
public class PacketSender : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TMP_InputField _PathInputField;
    [Header("Debug Varibles")]
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
        }
    }
    void Start()
    {
        if (_ExecuteWakeOnStart) SendMagicPacket();
        _PathInputField.text = JSONFilePath.Path;
    }
    #region Other Buttons
    public void CopyPath()
    {
        GUIUtility.systemCopyBuffer = JSONFilePath.Path;
        Debug.Log("Copied Path to clipboard");
    }
    public void OpenFile()
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(JSONFilePath.Path) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            InitializeJSON();
        }

    }
    public void QuitApplication()
    {
        Application.Quit();
        Debug.Log("Application Quit");
    }
    #endregion
    #region JSON
    void InitializeJSON()
    {
        WOL_ConfigSettings config = new();
        string json = JsonUtility.ToJson(config, true); ;
        File.WriteAllText(JSONFilePath.Path, json);
        Debug.Log("JSON Initialized");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Reloading Scene");
    }
    void LoadFromJSON()
    {
        string json = File.ReadAllText(JSONFilePath.Path);
        WOL_ConfigSettings configuration = JsonUtility.FromJson<WOL_ConfigSettings>(json);

        if (configuration != null)
        {
            _ExecuteWakeOnStart = configuration.ExecuteWakeOnStart;
            _MAC = configuration.MAC;
            _BroadcastAddress = configuration.BroadcastAddress;
            _BroadcastPort = configuration.BroadcastPort;
            _LaunchSitesAfterWake = configuration.LaunchSitesAfterWake;
            _QuitApplicationAfterWake = configuration.QuitApplicationAfterWake;
            _Ping_IP = configuration.Ping_IP;
            _SitesToLoad = configuration.SitesToLoad;
            Debug.Log("Data loaded successfully!");
        }
        else
        {
            Debug.LogWarning("No JSON / JSON load error, reInitializing");
            InitializeJSON();
        }
    }
    #endregion
    #region WOL and ping
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
            Debug.Log("Application Quit");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    #endregion
}