using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TMPro;
public class PacketSender : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TMP_InputField _PathInputField;
    WOL_ConfigSettings _ConfigSettings;
    void Awake()
    {
         QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        _ConfigSettings = new ();
        if (File.Exists(JSONFilePath.Path))
        {
            LoadFromJSON();
        }
        else
        {
            Debug.LogWarning("No JSON, Initializing");
            InitializeJSON(true);
        }
    }
    void Start()
    {
        if (_ConfigSettings.ExecuteWakeOnStart) SendMagicPacket();
        _PathInputField.text = JSONFilePath.Path;
    }
    #region Other Buttons
    public void CopyPath()
    {
        GUIUtility.systemCopyBuffer = JSONFilePath.Path;
        Debug.Log("Copied Path to clipboard");
    }
    public void OpenConfigFile()
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(JSONFilePath.Path) { UseShellExecute = true });
        }
        catch
        {
            InitializeJSON(false);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(JSONFilePath.Path) { UseShellExecute = true });
        }
    }
    public void DeleteConfigFile()
    {
        if (File.Exists(JSONFilePath.Path))
        {
            File.Delete(JSONFilePath.Path);
            Debug.Log("Config File Deleted");
        }
    }
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Scenereset");
    }
    public void QuitApplication()
    {
        Application.Quit();
        Debug.Log("Application Quit");
    }
    #endregion
    #region JSON
    void InitializeJSON(bool reloadscene)
    {
        WOL_ConfigSettings config = new();
        string json = JsonUtility.ToJson(config, true); ;
        File.WriteAllText(JSONFilePath.Path, json);
        Debug.Log("JSON Initialized");
        if(!reloadscene) return;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Reloading Scene");
    }
    void LoadFromJSON()
    {
        string json = File.ReadAllText(JSONFilePath.Path);
        _ConfigSettings = JsonUtility.FromJson<WOL_ConfigSettings>(json);
        if (_ConfigSettings != null)
        {
            Debug.Log("Data loaded successfully!");
        }
        else
        {
            Debug.LogWarning("No JSON / JSON load error, reInitializing");
            InitializeJSON(true);
        }
    }
    #endregion
    #region WOL and ping
    public void SendMagicPacket()
    {
        byte[] macBytes = ParseMacAddress(_ConfigSettings.MAC);

        // Create the magic packet
        byte[] packet = new byte[102];
        for (int i = 0; i < 6; i++) packet[i] = 0xFF; // Start with 6 bytes of 0xFF
        for (int i = 0; i < 16; i++) macBytes.CopyTo(packet, 6 + i * 6); // Append MAC 16 times

        // Send the magic packet via UDP
        using (UdpClient udpClient = new UdpClient())
        {
            udpClient.EnableBroadcast = true;
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(_ConfigSettings.BroadcastAddress), _ConfigSettings.BroadcastPort);
            udpClient.Send(packet, packet.Length, endpoint);
        }
        Debug.Log("sent magic packet");
        if (_ConfigSettings.LaunchSitesAfterWake)
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
            serveronlinestatus = SimplePing(_ConfigSettings.Ping_IP, delay);
            _servercheckcounter += 1;
            await Task.Delay(delay + 5);
        }
        while (!serveronlinestatus);
        Debug.Log("Server Ping Response Sucess!!! Server is online");
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
            if (_ConfigSettings.SitesToLoad == null)
            {
                FinishedTasks();
                return;
            }
            foreach (string site in _ConfigSettings.SitesToLoad)
            {
                Application.OpenURL(site);
                Debug.Log("Sites opened");
            }
            FinishedTasks();
            return;
        }
        #endregion
    }
    void FinishedTasks()
    {
        if(_ConfigSettings.QuitApplicationAfterWake)
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