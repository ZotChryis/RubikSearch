using UnityEngine;

public class UIMenu : MonoBehaviour
{
    [SerializeField] private RectTransform m_arrow;

    public void PointArrowTo(RectTransform rectTransform)
    {
        m_arrow.position = new Vector2(m_arrow.position.x, rectTransform.position.y);
    }
}
