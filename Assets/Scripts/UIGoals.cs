using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIGoals : MonoBehaviour
{
    [SerializeField] private RectTransform WordListParent;
    [SerializeField] private GameObject GoalPrefab;

    private List<UIGoal> _goals = new List<UIGoal>();
    private Color[] _unusedColors;

    private void Start()
    {
        ServiceLocator.Instance.Board.OnEvaluation += OnBoardEvaluation;
    }

    private void OnDestroy()
    {
        ServiceLocator.Instance.Board.OnEvaluation -= OnBoardEvaluation;
    }

    private void OnBoardEvaluation(List<WordSearchGenerator.WordAnswer> foundAnswers)
    {
        foreach (var uiGoal in _goals)
        {
            uiGoal.MarkInProgress();
        }

        foreach (var uiGoal in _goals)
        {
            var answer = foundAnswers.Find(answer => answer.WordEntry == uiGoal.Answer.WordEntry);
            if (answer == null)
            {
                continue;
            }
            
            uiGoal.MarkDone(answer);
        }
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
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(WordListParent);
    }

    public void SetRows(string value)
    {
        ServiceLocator.Instance.Game.Config.Rows = int.Parse(value);
    }
    public void SetCols(string value)
    {
        ServiceLocator.Instance.Game.Config.Cols = int.Parse(value);
    }
    public void SetWordCount(string value)
    {
        ServiceLocator.Instance.Game.Config.WordCount = int.Parse(value);
    }
    public void SetMinWordLength(string value)
    {
        ServiceLocator.Instance.Game.Config.MinWordLength = int.Parse(value);
    }
}