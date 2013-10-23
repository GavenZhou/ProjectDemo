using UnityEngine;
using System.Collections;

public class particleControl : MonoBehaviour {
	
	public GameObject[] mParticle;
	
	public bool turnOn;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(turnOn)
		{
			turnOn = false;
			PlayParticle();
		}
	}
	
	void PlayParticle()
	{
		foreach(GameObject x in mParticle)
		{
			x.particleSystem.Play();
		}
	}
}
