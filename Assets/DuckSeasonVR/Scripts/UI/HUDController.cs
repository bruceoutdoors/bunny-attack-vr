using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {
    public Text ScoreText;
    public Text CurrentWordNotification;
    public Text WordsRemainingText;
    public Text HealthRemaining;

    private const string _ScoreDisplayFmt = "Score: {0}";
    private const string _WordsRemainFmt = "Words Remain: {0}";
    private const string _HealthFmt = "Health: {0}";
    private TextBlink healthRemainBlink;

    private void Start()
    {
        CurrentWordNotification.enabled = false;
        healthRemainBlink = HealthRemaining.gameObject.GetComponent<TextBlink>();
    }

    void OnEnable()
    {
        Events.instance.AddListener<UpdateScoreEvent>(OnScoreUpdated);
        Events.instance.AddListener<UpdateHealthEvent>(OnHealthUpdated);
    }

    void OnDisable()
    {
        Events.instance.RemoveListener<UpdateScoreEvent>(OnScoreUpdated);
        Events.instance.RemoveListener<UpdateHealthEvent>(OnHealthUpdated);
        CurrentWordNotification.enabled = false;
    }

    void OnScoreUpdated(UpdateScoreEvent e)
    {
        if (e.Difference < 0)
        {
            FlashMessage("INVALID WORD", Color.red, 1.0f);
            return;
        }

        WordsRemainingText.text = string.Format(_WordsRemainFmt, e.WordsRemaining);
        ScoreText.text = string.Format(_ScoreDisplayFmt, e.NewScore);

        if (e.Difference > 0)
        {
            FlashMessage(string.Format("\"{0}\" = {1} Points!", e.Word, e.Difference), Color.green, 1.5f);
        }  
    }

    private void OnHealthUpdated(UpdateHealthEvent e)
    {
        HealthRemaining.text = string.Format(_HealthFmt, e.CurrentLife);
        if (e.Difference < 0)
        {
            healthRemainBlink.StartBlink();
        }
    }

    public void FlashMessage(string msg, Color color, float seconds = 2.0f)
    {
        CurrentWordNotification.enabled = true;
        CurrentWordNotification.text = msg;
        CurrentWordNotification.color = color;

        IEnumerator coroutine = WaitToDisable(seconds);
        StartCoroutine(coroutine);
    }

    IEnumerator WaitToDisable(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        CurrentWordNotification.enabled = false;
    }
}
