using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameConfig Config { get; set; }
    public Stopwatch Timer { get; private set; } = new Stopwatch();

    public bool IsGameActive => Timer.IsRunning;
    
    [SerializeField] private GameConfig m_defaultConfig;
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
        Config = m_defaultConfig;
        ServiceLocator.Instance.Board.OnEvaluation += OnBoardEvaluation;

        ServiceLocator.Instance.SoundManager.RequestMusic(SoundManager.Music.Main);
    }

    private void OnDestroy()
    {
        ServiceLocator.Instance.Board.OnEvaluation -= OnBoardEvaluation;
    }

    private void OnBoardEvaluation(List<WordSearchGenerator.WordAnswer> words)
    {
        if (words.Count >= _wordSearch.Answers.Count)
        {
            TempWinGame();
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
        
        Timer.Stop();
        Timer.Restart();
        
        OnGameStartEvent?.Invoke();
    }

    public void TempWinGame()
    {
        Timer.Stop();
        OnGameEndEvent?.Invoke();
        
        StartCoroutine(DelayedWinner());
    }

    public void TempReturnToMenu()
    {
        // Hide the game
        m_game.enabled = false;
        
        // Show the menus
        m_main.enabled = true;
        
        // todo: loading system
        m_loading.enabled = false;
    }

    private IEnumerator DelayedWinner()
    {
        yield return new WaitForSeconds(0.5f);
        ServiceLocator.Instance.SoundManager.RequestSfx(SoundManager.Sfx.Win);
        ServiceLocator.Instance.PopupManager.RequestPopup(PopupManager.PopupType.Win);
    }
}