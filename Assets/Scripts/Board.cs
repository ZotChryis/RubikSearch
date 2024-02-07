using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Setup")] 
    [SerializeField] private RectTransform board;
    [SerializeField] private RectTransform grid;
    [SerializeField] private RectTransform linesParent;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private LineRenderer linePrefab;
    [SerializeField] private DragSettings dragSettings;
    [SerializeField] private GraphicRaycaster boardRaycaster;
    [SerializeField] private Color[] lineColors;
    
    private List<LineRenderer> _lines;
    private List<Color> _unusedLineColors;

    private enum DragOrientation
    {
        None,
        Horizontal,
        Vertical
    }

    public struct Coordinate
    {
        public int Row;
        public int Col;

        public Coordinate(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }
    
    private const int TileSize = 100;
    private static readonly Vector2 HorizontalOnly = new Vector2(1, 0);
    private static readonly Vector2 VerticalOnly = new Vector2(0, 1);

    public Tile DraggedTile => _draggedTile;

    private int _leftShiftBoundary;
    private int _rightShiftBoundary;
    private int _upShiftBoundary;
    private int _downShiftBoundary;
    private Vector3 _lineOffset;
    
    private Tile[,] _grid;
    private Tile _draggedTile;
    private int _draggedTileRow;
    private int _draggedTileCol;
    private DragOrientation _dragOrientation = DragOrientation.None;
    private Vector2 _dragDelta;

    private int _rows;
    private int _cols;
    private WordSearchGenerator.WordSearch _wordSearch;

    public Action<List<WordSearchGenerator.WordEntry>> OnEvaluation;
    
    private void InitializeGrid(char[,] wordSearchCharacters)
    {
        _rows = wordSearchCharacters.GetLength(0);
        _cols = wordSearchCharacters.GetLength(1);
        
        board.sizeDelta = new Vector2(_rows * TileSize, _cols * TileSize);
        _leftShiftBoundary = -TileSize;
        _rightShiftBoundary = TileSize * _cols;
        _upShiftBoundary = TileSize;
        _downShiftBoundary = -TileSize * _rows;
        
        _grid = new Tile[_rows, _cols];
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _cols; c++)
            {
                Tile tile = Instantiate(tilePrefab, grid);
                RectTransform tileTransform = tile.GetComponent<RectTransform>();
                var anchoredPosition = tileTransform.anchoredPosition;
                anchoredPosition = new Vector2(
                    anchoredPosition.x + c * TileSize,
                    anchoredPosition.y - r * TileSize
                );
                tileTransform.anchoredPosition = anchoredPosition;
                _grid[r, c] = tile;
                
                tile.SetChar(_wordSearch.Characters[r, c]);
            }
        }
    }
    
    private void Clear()
    {
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _cols; c++)
            {
                Destroy(_grid[r, c].gameObject);
            }
        }

        ClearLines();
    }

    private void ClearLines()
    {
        if (_lines != null)
        {
            for (int i = 0; i < _lines.Count; i++)
            {
                if (_lines[i] && _lines[i].gameObject)
                {
                    Destroy(_lines[i].gameObject);
                }
            }
            _lines.Clear();
        }

        _unusedLineColors = new List<Color>(lineColors.Length);
        _unusedLineColors.AddRange(lineColors);
    }
    
    public void Setup(WordSearchGenerator.WordSearch wordSearch)
    {
        _wordSearch = wordSearch;
        
        Clear();
        InitializeGrid(_wordSearch.Characters);

        _lines = new List<LineRenderer>();
        _lineOffset = new Vector3(TileSize / 2, -TileSize / 2, 0);
        
        CheckAnswers();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        //  Only accept left button drags
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        _draggedTile = GetTile(eventData);

        var coordinate = GetTileCoordinate(_draggedTile);
        _draggedTileRow = coordinate.Row;
        _draggedTileCol = coordinate.Col;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //  Only accept left button drags
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        
        if (_draggedTile == null)
        {
            return;
        }

        _dragDelta = eventData.delta;
        
        // Determine what type of drag this will be read as
        if (_dragOrientation == DragOrientation.None)
        {
            // // Ensure we pass the minimum threshold
            var magDeltaX = Mathf.Abs(_dragDelta.x);
            var magDeltaY = Mathf.Abs(_dragDelta.y);
            if (magDeltaX > magDeltaY && magDeltaX >= dragSettings.minDrag)
            {
                _dragOrientation = DragOrientation.Horizontal;
            }
            else if (magDeltaY > magDeltaX && magDeltaY >= dragSettings.minDrag)
            {
                _dragOrientation = DragOrientation.Vertical;
            }
        }

        //  If there is yet to be a drag sufficiently long, ignore the input.
        if (_dragOrientation == DragOrientation.None)
        {
            return;
        }
        
        //  Move everything towards the desired orientation
        if (_dragOrientation == DragOrientation.Horizontal)
        {
            // Shift all tiles in the same row
            for (var c = 0; c < _cols; c++)
            {
                _grid[_draggedTileRow, c].AnchoredPosition += _dragDelta * HorizontalOnly;
            }

            //  Determine if the drag is sufficient enough to warrant a warp and re-index of tiles
            var firstTile = _grid[_draggedTileRow, 0];
            var lastTile = _grid[_draggedTileRow, _cols - 1];

            //  Handle horizontal left side warp
            if (firstTile.AnchoredPosition.x <= _leftShiftBoundary)
            {
                //  Move all elements in same row one column left
                for (int c = 0; c < _cols - 1; c++)
                {
                    _grid[_draggedTileRow, c] = _grid[_draggedTileRow, c + 1];
                }
                _grid[_draggedTileRow, _cols - 1] = firstTile;
                
                SnapTilesInRow(_draggedTileRow);
                CheckAnswers();
            }
            else if (lastTile.AnchoredPosition.x >= _rightShiftBoundary)
            {
                //  Move all elements in same row one column right
                for (int c = _cols - 1; c > 0; c--)
                {
                    _grid[_draggedTileRow, c] = _grid[_draggedTileRow, c - 1];
                }
                _grid[_draggedTileRow, 0] = lastTile;
                
                SnapTilesInRow(_draggedTileRow);
                CheckAnswers();
            }
        }
        else if (_dragOrientation == DragOrientation.Vertical)
        {
            // Shift all tiles in the same column
            for (var r = 0; r < _rows; r++)
            {
                _grid[r, _draggedTileCol].AnchoredPosition += _dragDelta * VerticalOnly;
            }
            
            //  Determine if the drag is sufficient enough to warrant a warp and re-index of tiles
            var firstTile = _grid[0, _draggedTileCol];
            var lastTile = _grid[_rows - 1, _draggedTileCol];

            //  Handle horizontal left side warp
            if (firstTile.AnchoredPosition.y >= _upShiftBoundary)
            {
                //  Move all elements in same col one row up
                for (int r = 0; r < _rows - 1; r++)
                {
                    _grid[r, _draggedTileCol] = _grid[r + 1, _draggedTileCol];
                }
                _grid[_rows - 1, _draggedTileCol] = firstTile;

                SnapTilesInCol(_draggedTileCol);
                CheckAnswers();
            }
            else if (lastTile.AnchoredPosition.y <= _downShiftBoundary)
            {
                //  Move all elements in same col one row down
                for (int r = _rows - 1; r > 0; r--)
                {
                    _grid[r, _draggedTileCol] = _grid[r - 1, _draggedTileCol];
                }
                _grid[0, _draggedTileCol] = lastTile;
                
                SnapTilesInCol(_draggedTileCol);
                CheckAnswers();
            }
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //  Only accept left button drags
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (_dragOrientation == DragOrientation.Horizontal)
        {
            SnapTilesInRow(_draggedTileRow);
        }
        else if (_dragOrientation == DragOrientation.Vertical)
        {
            SnapTilesInCol(_draggedTileCol);
        }
        
        _draggedTile = null;
        _draggedTileRow = -1;
        _draggedTileCol = -1;
        _dragDelta = new Vector2(0, 0);
        _dragOrientation = DragOrientation.None;
    }
    
    /// <summary>
    /// Returns the tile at the given touch screen position.
    /// Can return null if no tile is found at that location.
    /// </summary>
    private Tile GetTile(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        boardRaycaster.Raycast(eventData, results);
        for (int i = 0; i < results.Count; i++)
        {
            var result = results[i];
            
            //  Non-alloc version will return default when no hit. Skip these.
            if (result.gameObject == null)
            {
                continue;
            }
            
            //  If we didn't hit a tile, skip this.
            Tile tile = result.gameObject.GetComponent<Tile>();
            if (tile == null)
            {
                continue;
            }

            return tile;
        }

        return null;
    }

    private Coordinate GetTileCoordinate(Tile t)
    {
        for (var c = 0; c < _cols; c++)
        {
            for (var r = 0; r < _rows; r++)
            {
                if (_grid[r, c] == t)
                {
                    return new Coordinate
                    {
                        Row = r,
                        Col = c
                    };
                }
            }
        }

        return default;
    }

    private void SnapTilesInRow(int row)
    {
        for (int c = 0; c < _cols; c++)
        {
            _grid[row, c].AnchoredPosition = new Vector2(TileSize * c, -TileSize * row);
        }
    }
    
    private void SnapTilesInCol(int col)
    {
        for (int r = 0; r < _rows; r++)
        {
            _grid[r, col].AnchoredPosition = new Vector2(TileSize * col, -TileSize * r);
        }
    }

    private void CheckAnswers()
    {
        ClearLines();
        
        char[,] characters = new char[_wordSearch.Characters.GetLength(0), _wordSearch.Characters.GetLength(1)];
        for (int r = 0; r < characters.GetLength(0); r++)
        {
            for (int c = 0; c < characters.GetLength(1); c++)
            {
                characters[r, c] = _grid[r, c].GetChar();
            }
        }

        var wordSearchGenerator = ServiceLocator.Instance.WordSearchGenerator;
        List<WordSearchGenerator.WordEntry> completedAnswers = new List<WordSearchGenerator.WordEntry>();
        foreach (var wordSearchAnswer in _wordSearch.Answers)
        {
            if (wordSearchGenerator.FindWordEntry(wordSearchAnswer.WordEntry, characters, out WordSearchGenerator.WordAnswer placedAnswer))
            {
                completedAnswers.Add(placedAnswer.WordEntry);
                MarkAnswerDone(placedAnswer);
            }
        }
        
        OnEvaluation?.Invoke(completedAnswers);
    }
    
    private Color GetUnusedLineColor()
    {
        var color = _unusedLineColors[Random.Range(0, _unusedLineColors.Count)];
        _unusedLineColors.Remove(color);
        return color;
    }
    
    // todo: find a better home?
    public void MarkAnswerDone(WordSearchGenerator.WordAnswer answer)
    {
        // Create a new gameobject with the line renderer component
        var line = Instantiate(linePrefab, linesParent.transform);
        var color = GetUnusedLineColor();
        line.startColor = color;
        line.endColor = color;

        Tile firstTile = _grid[answer.Start.Row, answer.Start.Col];
        Tile lastTile = _grid[answer.End.Row, answer.End.Col];
        line.SetPositions(new [] {firstTile.AnchoredPositionV3 + _lineOffset, lastTile.AnchoredPositionV3 + _lineOffset});

        _lines.Add(line);
    }
}
