using UnityEngine;

public class UIMenu : MonoBehaviour
{
    [SerializeField] private RectTransform m_arrow;
    [SerializeField] private  RectTransform m_target;

    public void PointArrowTo(RectTransform rectTransform)
    {
        m_target = rectTransform;
    }

    private void Update()
    {
        m_arrow.position = new Vector2(m_arrow.position.x, m_target.position.y);
    }
}
