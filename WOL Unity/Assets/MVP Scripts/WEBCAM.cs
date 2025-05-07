using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WEBCAM : MonoBehaviour
{
    [SerializeField]TMP_Text fpscounter;
    [SerializeField] RawImage _rawImage; // Serialized RawImage instance
    [SerializeField] TMP_Dropdown _Dropdown;
    [SerializeField] Texture _NullImage; // Fallback image when webcam fails
    private WebCamTexture _webcamTexture;
    private List<string> _deviceNames = new List<string>();
    public GameObject Toggle;
    private float deltaTime = 0.0f;
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120; // Set to desired FPS
    }

    void Start()
    {
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

        // Listen for dropdown value changes
        _Dropdown.onValueChanged.AddListener(ChangeCamera);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Toggle.SetActive(!Toggle.activeSelf);
        }
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpscounter.text = $"FPS: {Mathf.Ceil(fps)}"; // Display FPS value
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


    public void QuitApp()
    {
        Application.Quit();
    }
}
