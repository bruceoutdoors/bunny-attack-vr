using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextButton : MonoBehaviour
{
    public Color NormalColor = Color.white;
    public Color OnHoverColor = Color.yellow;
    public Text ButtonText;

    Color normalColor;

    // Use this for initialization
    void Start()
    {
        ButtonText.color = NormalColor;
    }

    public void OnGazeEnter()
    {
        ButtonText.color = OnHoverColor;
    }

    public void OnGazeExit()
    {
        ButtonText.color = NormalColor;
    }
}
