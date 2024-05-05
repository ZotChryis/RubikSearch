using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public enum PopupType
    {
        Info,
        PatchNotes,
        Win,
        Options,
    }
    
    [Serializable]
    public struct PopupConfig
    {
        public PopupType Type;
        public UIPopup Prefab;
    }

    public PopupConfig[] Configs;
    public GameObject SharedBG;
    public Transform Root;

    /// <summary>
    /// A helper mapping between type and prefab. This is needed because Unity does not support dictionary
    /// serialization. We construct this from the config list.
    /// </summary>
    private Dictionary<PopupType, UIPopup> m_mapping = new Dictionary<PopupType, UIPopup>();

    /// <summary>
    /// The queue of popups to be shown.
    /// </summary>
    private List<PopupType> m_queue = new List<PopupType>();

    /// <summary>
    /// The instance of the current popup type.
    /// </summary>
    private UIPopup m_currentPopup;

    private void Start()
    {
        foreach (var popupConfig in Configs)
        {
            UIPopup instance = Instantiate(popupConfig.Prefab, Root);
            instance.Hide(true);
            m_mapping.Add(popupConfig.Type, instance);
        }
    }

    public void RequestPopup(PopupType type)
    {
        m_queue.Add(type);
        ProcessQueue();
    }

    public void RequestClose()
    {
        if (m_currentPopup == null)
        {
            return;
        }

        // Hide
        m_currentPopup.Hide();
        m_currentPopup = null;
        
        // Try to go on
        ProcessQueue();
    }

    private void ProcessQueue()
    {
        if (m_currentPopup != null)
        {
            return;
        }

        if (m_queue.Count == 0)
        {
            SharedBG.SetActive(false);
            return;
        }

        PopupType popupType = m_queue[0];
        m_queue.RemoveAt(0);
        
        // Show
        m_currentPopup = m_mapping[popupType];
        m_currentPopup.Show();
        SharedBG.SetActive(true);
    }
}
