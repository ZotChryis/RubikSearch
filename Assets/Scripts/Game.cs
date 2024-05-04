using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameConfig Config { get; set; }

    [SerializeField] private Canvas m_main;
    [SerializeField] private Canvas m_loading;
    [SerializeField] private Canvas m_game;
    
    public delegate void OnGameStart();
    public OnGameStart OnGameStartEvent;
    
    public delegate void OnGameEnd();
    public OnGameEnd OnGameEndEvent;
    
    private WordSearchGenerator.WordSearch _wordSearch;

    public void Regenerate()
    {
        WordSearchGenerator wordSearchGenerator = ServiceLocator.Instance.WordSearchGenerator;
        _wordSearch = wordSearchGenerator.GeneratePuzzle(
            Config.Rows, 
            Config.Cols, 
            Config.WordCount, 
            Config.MinWordLength
        );
        
        ServiceLocator.Instance.Board.Setup(_wordSearch);
        ServiceLocator.Instance.Board.Shuffle();
        ServiceLocator.Instance.Goals.Setup(_wordSearch);
    }
    
    private void Start()
    {
        ServiceLocator.Instance.Board.OnEvaluation += OnBoardEvaluation;
    }

    private void OnDestroy()
    {
        ServiceLocator.Instance.Board.OnEvaluation -= OnBoardEvaluation;
    }

    private void OnBoardEvaluation(List<WordSearchGenerator.WordEntry> words)
    {
        if (words.Count >= _wordSearch.Answers.Count)
        {
            TempEndGame();
        }
    }
    
    // TODO: This needs to be a part of a loading system to show a bar, etc.
    public void TempStartGame()
    {
        // Regenerate the puzzle. This is likely where we'll spend some time 'loading'
        Regenerate();
        
        // Hide the menus
        m_main.enabled = false;

        // Show the game canvas
        m_game.enabled = true;
        
        // todo: loading system
        m_loading.enabled = false;
        
        OnGameStartEvent?.Invoke();
    }

    public void TempEndGame()
    {
        OnGameEndEvent?.Invoke();
    }

    public void TempBack()
    {
        // Hide the game
        m_game.enabled = false;
        
        // Show the menus
        m_main.enabled = true;
        
        // todo: loading system
        m_loading.enabled = false;
    }
}