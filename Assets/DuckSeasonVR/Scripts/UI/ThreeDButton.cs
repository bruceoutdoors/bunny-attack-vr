using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ThreeDButton : MonoBehaviour
{
    public Color ButtonColor = Color.red;
    GvrAudioSource clickSound;
    Material buttonMat;

    // Use this for initialization
    void Start()
    {
        clickSound = GetComponent<GvrAudioSource>();
        var mr = GetComponent<MeshRenderer>();
        buttonMat = new Material(mr.material);
        mr.material = buttonMat;
        buttonMat.color = ButtonColor;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnGazeEnter()
    {
        buttonMat.SetColor("_EmissionColor", buttonMat.color);
    }

    public void OnGazeExit()
    {
        buttonMat.SetColor("_EmissionColor", Color.black);
    }

    public void ButtonClick()
    {
        clickSound.Play();
        DOTween.Sequence()
            .Append(transform.DOLocalMoveY(-0.025f, 0.1f))
            .Append(transform.DOLocalMoveY(0, 0.1f));
    }
}
