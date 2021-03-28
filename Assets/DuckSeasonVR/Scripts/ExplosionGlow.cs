using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionGlow : MonoBehaviour
{
    public float GlowSpeed;
    public Color GlowColor;

    SkinnedMeshRenderer smr;
    Material glowMat;

    // Use this for initialization
    void Start()
    {
        smr = GetComponent<SkinnedMeshRenderer>();
        var ms = smr.sharedMaterials;
        Material m = ms[0];
        glowMat = new Material(m);
        ms[0] = glowMat;
        smr.sharedMaterials = ms;
    }

    void Update()
    {
        float emission = Mathf.PingPong(Time.time, GlowSpeed);
        Color finalColor = GlowColor * Mathf.LinearToGammaSpace(emission);

        glowMat.SetColor("_EmissionColor", finalColor);
    }

    private void OnDisable()
    {
        //reset to no glow
        glowMat.SetColor("_EmissionColor", Color.black);
    }
}
