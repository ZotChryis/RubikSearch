using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private RectTransform rectTransform;

    private char _char;
    
    public Vector2 AnchoredPosition
    {
        get => rectTransform.anchoredPosition;
        set => rectTransform.anchoredPosition = value;
    }
    
    public Vector3 AnchoredPositionV3 => rectTransform.anchoredPosition;

    private int _row;
    private int _col;
    
    public void SetChar(char newChar)
    {
        _char = newChar;
        text.SetText(_char.ToString());
    }

    public char GetChar()
    {
        return _char;
    }
}
