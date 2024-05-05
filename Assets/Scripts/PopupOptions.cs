using UnityEngine;
using UnityEngine.UI;

public class PopupOptions : MonoBehaviour
{
    // todo: Better serialization of preferences
    private const string c_keySfxVolume = "SFX_VOLUME";

    [SerializeField] private Slider m_sfxSlider;
    
    private float _sfxVolume = 0.5f;
    
    private void Start()
    {
        Load();
        Apply();

        m_sfxSlider.onValueChanged.AddListener(UpdateSfxVolume);
    }

    private void Load()
    {
        _sfxVolume = PlayerPrefs.GetFloat(c_keySfxVolume, 0.5f);
        m_sfxSlider.normalizedValue = _sfxVolume;
    }

    private void Save()
    {
        PlayerPrefs.SetFloat(c_keySfxVolume, _sfxVolume);
    }
    
    private void Apply()
    {
        // todo: better reactive updates
        ServiceLocator.Instance.SoundManager.SfxVolume = _sfxVolume;
        ServiceLocator.Instance.SoundManager.ApplySettings();
    }
    
    private void UpdateSfxVolume(float arg0)
    {
        _sfxVolume = m_sfxSlider.normalizedValue;

        Save();
        Apply();
    }
}
