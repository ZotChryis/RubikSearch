using UnityEngine;
using UnityEngine.UI;

public class ButtonSfxRequester : MonoBehaviour//, IPointerEnterHandler
{
    [SerializeField] private Button m_button;

    public void Start()
    {
        m_button.onClick.AddListener(RequestClickSfx);
    }

    private void RequestClickSfx()
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
