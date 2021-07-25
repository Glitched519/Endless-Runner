using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareExpand : MonoBehaviour {

    LensFlare flare;
    float time = 0;
	// Use this for initialization
	void Start () {
        flare = GetComponent<LensFlare>();
	}
	
	// Update is called once per frame
	void Update () {
        flare.brightness += 0.0001f;
        time += Time.deltaTime;
        flare.color = new Color(Mathf.Sin(time), Mathf.Cos(time), Mathf.Sin(time + Mathf.PI/2));
	}
}
