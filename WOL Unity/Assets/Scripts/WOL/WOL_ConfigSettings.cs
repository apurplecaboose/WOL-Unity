using System;
using UnityEngine;
[Serializable]
public class WOL_ConfigSettings
{
    public bool StartFullscreen = false;
    public Vector2Int DefaultResolution = Vector2Int.zero;

    public bool ExecuteWakeOnStart = false;
    public float WakeOnStartDelay = 3f;
    public string MAC = "FF:FF:FF:FF:FF:FF";
    public string BroadcastAddress = "255.255.255.255";
    public int BroadcastPort = 9;
    public bool LaunchSitesAfterWake = false;
    public bool QuitApplicationAfterWake = false;
    public string Ping_IP = "192.168.1.1";
    public string[] SitesToLoad = { "https://turniptoss.itch.io/wake-on-lan-wol-gui", "https://github.com/apurplecaboose/WOL-Unity", "http://192.168.1.1" };
}

public class JSONFilePath
{
    public static string Path = UnityEngine.Application.persistentDataPath + "/WOL_CONFIG_SETTINGS.json";
}
