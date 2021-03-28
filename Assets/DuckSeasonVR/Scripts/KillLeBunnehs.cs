using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillLeBunnehs : MonoBehaviour
{
    public ParticleSystem PlasmaExplosion;
    public GvrAudioSource PlasmaBlastSound;
    public GameObject rechargingEffects;
    public float BlastIntervalDuration = 3.0f;
    public float BlastRadius = 10.0f;
    public Text RechargingText;

    int shootableMask;
    float timer = 0;
    bool canBlast = true;

    // Use this for initialization
    void Start()
    {
        shootableMask = LayerMask.GetMask("Shootable");
        RechargingText.enabled = false;
        rechargingEffects.SetActive(false);
        Events.instance.AddListener<GameStateChangeEvent>(OnGameStateChange);
    }

    private void OnGameStateChange(GameStateChangeEvent e)
    {
        if (e.State == GameState.START)
        {
            rechargingEffects.SetActive(false);
            RechargingText.enabled = false;
            timer = 0;
            canBlast = true;
        }
    }

    private void Update()
    {
        // recharging...
        if (!canBlast)
        {
            timer -= Time.deltaTime;
            RechargingText.text = string.Format("RECHARGING ({0:0%})...", 
                (BlastIntervalDuration - timer) / BlastIntervalDuration);
            if (timer <= 0)
            {
                rechargingEffects.SetActive(false);
                RechargingText.enabled = false;
                canBlast = true;
            }
        }
    }

    public void FuckThoseBunnies()
    {
        if (!canBlast) return;

        canBlast = false;
        RechargingText.enabled = true;
        timer = BlastIntervalDuration;
        rechargingEffects.SetActive(true);

        PlasmaExplosion.Play();
        PlasmaBlastSound.Play();
        Collider[] colliders = Physics.OverlapSphere(transform.position, BlastRadius, shootableMask);
        foreach (Collider c in colliders)
        {
            if (c.tag == "ScrabbleTile")
            {
                ScrabbleTileController stc = c.gameObject.GetComponent<ScrabbleTileController>();
                stc.KillYourself();
            }
        }
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Gizmos.DrawWireSphere(transform.position, BlastRadius);
#endif
    }
}
