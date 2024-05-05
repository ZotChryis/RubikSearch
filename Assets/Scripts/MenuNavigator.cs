using UnityEngine;
using UnityEngine.UI;

public class MenuNavigator : MonoBehaviour
{
    [SerializeField] private Button m_button;
    [SerializeField] private bool m_close;
    [SerializeField] private bool m_mainMenu;

    private void Start()
    {
        m_button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (m_close)
        {
            ServiceLocator.Instance.PopupManager.RequestClose();
        }
        
        if (m_mainMenu)
        {
            ServiceLocator.Instance.Game.TempReturnToMenu();
        }
    }
}
