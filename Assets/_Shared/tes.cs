using UnityEngine;
using System.Collections;

public class tes : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		this.transform.Rotate(0,10*Time.deltaTime,0);
	
	}
}
