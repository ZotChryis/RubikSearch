
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WordSearchGenerator
{
    public enum Direction
    {
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW
    }
    public struct WordSearch
    {
        public char[,] Characters;
        public List<WordAnswer> Answers;
    }

    public struct WordAnswer
    {
        public WordEntry WordEntry;
        public Board.Coordinate Start;
        public Board.Coordinate End;
        public Direction Direction;
    }

    [Serializable]
    public struct WordEntry
    {
        public string word;
        public string type;
        public string description;
    }

    private List<WordEntry> _wordEntries;

    public WordSearchGenerator(TextAsset dictionaryJSON)
    {
        WordEntry[] wordEntries = JsonHelper.GetJsonArray<WordEntry>(dictionaryJSON.text);
        _wordEntries = wordEntries.Where(PassesFilter).ToList();
        _wordEntries.ForEach(wordEntry => wordEntry.word = wordEntry.word.ToUpper());
    }

    // todo: make this in a coroutine and mask with a loading bar
    // Might want to consider check a word many times and improve the heuristic in choosing the words
    // For example, it might be better to look to place words that share a lot of letters with current answers
    public WordSearch GeneratePuzzle(int rows, int cols, int wordCount, int minWordLength)
    {
        var characters = new char[rows, cols];
        List<WordAnswer> answers = new List<WordAnswer>(wordCount);

        var successfulAnswers = 0;
        var invalidAttempts = 0;
        while (successfulAnswers < wordCount && invalidAttempts < 1000)
        {
            WordEntry randomWordEntry = GetRandomWordEntry(Random.Range(minWordLength, rows));
            if (IsDuplicateWord(answers, randomWordEntry.word))
            {
                continue;
            }
            
            if (PlaceWord(randomWordEntry, characters, out WordAnswer answer))
            {
                answers.Add(answer);
                successfulAnswers++;
                continue;
            }

            invalidAttempts++;
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (characters[r, c] == default)
                {
                    characters[r, c] = '-';//(char)('A' + Random.Range (0, 26));
                }
            }
        }

        return new WordSearch()
        {
            Characters = characters,
            Answers = answers
        };
    }

    private bool IsDuplicateWord(List<WordAnswer> answers, string word)
    {
        return answers.Any(a => a.WordEntry.word.ToUpper() == word.ToUpper());
    }

    private bool PassesFilter(WordEntry entry)
    {
        // We want wordCount of 3-5 length for now. The game supports these.
        var word = entry.word;
        if (word.Length < 3 || word.Length > 5)
        {
            return false;
        }

        // We only support wordCount that don't contain non-letter items, like hypens, apostraphes, etc.
        if (!word.All(Char.IsLetter))
        {
            return false;
        }

        return true;
    }

    private WordEntry GetRandomWordEntry(int length)
    {
        if (length < 3 || length > 5)
        {
            return default;
        }

        var possibleWordEntries = _wordEntries.Where(e => e.word.Length == length).ToList();
        return possibleWordEntries[Random.Range(0, possibleWordEntries.Count)];
    }

    private bool PlaceWord(WordEntry wordEntry, char[,] characters, out WordAnswer answer)
    {
        answer = default;

        int rows = characters.GetLength(0);
        int cols = characters.GetLength(1);

        // Maybe not the best solution, but just try a random spot on the board and try to fit all orientations
        // of the word (horizontal, vertical, diagonal, forward and reverse, etc) from that location
        int randomRow = Random.Range(0, rows);
        int randomCol = Random.Range(0, cols);

        List<WordAnswer> potentialLocations = new List<WordAnswer>();
        if (FitsWordWest(wordEntry, characters, randomRow, randomCol, out WordAnswer potentialAnswerW))
        {
            potentialLocations.Add(potentialAnswerW);
        }

        if (FitsWordEast(wordEntry, characters, randomRow, randomCol, out WordAnswer potentialAnswerE))
        {
            potentialLocations.Add(potentialAnswerE);
        }
        
        if (FitsWordNorth(wordEntry, characters, randomRow, randomCol, out WordAnswer potentialAnswerN))
        {
            potentialLocations.Add(potentialAnswerN);
        }
        
        if (FitsWordSouth(wordEntry, characters, randomRow, randomCol, out WordAnswer potentialAnswerS))
        {
            potentialLocations.Add(potentialAnswerS);
        }
        
        if (FitsWordNorthEast(wordEntry, characters, randomRow, randomCol, out WordAnswer potentialAnswerNE))
        {
            potentialLocations.Add(potentialAnswerNE);
        }
        
        if (FitsWordNorthWest(wordEntry, characters, randomRow, randomCol, out WordAnswer potentialAnswerNW))
        {
            potentialLocations.Add(potentialAnswerNW);
        }

        if (FitsWordSouthEast(wordEntry, characters, randomRow, randomCol, out WordAnswer potentialAnswerSE))
        {
            potentialLocations.Add(potentialAnswerSE);
        }
        
        if (FitsWordSouthWest(wordEntry, characters, randomRow, randomCol, out WordAnswer potentialAnswerSW))
        {
            potentialLocations.Add(potentialAnswerSW);
        }
        
        if (potentialLocations.Count == 0)
        {
            return false;
        }

        answer = potentialLocations[Random.Range(0, potentialLocations.Count)];

        switch (answer.Direction)
        {
            case Direction.N:
                PlaceWordNorth(answer, characters);
                break;
            case Direction.NE:
                PlaceWordNorthEast(answer, characters);
                break;
            case Direction.E:
                PlaceWordEast(answer, characters);
                break;
            case Direction.SE:
                PlaceWordSouthEast(answer, characters);
                break;
            case Direction.S:
                PlaceWordSouth(answer, characters);
                break;
            case Direction.SW:
                PlaceWordSouthWest(answer, characters);
                break;
            case Direction.W:
                PlaceWordWest(answer, characters);
                break;
            case Direction.NW:
                PlaceWordNorthWest(answer, characters);
                break;
        }
        
        return true;
    }

    private void PlaceWordEast(WordAnswer answer, char[,] characters)
    {
        string word = answer.WordEntry.word;
        for (int i = 0; i < word.Length; i++)
        {
            characters[answer.Start.Row, answer.Start.Col + i] = word[i];
        }
    }
    
    private void PlaceWordWest(WordAnswer answer, char[,] characters)
    {
        string word = answer.WordEntry.word;
        for (int i = 0; i < word.Length; i++)
        {
            characters[answer.Start.Row, answer.Start.Col - i] = word[i];
        }
    }
    
    private void PlaceWordNorth(WordAnswer answer, char[,] characters)
    {
        string word = answer.WordEntry.word;
        for (int i = 0; i < word.Length; i++)
        {
            characters[answer.Start.Row - i, answer.Start.Col] = word[i];
        }
    }
    
    private void PlaceWordSouth(WordAnswer answer, char[,] characters)
    {
        string word = answer.WordEntry.word;
        for (int i = 0; i < word.Length; i++)
        {
            characters[answer.Start.Row + i, answer.Start.Col] = word[i];
        }
    }
    
    private void PlaceWordNorthEast(WordAnswer answer, char[,] characters)
    {
        string word = answer.WordEntry.word;
        for (int i = 0; i < word.Length; i++)
        {
            characters[answer.Start.Row - i, answer.Start.Col + i] = word[i];
        }
    }
    
    private void PlaceWordNorthWest(WordAnswer answer, char[,] characters)
    {
        string word = answer.WordEntry.word;
        for (int i = 0; i < word.Length; i++)
        {
            characters[answer.Start.Row - i, answer.Start.Col - i] = word[i];
        }
    }
    
    private void PlaceWordSouthEast(WordAnswer answer, char[,] characters)
    {
        string word = answer.WordEntry.word;
        for (int i = 0; i < word.Length; i++)
        {
            characters[answer.Start.Row + i, answer.Start.Col + i] = word[i];
        }
    }
    
    private void PlaceWordSouthWest(WordAnswer answer, char[,] characters)
    {
        string word = answer.WordEntry.word;
        for (int i = 0; i < word.Length; i++)
        {
            characters[answer.Start.Row + i, answer.Start.Col - i] = word[i];
        }
    }

    private bool FitsWordEast(
        WordEntry wordEntry,
        char[,] characters,
        int row,
        int col,
        out WordAnswer answer
    )
    {
        answer = default;
        var word = wordEntry.word;

        // If there is no sufficient space, we can exit early
        int cols = characters.GetLength(1);
        if (col + word.Length > cols)
        {
            return false;
        }
        
        // Go through and ensure that either the space is free or matches the word's ordering
        for (int i = 0; i < word.Length; i++)
        {
            char wordLetter = word[i];
            char letter = characters[row, col + i];
            
            // If we find that there is something in the way that doesn't line up, then the word won't fit
            if (letter != default && letter != wordLetter)
            {
                return false;
            }
        }
        
        answer = new WordAnswer()
        {
            WordEntry = wordEntry,
            Start = new Board.Coordinate(row, col),
            End = new Board.Coordinate(row, col + (word.Length - 1)),
            Direction = Direction.E
        };
        return true;
    }

    private bool FitsWordWest(
        WordEntry wordEntry,
        char[,] characters,
        int row,
        int col,
        out WordAnswer answer
    )
    {
        answer = default;
        var word = wordEntry.word;

        // If there is no sufficient space, we can exit early
        if (col - word.Length < 0)
        {
            return false;
        }
        
        // Go through and ensure that either the space is free or matches the word's ordering
        for (int i = 0; i < word.Length; i++)
        {
            char wordLetter = word[i];
            char letter = characters[row, col - i];
            
            // If we find that there is something in the way that doesn't line up, then the word won't fit
            if (letter != default && letter != wordLetter)
            {
                return false;
            }
        }
        
        answer = new WordAnswer()
        {
            WordEntry = wordEntry,
            Start = new Board.Coordinate(row, col),
            End = new Board.Coordinate(row, col - (word.Length - 1)),
            Direction = Direction.W
        };
        return true;
    }

    private bool FitsWordNorth(
        WordEntry wordEntry,
        char[,] characters,
        int row,
        int col,
        out WordAnswer answer
    )
    {
        answer = default;
        var word = wordEntry.word;

        // If there is no sufficient space, we can exit early
        if (row - word.Length < 0)
        {
            return false;
        }
        
        // Go through and ensure that either the space is free or matches the word's ordering
        for (int i = 0; i < word.Length; i++)
        {
            char wordLetter = word[i];
            char letter = characters[row - i, col];
            
            // If we find that there is something in the way that doesn't line up, then the word won't fit
            if (letter != default && letter != wordLetter)
            {
                return false;
            }
        }
        
        answer = new WordAnswer()
        {
            WordEntry = wordEntry,
            Start = new Board.Coordinate(row, col),
            End = new Board.Coordinate(row - (word.Length - 1), col),
            Direction = Direction.N
        };
        return true;
    }

    private bool FitsWordSouth(
        WordEntry wordEntry,
        char[,] characters,
        int row,
        int col,
        out WordAnswer answer
    )
    {
        answer = default;
        var word = wordEntry.word;

        // If there is no sufficient space, we can exit early
        int rows = characters.GetLength(0);
        if (row + word.Length > rows)
        {
            return false;
        }
        
        // Go through and ensure that either the space is free or matches the word's ordering
        for (int i = 0; i < word.Length; i++)
        {
            char wordLetter = word[i];
            char letter = characters[row + i, col];
            
            // If we find that there is something in the way that doesn't line up, then the word won't fit
            if (letter != default && letter != wordLetter)
            {
                return false;
            }
        }
        
        answer = new WordAnswer()
        {
            WordEntry = wordEntry,
            Start = new Board.Coordinate(row, col),
            End = new Board.Coordinate(row + (word.Length - 1), col),
            Direction = Direction.S
        };
        return true;
    }

    private bool FitsWordNorthEast(
        WordEntry wordEntry,
        char[,] characters,
        int row,
        int col,
        out WordAnswer answer
    )
    {
        answer = default;
        var word = wordEntry.word;

        // If there is no sufficient space, we can exit early
        int cols = characters.GetLength(1);
        if (row - word.Length < 0)
        {
            return false;
        }
        if (col + word.Length > cols)
        {
            return false;
        }
        
        // Go through and ensure that either the space is free or matches the word's ordering
        for (int i = 0; i < word.Length; i++)
        {
            char wordLetter = word[i];
            char letter = characters[row - i, col + i];
            
            // If we find that there is something in the way that doesn't line up, then the word won't fit
            if (letter != default && letter != wordLetter)
            {
                return false;
            }
        }
        
        answer = new WordAnswer()
        {
            WordEntry = wordEntry,
            Start = new Board.Coordinate(row, col),
            End = new Board.Coordinate(row - (word.Length - 1),  col + (word.Length - 1)),
            Direction = Direction.NE
        };
        return true;
    }

    private bool FitsWordNorthWest(
        WordEntry wordEntry,
        char[,] characters,
        int row,
        int col,
        out WordAnswer answer
    )
    {
        answer = default;
        var word = wordEntry.word;

        // If there is no sufficient space, we can exit early
        if (row - word.Length < 0)
        {
            return false;
        }
        if (col - word.Length < 0)
        {
            return false;
        }
        
        // Go through and ensure that either the space is free or matches the word's ordering
        for (int i = 0; i < word.Length; i++)
        {
            char wordLetter = word[i];
            char letter = characters[row - i, col - i];
            
            // If we find that there is something in the way that doesn't line up, then the word won't fit
            if (letter != default && letter != wordLetter)
            {
                return false;
            }
        }
        
        answer = new WordAnswer()
        {
            WordEntry = wordEntry,
            Start = new Board.Coordinate(row, col),
            End = new Board.Coordinate(row - (word.Length - 1),  col - (word.Length - 1)),
            Direction = Direction.NW
        };
        return true;
    }

    private bool FitsWordSouthEast(
        WordEntry wordEntry,
        char[,] characters,
        int row,
        int col,
        out WordAnswer answer
    )
    {
        answer = default;
        var word = wordEntry.word;

        // If there is no sufficient space, we can exit early
        int rows = characters.GetLength(0);
        int cols = characters.GetLength(1);
        if (row + word.Length > rows)
        {
            return false;
        }
        if (col + word.Length > cols)
        {
            return false;
        }
        
        // Go through and ensure that either the space is free or matches the word's ordering
        for (int i = 0; i < word.Length; i++)
        {
            char wordLetter = word[i];
            char letter = characters[row + i, col + i];
            
            // If we find that there is something in the way that doesn't line up, then the word won't fit
            if (letter != default && letter != wordLetter)
            {
                return false;
            }
        }
        
        answer = new WordAnswer()
        {
            WordEntry = wordEntry,
            Start = new Board.Coordinate(row, col),
            End = new Board.Coordinate(row + (word.Length - 1),  col + (word.Length - 1)),
            Direction = Direction.SE
        };
        return true;
    }

    private bool FitsWordSouthWest(
        WordEntry wordEntry,
        char[,] characters,
        int row,
        int col,
        out WordAnswer answer
    )
    {
        answer = default;
        var word = wordEntry.word;

        // If there is no sufficient space, we can exit early
        int rows = characters.GetLength(0);
        if (row + word.Length > rows)
        {
            return false;
        }
        if (col - word.Length < 0)
        {
            return false;
        }
        
        // Go through and ensure that either the space is free or matches the word's ordering
        for (int i = 0; i < word.Length; i++)
        {
            char wordLetter = word[i];
            char letter = characters[row + i, col - i];
            
            // If we find that there is something in the way that doesn't line up, then the word won't fit
            if (letter != default && letter != wordLetter)
            {
                return false;
            }
        }
        
        answer = new WordAnswer()
        {
            WordEntry = wordEntry,
            Start = new Board.Coordinate(row, col),
            End = new Board.Coordinate(row + (word.Length - 1),  col - (word.Length - 1)),
            Direction = Direction.SW
        };
        return true;
    }

    public static class JsonHelper
    {
        public static T[] GetJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }
}