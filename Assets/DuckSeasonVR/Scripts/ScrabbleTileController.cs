using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrabbleTileController : MonoBehaviour
{
    public char Letter;
    public GameObject BloodSplatter;
    public GvrAudioSource CollectSound;
    public GvrAudioSource DeathSound;
    Animator anim;

    public ScrabbleTileDat TileDat
    {
        get { return _TileDat; }
        set
        {
            _TileDat = value;

            // because we reuse this class for UI render also
            Image img = GetComponent<Image>();
            if (img == null)
            {
                GetComponentInChildren<SpriteRenderer>().sprite = _TileDat.TileSprite;
            }
            else
            {
                img.sprite = _TileDat.TileSprite;
            }
        }
    }

    private ScrabbleTileDat _TileDat = null;

    // Use this for initialization
    void Start()
    {
        //img = GetComponent<Image>();
        anim = GetComponent<Animator>();
        SetLetter(Letter);
    }

    public void SetLetter(char letter)
    {
        Debug.Assert(letter >= 65 && letter <= 90, "Letter must be between A-Z");

        TileDat = ScrabbleMan.ScrabbleTiles[letter - 65];

        Letter = letter;
    }

    public void Collect()
    {
        CollectSound.Play();
        KillYourself();

        // ops we still need the renderer if we collect haha!
        GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

    public void KillYourself()
    {
        DeathSound.Play();
        //GetComponentInChildren<Renderer>().enabled = false;
        foreach (var c in GetComponents<Collider>())
        {
            c.enabled = false;
        }
        GetComponentInChildren<SpriteRenderer>().enabled = false; // hide scrabble tile
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

        anim.SetTrigger("TriggerDeath");
    }

    public void BloodRelease(RaycastHit hit)
    {
        BloodSplatter.transform.position = hit.point;
        BloodSplatter.GetComponent<ParticleSystem>().Play();
    }
}
