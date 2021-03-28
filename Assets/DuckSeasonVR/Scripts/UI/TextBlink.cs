using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBlink : MonoBehaviour
{
    public float BlinkInterval = 0.4f;
    public int BlinkCount = 5;

    Text textComponent;
    bool isBlinking = false;
    float halfBlickInterval;

    // Use this for initialization
    void Start()
    {
        halfBlickInterval = BlinkInterval / 2;
        textComponent = GetComponent<Text>();
    }

    public void StartBlink()
    {
        if (isBlinking) return;

        StartCoroutine(StartBlinking());
    }

    IEnumerator StartBlinking()
    {
        isBlinking = true;

        for (int i = 0; i < BlinkCount; i++)
        {
            if (!enabled) yield break;
            if (textComponent == null)
            {
                textComponent = GetComponent<Text>();
            }

            textComponent.enabled = false;
            yield return new WaitForSeconds(halfBlickInterval);
            textComponent.enabled = true;
            yield return new WaitForSeconds(halfBlickInterval);
        }

        isBlinking = false;
    }

    private void OnDisable()
    {
        textComponent.enabled = true;
    }
}
