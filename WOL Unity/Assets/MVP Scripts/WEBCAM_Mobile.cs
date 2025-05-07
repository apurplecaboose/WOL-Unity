using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WEBCAM_Mobile : MonoBehaviour
{
    [SerializeField] RawImage _rawImage;
    [SerializeField] TMP_Dropdown _Dropdown;
    [SerializeField] Texture _NullImage;
    WebCamTexture _webcamTexture;
    List<string> _deviceNames = new();
    public GameObject Toggle;
    float _deltaTime = 0.0f;
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }

    void Start()
    {
        //Screen.orientation = ScreenOrientation.LandscapeRight; // or LandscapeRight
        RefreshWebcamDevices();

        if (_deviceNames.Count > 0)
        {
            ChangeCamera(0); // Start with the first device
        }
        else
        {
            Debug.LogWarning("No webcam devices found. Using fallback texture.");
            _rawImage.texture = _NullImage;
        }

        // Listen for dropdown value change events
        _Dropdown.onValueChanged.AddListener(ChangeCamera);
    }
    public void ToggleHideShowUI()
    {
        Toggle.SetActive(!Toggle.activeSelf);
    }
    void ChangeCamera(int index)
    {
        if (_webcamTexture != null)
        {
            _webcamTexture.Stop();
        }
        _webcamTexture = new WebCamTexture(_deviceNames[index]);
        _webcamTexture.Play();
        _rawImage.texture = _webcamTexture;
    }
    public void RefreshWebcamDevices()
    {
        _deviceNames.Clear();
        _Dropdown.ClearOptions();

        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            foreach (var device in devices)
            {
                _deviceNames.Add(device.name);
            }

            _Dropdown.AddOptions(_deviceNames);
        }
    }
}
