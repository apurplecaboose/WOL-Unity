using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingsPage : MonoBehaviour
{
    //[SerializeField] bool WakeOnStart = false;
    [SerializeField] string _mac;

    //[SerializeField] bool _advOptions;
    //[SerializeField] string _pingip;
    //[SerializeField] string[] _sitestoload;
    public void SaveToJSON()
    {
        WOL_ConfigSettings data = new();
        //data.MAC = _mac;
        // Convert the object to JSON
        string json = JsonUtility.ToJson(data, true);
        // Write JSON data to a file
        File.WriteAllText(JSONFilePath.Path, json);

        Debug.Log("Saved JSON file at: " + UnityEngine.Application.persistentDataPath);
        Debug.Log("Data saved successfully!");
    }
    public static void InitializeJSON()
    {
        WOL_ConfigSettings data = new();
        string json = JsonUtility.ToJson (data, true); ;
        File.WriteAllText (JSONFilePath.Path, json);
        Debug.Log("JSON Initialized");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7)) SaveToJSON();
    }
}
