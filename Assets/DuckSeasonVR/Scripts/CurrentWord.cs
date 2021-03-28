using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentWord : MonoBehaviour
{
    public GameObject HUDTile;
    public string TestWord;

    [HideInInspector]
    public string Word;

    // Use this for initialization
    void Start()
    {
        foreach (char c in TestWord)
        {
            AppendLetterToCurrentWord(c);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AppendLetterToCurrentWord(char c)
    {
        Word += c;
        var hudTile = Instantiate(HUDTile, transform);
        var tileRender = hudTile.GetComponent<ScrabbleTileController>();
        tileRender.SetLetter(c);
    }

    public void ClearWord()
    {
        Word = "";
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
