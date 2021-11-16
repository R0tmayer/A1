using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[AddComponentMenu("Game Managers/Audio Manager")]
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null; 
    public static AudioSettingsModel settings = null; 
    private static string _settingsPath = string.Empty;

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

    private void InitializeSettings()
    {
        if (settings == null)
        {
            settings = new AudioSettingsModel(); 
        }

        if (File.Exists(_settingsPath))
        { 
            LoadSettings(); 
        }
    }

    public void LoadSettings()
    {
        string _data = File.ReadAllText(_settingsPath); 
        settings = JsonUtility.FromJson<AudioSettingsModel>(_data); 
    }

    public void SaveSettings()
    {
        string _json_data = JsonUtility.ToJson(settings); 
        File.WriteAllText(_settingsPath, _json_data);
    }

    public void ToggleSounds(bool enabled)
    {
        settings.sounds = enabled; 
        SaveSettings(); 
        OnAudioSettingsChanged?.Invoke(); 
    }

    public void ToggleMusic(bool enabled)
    {
        settings.music = enabled; 
        SaveSettings(); 
        OnAudioSettingsChanged?.Invoke(); 
    }
}