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
using System.Threading;
public class PacketSenderAndroid : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TMP_InputField _PathInputField;
    [SerializeField] GameObject _LoadingScreen;
    private WOL_ConfigSettings _ConfigSettings;
    void Awake()
    {
        _ConfigSettings = new();
        if (File.Exists(JSONFilePath.Path))
        {
            LoadFromJSON();
            _PathInputField.text = JSONFilePath.Path;
        }
        else
        {
            InitializeJSON(true);
        }
    }

    #region Other Buttons
    public void OpenConfigFile()
    {
        if (File.Exists(JSONFilePath.Path))
        {
#if ENABLE_MONO
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(JSONFilePath.Path) { UseShellExecute = true });
#else
            Application.OpenURL(JSONFilePath.Path);
#endif
        }
        else
        {
            InitializeJSON(false);
#if ENABLE_MONO
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(JSONFilePath.Path) { UseShellExecute = true });
#else
            Application.OpenURL(JSONFilePath.Path);
#endif
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
        if (!reloadscene) return;
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
        _LoadingScreen.SetActive(true);
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
    CancellationTokenSource _canceltoken;
    async void CheckServerStatusandOpenSites()
    {
        int delay = 1000;
        bool serveronlinestatus = false;
        do
        {
            if (_servercheckcounter >= 20)
            {
                _LoadingScreen.SetActive(false);
                _canceltoken.Cancel();
                Debug.LogError("No response from server timeout");
                return;
            }
            if (_canceltoken.Token.IsCancellationRequested)
            {
                Debug.LogWarning("Threading Task cancelled.");
                return;
            }
            serveronlinestatus = SimplePing(_ConfigSettings.Ping_IP, delay);
            _servercheckcounter += 1;
            await Task.Delay(delay + 5, _canceltoken.Token);
        }
        while (!serveronlinestatus);
        Debug.Log("Server Ping Response Sucess!!! Server is online");
        //_LoadingScreen.SetActive(false);
        FinishedTasks();
        return;

        #region Subfunctions
        bool SimplePing(string host, int timeout)
        {
            using (System.Net.NetworkInformation.Ping newsimpleping = new System.Net.NetworkInformation.Ping())
            {
                PingReply reply = newsimpleping.Send(host, timeout);
                return reply.Status == IPStatus.Success; // will return 1 or true if match else false
            }
        }
        #endregion
    }
    void FinishedTasks()
    {
        _LoadingScreen.SetActive(false);
        if (_ConfigSettings.QuitApplicationAfterWake)
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