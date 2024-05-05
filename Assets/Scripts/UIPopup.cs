using UnityEngine;
using UnityEngine.UI;

public class UIPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private Button[] m_closeButtons;

    public void Show()
    {
        m_canvasGroup.alpha = 1.0f;
        m_canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        m_canvasGroup.alpha = 0.0f;
        m_canvasGroup.blocksRaycasts = false;
    }

    private void Start()
    {
        foreach (var button in m_closeButtons)
        {
            button.onClick.AddListener(OnCloseButtonPressed);
        }
    }
    
    private void OnCloseButtonPressed()
    {
        ServiceLocator.Instance.PopupManager.RequestClose();
    }
}
