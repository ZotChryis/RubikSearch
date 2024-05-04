using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    [SerializeField] private Image Fill;

    [Range(0f, 1f)]
    public float Percentage = 1.0f;

    private void Update()
    {
        Fill.fillAmount = Percentage;
    }
}
