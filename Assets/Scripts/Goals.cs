using System.Collections.Generic;
using UnityEngine;

public class Goals : MonoBehaviour
{
    // todo: move this stuff somewhere else, this class should be the goals UI only
    // A text asset I found online here: https://github.com/eddydn/DictionaryDatabase/blob/master/EDMTDictionary.json
    // It holds all the english dictionary in JSON
    [SerializeField] private TextAsset DictionaryJSON;
    [SerializeField] private Board Board;

    [SerializeField] private int Rows = 5;
    [SerializeField] private int Cols = 5;
    [SerializeField] private int WordCount = 3;
    [SerializeField] private int MinWordLength = 3;
    
    [SerializeField] private Transform WordListParent;
    [SerializeField] private GameObject GoalPrefab;

    private List<UIGoal> _goals = new List<UIGoal>();
    private Color[] _unusedColors;

    private void Start()
    {
        Regenerate();
    }

    private void Setup(WordSearchGenerator.WordSearch wordSearch)
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
            
            // for test, mark as done
            Board.MarkAnswerDone(wordSearchAnswer);
        }
    }

    public void Regenerate()
    {
        // todo: move this stuff somewhere else, this class should be the goals UI only
        WordSearchGenerator wordSearchGenerator = new WordSearchGenerator(DictionaryJSON);
        WordSearchGenerator.WordSearch wordSearch = wordSearchGenerator.GeneratePuzzle(
            Rows, 
            Cols, 
            WordCount, 
            MinWordLength
        );

        Board.Setup(wordSearch);
        Setup(wordSearch);
    }

    public void SetRows(string value)
    {
        Rows = int.Parse(value);
    }
    public void SetCols(string value)
    {
        Cols = int.Parse(value);
    }
    public void SetWordCount(string value)
    {
        WordCount = int.Parse(value);
    }
    public void SetMinWordLength(string value)
    {
        MinWordLength = int.Parse(value);
    }
}