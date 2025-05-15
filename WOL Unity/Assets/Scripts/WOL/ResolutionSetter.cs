using UnityEngine;
using System.IO;

public class ResolutionSetter : MonoBehaviour
{
    private static ResolutionSetter instance;
    WOL_ConfigSettings _ConfigSettings;
    void Awake()
    {
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
