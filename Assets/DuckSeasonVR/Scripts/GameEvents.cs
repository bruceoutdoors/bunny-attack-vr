using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All game events are listed in this file

public enum GameState
{
    START, END
}

public class GameStateChangeEvent : IGameEvent
{
    public GameState State;
    public bool PlayerWin;
}

public class UpdateHealthEvent : IGameEvent
{
    public int Difference;
    public int CurrentLife;
}

public class UpdateScoreEvent : IGameEvent
{
    public string Word;
    public int NewScore;
    public int Difference;
    public int WordsRemaining;
}