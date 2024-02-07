using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    public int Rows { get; set; } = 5;
    public int Cols { get; set; } = 5;
    public int WordCount { get; set; } = 3;
    public int MinWordLength { get; set; } = 3;

    private void Start()
    {
        Regenerate();
    }

    public void Regenerate()
    {
        WordSearchGenerator wordSearchGenerator = ServiceLocator.Instance.WordSearchGenerator;
        WordSearchGenerator.WordSearch wordSearch = wordSearchGenerator.GeneratePuzzle(
            Rows, 
            Cols, 
            WordCount, 
            MinWordLength
        );
        
        ServiceLocator.Instance.Board.Setup(wordSearch);
        ServiceLocator.Instance.Goals.Setup(wordSearch);
    }
}