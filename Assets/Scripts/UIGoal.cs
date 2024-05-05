public class UIGoal : UIEntry
{
    public void Setup(WordSearchGenerator.WordAnswer wordSearchAnswer)
    {
        base.SetupText(
            wordSearchAnswer.WordEntry.word, 
            wordSearchAnswer.WordEntry.description
        );
    }
}
