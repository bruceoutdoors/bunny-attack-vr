using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrabbleTileDat
{
    public ScrabbleTileDat(int id, char c, int points, int quantity)
    {
        _Id = id;
        _TileChar = c;
        _Points = points;
        _Quantity = quantity;
        _TileSprite = ScrabbleMan.TileSprites[id];
        _Texture = _TileSprite.GetTexture2D();
    }

    public int Id { get { return _Id; } }
    public char TileChar { get { return _TileChar; } }
    public int Score { get { return _Points; } }
    public int Quantity { get { return _Quantity; } }
    public Sprite TileSprite { get { return _TileSprite; } }
    public Texture2D TileTexture { get { return _Texture; } }

    private int _Id;
    private char _TileChar;
    private int _Points;
    private int _Quantity;
    private Sprite _TileSprite;
    private Texture2D _Texture;
}

public class ScrabbleMan : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public GameObject ScrabbleTile;
    public CurrentWord currentWord;
    public Transform PlayerLocation;
    public int TileCountPerGame = 7;
    public int WordsRemaining = 10;
    public AudioSource AcceptWordSound;
    public AudioSource WrongWrongSound;

    public static Sprite[] TileSprites;
    public static WordGameDict EnglishDictionary;
    public static List<ScrabbleTileDat> ScrabbleTiles;
    public List<ScrabbleTileDat> ScrabbleBag;
    [HideInInspector]
    public int CurrentScore = 0;

    private int _CurrentTileCountInGame = 0;
    int currentWordsRemaining;

    void Awake()
    {
        TileSprites = Resources.LoadAll<Sprite>("ScrabbleTiles");
        EnglishDictionary = new WordGameDict("ospd");
        ScrabbleBag = new List<ScrabbleTileDat>();

        // hardcoding everything cause hackathon!!
        ScrabbleTiles = new List<ScrabbleTileDat>(new ScrabbleTileDat[]
        {
            new ScrabbleTileDat(0 , 'A', 1 , 9 ),
            new ScrabbleTileDat(1 , 'B', 3 , 2 ),
            new ScrabbleTileDat(2 , 'C', 3 , 2 ),
            new ScrabbleTileDat(3 , 'D', 2 , 4 ),
            new ScrabbleTileDat(4 , 'E', 1 , 12),
            new ScrabbleTileDat(5 , 'F', 4 , 2 ),
            new ScrabbleTileDat(6 , 'G', 2 , 3 ),
            new ScrabbleTileDat(7 , 'H', 4 , 2 ),
            new ScrabbleTileDat(8 , 'I', 1 , 9 ),
            new ScrabbleTileDat(9 , 'J', 8 , 1 ),
            new ScrabbleTileDat(10, 'K', 5 , 1 ),
            new ScrabbleTileDat(11, 'L', 1 , 4 ),
            new ScrabbleTileDat(12, 'M', 3 , 2 ),
            new ScrabbleTileDat(13, 'N', 1 , 6 ),
            new ScrabbleTileDat(14, 'O', 1 , 8 ),
            new ScrabbleTileDat(15, 'P', 3 , 2 ),
            new ScrabbleTileDat(16, 'Q', 10, 1 ),
            new ScrabbleTileDat(17, 'R', 1 , 6 ),
            new ScrabbleTileDat(18, 'S', 1 , 4 ),
            new ScrabbleTileDat(19, 'T', 1 , 6 ),
            new ScrabbleTileDat(20, 'U', 1 , 4 ),
            new ScrabbleTileDat(21, 'V', 4 , 2 ),
            new ScrabbleTileDat(22, 'W', 4 , 2 ),
            new ScrabbleTileDat(23, 'X', 8 , 1 ),
            new ScrabbleTileDat(24, 'Y', 4 , 2 ),
            new ScrabbleTileDat(25, 'Z', 10, 1 )
        });

        RefreshScrabbleBag();
    }

    void Start()
    {
        //ResetWordsRemainingAndScore();
    }

    public void ResetWordsRemainingAndScore()
    {
        currentWordsRemaining = WordsRemaining;
        CurrentScore = 0;
        var e = new UpdateScoreEvent();
        e.WordsRemaining = currentWordsRemaining;
        e.Difference = 0;
        e.NewScore = CurrentScore;
        e.Word = "";
        Events.instance.Raise(e);
    }

    private void FillGameWithTiles()
    {
        for (int i = _CurrentTileCountInGame; i < TileCountPerGame; i++)
        {
            AddRandomTileToGame();
        }
    }

    private void AddRandomTileToGame()
    {
        ScrabbleTileDat std = GetRandomScrabbleTileDat();

        if (std == null)
        {
            // no tiles remaining
            return;
        }

        Vector3 direction = Random.onUnitSphere;
        direction.y = Mathf.Clamp(direction.y, 0.5f, 1f);
        float distance = 2 * Random.value + 1.5f;
        Vector3 position = direction * distance;

        GameObject go = Instantiate(ScrabbleTile, position,
            Quaternion.LookRotation(position) * Quaternion.Euler(20, 0, 0));

        ScrabbleTileController str = go.GetComponent<ScrabbleTileController>();
        str.SetLetter(std.TileChar);

        _CurrentTileCountInGame++;
    }

    private void RefreshScrabbleBag()
    {
        ScrabbleBag.Clear();

        foreach (ScrabbleTileDat std in ScrabbleTiles)
        {
            for (int i = 0; i < std.Quantity; i++)
            {
                ScrabbleBag.Add(std);
            }
        }

        ScrabbleBag.Shuffle();
    }

    // Get a random tile. Returns null if there are no more remaining tiles.
    public ScrabbleTileDat GetRandomScrabbleTileDat()
    {
        if (ScrabbleBag.Count == 0)
        {
            RefreshScrabbleBag();
        }

        // select a random tile from a randomly shuffled bag:
        int randIdx = Random.Range(0, ScrabbleBag.Count);
        ScrabbleTileDat selectedTile = ScrabbleBag.Pop(randIdx);

        return selectedTile;
    }

    // returns value of word. Returns -1 if word is invalid
    public int GetWordScore(string word)
    {
        if (word == "")
        {
            return 0;
        }

        if (!EnglishDictionary.CheckWord(word))
        {
            return -1;
        }

        int score = 0;
        foreach (char c in word)
        {
            int idx = c - 65; // get index from ascii code
            ScrabbleTileDat tile = ScrabbleTiles[idx];
            score += tile.Score;
        }

        return score;
    }

    public void AcceptCurrentWord()
    {
        int score = GetWordScore(currentWord.Word);

        // ignore empty word
        if (score == 0) return;

        // clear word
        string word = currentWord.Word;
        _CurrentTileCountInGame -= currentWord.Word.Length;
        currentWord.ClearWord();

        CurrentScore += score;
        var e = new UpdateScoreEvent();
        e.Difference = score;
        e.NewScore = CurrentScore;
        e.Word = word;

        if (e.Difference > 0)
        {
            AcceptWordSound.Play();
            currentWordsRemaining -= 1;
        }
        else
        {
            // Wrong word!
            WrongWrongSound.Play();
            playerHealth.AffectHealth(-1);
        }

        e.WordsRemaining = currentWordsRemaining;

        Events.instance.Raise(e);

        if (currentWordsRemaining == 0)
        {
            var ev = new GameStateChangeEvent();
            ev.State = GameState.END;
            ev.PlayerWin = true;
            Events.instance.Raise(ev);
        }
    }
}
