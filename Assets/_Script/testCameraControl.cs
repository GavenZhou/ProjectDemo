using UnityEngine;
using System.Collections;

public class testCameraControl : MonoBehaviour {
	
	GameObject playerObj;

    [System.NonSerialized]
    public Camera mainCamera;
    public static testCameraControl instance;

	void Awake() {
        instance = this;
        mainCamera = GetComponentInChildren(typeof(Camera)) as Camera;
    }

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
