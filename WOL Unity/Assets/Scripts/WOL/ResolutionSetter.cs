using UnityEngine;
using System.IO;
using System.Collections;

public class ResolutionSetter : MonoBehaviour
{
    PacketSender _Pcksender;
    private static ResolutionSetter instance;
    WOL_ConfigSettings _ConfigSettings;
    void Awake()
    {
        _Pcksender = Camera.main.GetComponent<PacketSender>();
        EnsureSingleton();
        string json = File.ReadAllText(JSONFilePath.Path);
        _ConfigSettings = JsonUtility.FromJson<WOL_ConfigSettings>(json);
        if (_ConfigSettings.StartFullscreen)
        {
            Screen.fullScreen = true;
            return;
        }
        if (_ConfigSettings.DefaultResolution.x > 0 && _ConfigSettings.DefaultResolution.y > 0)
        {
            Screen.SetResolution(_ConfigSettings.DefaultResolution.x, _ConfigSettings.DefaultResolution.y, false);
        }
        else
        {
            SetAppHalfResolution();
        }
    }
    IEnumerator Start()
    {
        float startdelay = Mathf.Clamp(_ConfigSettings.WakeOnStartDelay, 0, _ConfigSettings.WakeOnStartDelay);
        //waits 3 frames + start delay till start till execute
        yield return null;
        yield return null;
        yield return null;
        yield return new WaitForSeconds(startdelay);
        if (_ConfigSettings.ExecuteWakeOnStart) WakeOnStart();
    }
    void WakeOnStart()
    {
        _Pcksender.SendMagicPacket();
    }
    void EnsureSingleton()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void SetAppHalfResolution()
    {
        int screenWidth = Screen.currentResolution.width;
        int screenHeight = Screen.currentResolution.height;
        float aspectRatio = (float)screenWidth / screenHeight;
        float targetAspectRatio = 16f / 9f;
        int targetWidth, targetHeight;

        if (aspectRatio >= targetAspectRatio) // Wider than 16:9 (e.g., ultrawide)
        {
            targetWidth = screenWidth / 2;  // Scale width to 50% of monitor width
            targetHeight = Mathf.RoundToInt(targetWidth / targetAspectRatio); // Calculate height
        }
        else // Taller or equal to 16:9
        {
            targetHeight = screenHeight / 2; // Scale height to 50% of monitor height
            targetWidth = Mathf.RoundToInt(targetHeight * targetAspectRatio); // Calculate width
        }
        Screen.SetResolution(targetWidth, targetHeight, false);
    }
}
