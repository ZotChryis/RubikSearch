using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private RectTransform rectTransform;

    public Vector2 AnchoredPosition
    {
        get => rectTransform.anchoredPosition;
        set => rectTransform.anchoredPosition = value;
    }
    
    public Vector3 AnchoredPositionV3 => rectTransform.anchoredPosition;

    private int _row;
    private int _col;
    
    public void SetText(string newText)
    {
        text.SetText(newText);
    }

    public string GetText()
    {
        return text.text;
    }
}
