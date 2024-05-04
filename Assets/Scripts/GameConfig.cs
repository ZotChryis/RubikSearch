using UnityEngine;

[CreateAssetMenu]
public class GameConfig : ScriptableObject
{
    public int Rows = 5;
    public int Cols = 5;
    public int WordCount = 3;
    public int MinWordLength = 3;
}
