using UnityEngine;
using UnityEngine.UI;

public class UIGoal : UIEntry
{
    [SerializeField] private Image m_doneLine;

    public WordSearchGenerator.WordAnswer Answer { get; private set; }

    public void Setup(WordSearchGenerator.WordAnswer wordSearchAnswer)
    {
        Answer = wordSearchAnswer;
        MarkInProgress();
        
        base.SetupText(
            wordSearchAnswer.WordEntry.word, 
            wordSearchAnswer.WordEntry.description
        );
    }

    public void MarkDone(WordSearchGenerator.WordAnswer answer)
    {
        m_doneLine.enabled = true;
        m_doneLine.color = answer.Color;
    }

    public void MarkInProgress()
    {
        m_doneLine.enabled = false;
    }
}
