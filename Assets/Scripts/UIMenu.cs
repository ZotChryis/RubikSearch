using System;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    [SerializeField] private RectTransform m_arrow;
    [SerializeField] private  RectTransform m_defaultTarget;

    private RectTransform m_target;
    
    public void Start()
    {
        PointArrowTo(m_defaultTarget);
    }

    public void PointArrowTo(RectTransform rectTransform)
    {
        m_target = rectTransform;
    }

    private void Update()
    {
        if (m_target == null)
        {
            return;
        }
        
        m_arrow.position = new Vector2(m_arrow.position.x, m_target.position.y);
    }
}
