using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour//, IPointerEnterHandler
{
    [SerializeField] private Button m_button;

    public void Start()
    {
        m_button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        ServiceLocator.Instance.SoundManager.RequestSfx(SoundManager.Sfx.ButtonClick);
    }

    /*
    public void OnPointerEnter(PointerEventData eventData)
    {
        ServiceLocator.Instance.SoundManager.RequestSfx(SoundManager.Sfx.ButtonHover);
    }
    */
}
