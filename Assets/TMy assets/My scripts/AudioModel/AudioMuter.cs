using UnityEngine;

[AddComponentMenu("Audio/Audio Muter Component")]
public class AudioMuter : MonoBehaviour
{
    public bool isMusic = true;
    private AudioSource _audioSource;
    private float _baseVolume = 1F;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _baseVolume = _audioSource.volume;

        AudioManager.instance.OnAudioSettingsChanged += AudioSettingsChanged;
        AudioSettingsChanged();
    }

    private void OnDestroy()
    {
        AudioManager.instance.OnAudioSettingsChanged -= AudioSettingsChanged;
    }

    private void AudioSettingsChanged()
    {
        _audioSource.volume = (AudioManager.settings.music) ? _baseVolume : 0F;
    }
}