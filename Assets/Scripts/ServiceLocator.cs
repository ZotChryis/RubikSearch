using UnityEngine;

/// <summary>
/// This is the service locator pattern. All shared game systems will register to this singleton,
/// and is the main way systems communicate.
/// </summary>
public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance { get; private set; }
    
    // A text asset I found online here: https://github.com/eddydn/DictionaryDatabase/blob/master/EDMTDictionary.json
    // It holds all the english dictionary in JSON
    [SerializeField] private TextAsset DictionaryJSON;
    
    public Game Game { get; private set; }
    public Board Board { get; private set; }
    public UIGoals Goals { get; private set; }
    public WordSearchGenerator WordSearchGenerator{ get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        
        // Find all MonoBehaviour game systems.
        Game = FindObjectOfType<Game>();
        Board = FindObjectOfType<Board>();
        Goals = FindObjectOfType<UIGoals>();
        
        // Create non MonoBehaviour game systems and helpers.
        WordSearchGenerator = new WordSearchGenerator(DictionaryJSON);
    }

    
}
