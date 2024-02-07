using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGoal : MonoBehaviour
{
    [SerializeField] private TMP_Text Word;
    [SerializeField] private TMP_Text Definition;
    
    public void Setup(WordSearchGenerator.WordAnswer wordSearchAnswer)
    {
        Word.SetText(wordSearchAnswer.WordEntry.word);
        Definition.SetText(wordSearchAnswer.WordEntry.description);
    }
}
