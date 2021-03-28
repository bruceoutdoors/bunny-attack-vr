using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int StartingHealth;
    public float InvulnerabilityTime = 2.5f;

    private int currentHealth;
    private bool canAttackPlayer = true;

    // Use this for initialization
    void Start()
    {
        //ResetHealth();
    }

    public void ResetHealth()
    {
        UpdateHealthEvent e = new UpdateHealthEvent();
        e.CurrentLife = StartingHealth;
        e.Difference = 0;
        Events.instance.Raise(e);
        currentHealth = StartingHealth;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void AffectHealth(int difference)
    {
        if (difference < 0 && !canAttackPlayer) return;

        currentHealth += difference;

        if (difference != 0)
        {
            UpdateHealthEvent e = new UpdateHealthEvent();
            e.CurrentLife = currentHealth;
            e.Difference = difference;
            Events.instance.Raise(e);

            if (difference < 0 && canAttackPlayer)
            {
                StartCoroutine(invulnerabilityTimeStart());
            }
        } 

        if (currentHealth == 0)
        {
            GameStateChangeEvent e = new GameStateChangeEvent();
            e.State = GameState.END;
            e.PlayerWin = false;
            Events.instance.Raise(e);
        }
    }

    private IEnumerator invulnerabilityTimeStart()
    {
        canAttackPlayer = false;

        yield return new WaitForSeconds(InvulnerabilityTime);

        canAttackPlayer = true;
    }
}
