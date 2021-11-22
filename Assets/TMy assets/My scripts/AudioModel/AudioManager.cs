using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[AddComponentMenu("Game Managers/Audio Manager")]
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; 
    public static AudioSettingsModel settings;
    private static string _settingsPath;

    public delegate void AudioSettingsChanged(); 
    public event AudioSettingsChanged OnAudioSettingsChanged; 

    private void Awake()
    {
        Debug.Log(Application.persistentDataPath);
        _settingsPath = Application.persistentDataPath + "/audioSettings.gdf";

        if (instance == null)
        { 
            instance = this; 
        }

        DontDestroyOnLoad(gameObject);
        InitializeSettings();
    }

    private void Start()
    {
        UIManager.Instance.ShowMenuScreen();
    }

    private void InitializeSettings()
    {
        settings ??= new AudioSettingsModel();

        if (File.Exists(_settingsPath))
        { 
            LoadSettings(); 
        }
    }

    private void LoadSettings()
    {
        string data = File.ReadAllText(_settingsPath); 
        settings = JsonUtility.FromJson<AudioSettingsModel>(data); 
    }

    private void SaveSettings()
    {
        string jsonData = JsonUtility.ToJson(settings); 
        File.WriteAllText(_settingsPath, jsonData);
    }

    [ContextMenu("Toggle Music")]
    public void ToggleMusic()
    {
        settings.music = !settings.music; 
        SaveSettings(); 
        OnAudioSettingsChanged?.Invoke(); 
    }
}