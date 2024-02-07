using TMPro;
using UnityEngine;

public class DebugInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text draggedTileText;
    
    private Board _board;

    public void Start()
    {
        _board = FindObjectOfType<Board>();
    }

    public void Update()
    {
        draggedTileText.SetText("Dragged Tile: " + _board.DraggedTile ?? _board.DraggedTile.ToString());
    }
}
