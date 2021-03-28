using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatUIController : MonoBehaviour
{
    public ScrabbleMan scrabbleMan;
    public AudioSource LossSting;
    public AudioSource WinSting;
    public AudioSource StartSting;
    public AudioSource NewHighScoreSting;
    public GameObject GameStartScreen;
    public GameObject GameOverScreen;
    public GameObject YouWinText;
    public GameObject YouLooseText;
    public GameObject HighScoreText;
    public GameObject NewHighScoreText;
    public Text FinalGameScoreText;
    public Light UISpotLight;

    const string highscore = "HighScore";
    GameObject canvasParent;
    // Use this for initialization
    void Start()
    {
        canvasParent = GameStartScreen.transform.parent.gameObject;
        GameStartScreen.SetActive(true);
        GameOverScreen.SetActive(false);
        UISpotLight.enabled = true;
        UISpotLight.color = Color.white;
        Events.instance.AddListener<GameStateChangeEvent>(OnGameStateChangeEvent);
        //PlayerPrefs.DeleteKey(highscore);
        DisplayHighScore();
    }

    void DisplayHighScore()
    {
        int score = 0;

        if (PlayerPrefs.HasKey(highscore))
        {
            score = PlayerPrefs.GetInt(highscore);
        }

        HighScoreText.SetActive(true);
        HighScoreText.GetComponent<Text>().text = string.Format("HIGH SCORE: {0}", score);
    }

    private void OnGameStateChangeEvent(GameStateChangeEvent e)
    {
        switch (e.State)
        {
            case GameState.END:
                bool hasNewHighScore = false;
                if (PlayerPrefs.HasKey(highscore))
                {
                    if (scrabbleMan.CurrentScore > PlayerPrefs.GetInt(highscore))
                    {
                        hasNewHighScore = true;
                    }
                }
                else
                {
                    hasNewHighScore = true;
                }

                
                UISpotLight.enabled = true;
                GameOverScreen.SetActive(true);
                FinalGameScoreText.enabled = true;
                FinalGameScoreText.text = string.Format("SCORE: {0}", scrabbleMan.CurrentScore);

                if (e.PlayerWin)
                {
                    if (hasNewHighScore)
                    {
                        NewHighScoreSting.Play();
                        FinalGameScoreText.enabled = false;
                        PlayerPrefs.SetInt(highscore, scrabbleMan.CurrentScore);
                        NewHighScoreText.SetActive(true);
                        NewHighScoreText.GetComponent<Text>().text = string.Format("NEW HIGH SCORE: {0}!!", scrabbleMan.CurrentScore);
                        NewHighScoreText.GetComponent<TextBlink>().StartBlink();
                    }
                    else
                    {
                        WinSting.Play();
                        DisplayHighScore();
                    }

                    UISpotLight.color = Color.yellow;
                    YouWinText.SetActive(true);
                    YouLooseText.SetActive(false);
                }
                else
                {
                    DisplayHighScore();
                    UISpotLight.color = Color.red;
                    LossSting.Play();
                    YouWinText.SetActive(false);
                    YouLooseText.SetActive(true);
                }
                break;
            case GameState.START:
                UISpotLight.enabled = false;
                GameOverScreen.SetActive(false);
                GameStartScreen.SetActive(false);
                HighScoreText.SetActive(false);
                NewHighScoreText.SetActive(false);
                StartSting.Play();
                break;
        }
    }

}
