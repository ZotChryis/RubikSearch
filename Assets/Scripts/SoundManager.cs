using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum Music
    {
        Main = 0,    
    }
    
    public enum Sfx
    {
        ButtonHover = 0,
        ButtonClick = 1,
        
        GridSnap = 2,
        
        PopupEnter = 3,
        PopupExit = 4,
        
        Win = 5,
    }

    [Serializable]
    public struct MusicEntry
    {
        public Music Type;
        public AudioClip Clip;
    }
    
    [Serializable]
    public struct SFXEntry
    {
        public Sfx Type;
        public AudioClip Clip;
    }

    public float MusicVolume { get; set; } = 1.0f;
    public float SfxVolume { get; set; } = 1.0f;

    [SerializeField] private AudioSource m_musicAudioSource;
    [SerializeField] private AudioSource m_sfxAudioSource;
    [SerializeField] private MusicEntry[] m_musicEntries;
    [SerializeField] private SFXEntry[] m_sfxEntries;

    private Dictionary<Music, AudioClip> _musicMapping = new Dictionary<Music, AudioClip>();
    private Dictionary<Sfx, AudioClip> _sfxMapping = new Dictionary<Sfx, AudioClip>();

    private void Start()
    {
        foreach (var entry in m_musicEntries)
        {
            _musicMapping.Add(entry.Type, entry.Clip);
        }
        foreach (var entry in m_sfxEntries)
        {
            _sfxMapping.Add(entry.Type, entry.Clip);
        }

        ApplySettings();
    }

    public void ApplySettings()
    {
        //m_musicAudioSource.volume = MusicVolume;
        m_sfxAudioSource.volume = SfxVolume;
    }

    public void RequestMusic(Music type)
    {
        //m_musicAudioSource.clip = _musicMapping[type];
        //m_musicAudioSource.loop = true;
        //m_musicAudioSource.Play();
    }
    
    public void RequestSfx(Sfx type)
    {
        if (!_sfxMapping.ContainsKey(type))
        {
            return;
        }
        
        Debug.Log("Request SFX: " + type);
        m_sfxAudioSource.PlayOneShot(_sfxMapping[type]);
    }
}
