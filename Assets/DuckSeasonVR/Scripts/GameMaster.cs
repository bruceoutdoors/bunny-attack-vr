using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public CurrentWord currentWord;
    public float SpawnTimeInterval;
    public GameObject HUD;
    
    public FortMachinery Fort;
    public ScrabbleMan scrabbleMan;
    public GameObject[] DuckTypes;
    public RendezvousPoint[] SpawningPoints;

    bool stopSpawning = false;

    void Start()
    {
        Events.instance.AddListener<GameStateChangeEvent>(OnGameStateChangeEvent);
    }

    private void OnGameStateChangeEvent(GameStateChangeEvent e)
    {
        switch (e.State)
        {
            case GameState.START:
                GameStart();
                break;
            case GameState.END:
                GameEnd(e.PlayerWin);
                break;
        }
    }

    public void TriggerGameStart()
    {
        GameStateChangeEvent ev = new GameStateChangeEvent();
        ev.State = GameState.START;
        Events.instance.Raise(ev);
    }

    private void GameStart()
    {
        // HUD must be set active first before firing events.
        HUD.SetActive(true);

        currentWord.ClearWord();
        scrabbleMan.ResetWordsRemainingAndScore();
        playerHealth.ResetHealth();
        stopSpawning = false;
        InvokeRepeating("SpawnDucks", 0.0f, SpawnTimeInterval);
        
        Fort.Resurrect();
    }

    private void GameEnd(bool playerWin)
    {
        stopSpawning = true;
        HUD.SetActive(false);
        StartCoroutine(RemoveAllBunnies());
        Fort.Burry();
    }

    IEnumerator RemoveAllBunnies()
    {
        yield return new WaitForSeconds(2.0f);
        GameObject[] bunnies = GameObject.FindGameObjectsWithTag("ScrabbleTile");
        foreach (GameObject obj in bunnies)
        {
            Destroy(obj);
        }
    }

    public void SpawnDucks()
    {
        if (stopSpawning)
        {
            stopSpawning = false;
            CancelInvoke();
            return;
        }

        foreach (RendezvousPoint p in SpawningPoints)
        {
            GameObject duck = DuckTypes[UnityEngine.Random.Range(0, DuckTypes.Length)];
            GameObject d = Instantiate(duck, p.transform.position, p.transform.rotation);
            DuckMover dm = d.GetComponent<DuckMover>();
            dm.Rendezvous = p;
            dm.playerHealth = playerHealth;
            ScrabbleTileDat t = scrabbleMan.GetRandomScrabbleTileDat();

            if (t == null)
            {
                Debug.LogError("NO MORE TILES! YOU SHOULDN'T CREATE ANYMORE!");
                return;
            }

            d.GetComponent<ScrabbleTileController>().SetLetter(t.TileChar);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
