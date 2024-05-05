using System;
using TMPro;
using UnityEngine;

public class UIEntry : MonoBehaviour
{
    [Serializable]
    public struct EntryDefinition
    {
        public string Title;
        public string Description;
    }
    
    [SerializeField] private GameObject Seperator;
    [SerializeField] private TMP_Text Title;
    [SerializeField] private TMP_Text Description;

    public void Setup(EntryDefinition definition)
    {
        bool hasTitle = !string.IsNullOrEmpty(definition.Title);
        bool hasDescription = !string.IsNullOrEmpty(definition.Description);
        
        Title.gameObject.SetActive(hasTitle);
        Description.gameObject.SetActive(hasDescription);
        Seperator.SetActive(hasTitle && hasDescription);
        
        SetupText(definition.Title, definition.Description);
    }
    
    public void SetupText(string title, string description)
    {
        Title.SetText(title);
        Description.SetText(description);
    }
}
