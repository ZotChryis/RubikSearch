using UnityEngine;
using UnityEngine.UI;

public class UIGoal : UIEntry
{
    [SerializeField] private Image m_doneLine;

    public WordSearchGenerator.WordAnswer Answer { get; private set; }
    
    public void Setup(WordSearchGenerator.WordAnswer wordSearchAnswer)
    {
        Answer = wordSearchAnswer;
        
        base.SetupText(
            wordSearchAnswer.WordEntry.word, 
            wordSearchAnswer.WordEntry.description
        );

        MarkDone(wordSearchAnswer);
    }

    private void MarkDone(WordSearchGenerator.WordAnswer answer)
    {
        m_doneLine.enabled = answer.Line != null;
        m_doneLine.color = answer.Color;
    }
}
