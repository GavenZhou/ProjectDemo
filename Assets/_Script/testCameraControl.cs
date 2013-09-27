using UnityEngine;
using System.Collections;

public class testCameraControl : MonoBehaviour {
	
	private GameObject playerObj;
	
	// Use this for initialization
	void Start () {
		
		playerObj = GameObject.FindGameObjectWithTag("player");
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(transform.position != playerObj.transform.position)
			transform.position = playerObj.transform.position;
	
	}
}
