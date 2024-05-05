using UnityEngine;
using UnityEngine.UI;

public class PopupRequester : MonoBehaviour
{
    public Button Button;
    public PopupManager.PopupType PopupType;

    private void Start()
    {
        Button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        ServiceLocator.Instance.PopupManager.RequestPopup(PopupType);
    }
}
