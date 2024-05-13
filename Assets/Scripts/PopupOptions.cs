using UnityEngine;
using UnityEngine.UI;

public class PopupOptions : MonoBehaviour
{
    // todo: Better serialization of preferences
    private const string c_keyMusicVolume = "MUSIC_VOLUME";
    private const string c_keySfxVolume = "SFX_VOLUME";

    [SerializeField] private Slider m_musicSlider;
    [SerializeField] private Slider m_sfxSlider;
    
    private float _musicVolume = 0.5f;
    private float _sfxVolume = 0.5f;
    
    private void Start()
    {
        Load();
        Apply();

        m_musicSlider.onValueChanged.AddListener(UpdateMusicVolume);
        m_sfxSlider.onValueChanged.AddListener(UpdateSfxVolume);
    }

    private void Load()
    {
        _musicVolume = PlayerPrefs.GetFloat(c_keyMusicVolume, 0.2f);
        _sfxVolume = PlayerPrefs.GetFloat(c_keySfxVolume, 0.5f);
        m_musicSlider.normalizedValue = _musicVolume;
        m_sfxSlider.normalizedValue = _sfxVolume;
    }

    private void Save()
    {
        PlayerPrefs.SetFloat(c_keyMusicVolume, _musicVolume);
        PlayerPrefs.SetFloat(c_keySfxVolume, _sfxVolume);
        PlayerPrefs.Save();
    }
    
    private void Apply()
    {
        // todo: better reactive updates
        ServiceLocator.Instance.SoundManager.MusicVolume = _musicVolume;
        ServiceLocator.Instance.SoundManager.SfxVolume = _sfxVolume;
        ServiceLocator.Instance.SoundManager.ApplySettings();
    }
    
    private void UpdateMusicVolume(float _)
    {
        _musicVolume = m_musicSlider.normalizedValue;

        Save();
        Apply();
    }
    
    private void UpdateSfxVolume(float _)
    {
        _sfxVolume = m_sfxSlider.normalizedValue;

        Save();
        Apply();
    }
}
