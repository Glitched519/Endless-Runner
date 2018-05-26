using System.Collections;
using UnityEngine;

public class Background : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();
	}
}
