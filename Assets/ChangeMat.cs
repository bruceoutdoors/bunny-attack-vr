using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMat : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Material mat = new Material(Shader.Find("Standard"));
        mat.SetTexture("_MainTex", ScrabbleMan.ScrabbleTiles[2].TileTexture);
        // set tiling
        mat.SetTextureScale("_MainTex", new Vector2(-12.63f, -18.82f));
        // set offset
        mat.SetTextureOffset("_MainTex", new Vector2(-0.09f, 0));
        SkinnedMeshRenderer r = GetComponent<SkinnedMeshRenderer>();
        Material[] mats = r.sharedMaterials;
        mats[2] = mat;
        GetComponent<SkinnedMeshRenderer>().sharedMaterials = mats;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
