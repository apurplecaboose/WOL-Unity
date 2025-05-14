using System;
[Serializable]
public class DeviceData
{
    public bool WakeOnStart = false;
    public string MAC = "FF:FF:FF:FF:FF:FF";//
    public string BroadcastAddress = "255.255.255.255";
    public int BroadcastPort = 9;
    public bool ADVANCEDOPTIONS = false;
    public string Ping_IP = "192.168.1.1";
    public string[] SitesToLoad = { "https://duckduckgo.com/", "https://www.youtube.com/", "http://192.168.1.1" };
}
public class JSONFilePath
{
    public static string Path = UnityEngine.Application.persistentDataPath + "/DeviceSaveData.json";
}
