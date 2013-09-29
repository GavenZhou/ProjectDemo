using UnityEngine;
using System.Collections;

public class ParticleControl : MonoBehaviour {
	
	public GameObject[] particleChildren;
	
	public bool playParticle;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		PlayParticle();
	}
	
	
	public void PlayParticle()
	{
		foreach(GameObject child in particleChildren)
		{
			if(child.particleEmitter.emit != playParticle)
				child.particleEmitter.emit = playParticle;
		}
	}

}
