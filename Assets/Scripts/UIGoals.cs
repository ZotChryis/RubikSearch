using System.Collections.Generic;
using UnityEngine;

public class UIGoals : MonoBehaviour
{
    [SerializeField] private Transform WordListParent;
    [SerializeField] private GameObject GoalPrefab;

    private List<UIGoal> _goals = new List<UIGoal>();
    private Color[] _unusedColors;

    private void Start()
    {
        ServiceLocator.Instance.Board.OnEvaluation += OnBoardEvaluation;
    }

    private void OnBoardEvaluation(List<WordSearchGenerator.WordEntry> foundAnswers)
    {
        
    }

    public void Setup(WordSearchGenerator.WordSearch wordSearch)
    {
        foreach (var uiGoal in _goals)
        {
            Destroy(uiGoal.gameObject);
        }
        _goals.Clear();
        
        foreach (var wordSearchAnswer in wordSearch.Answers)
        {
            UIGoal uiGoal = Instantiate(GoalPrefab, WordListParent).GetComponent<UIGoal>();
            uiGoal.Setup(wordSearchAnswer);
            _goals.Add(uiGoal);
        }
    }

    public void SetRows(string value)
    {
        ServiceLocator.Instance.Game.Rows = int.Parse(value);
    }
    public void SetCols(string value)
    {
        ServiceLocator.Instance.Game.Cols = int.Parse(value);
    }
    public void SetWordCount(string value)
    {
        ServiceLocator.Instance.Game.WordCount = int.Parse(value);
    }
    public void SetMinWordLength(string value)
    {
        ServiceLocator.Instance.Game.MinWordLength = int.Parse(value);
    }
}