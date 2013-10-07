using UnityEngine;

public class SelfRotate : MonoBehaviour {

    public float xRotate;
    public float yRotate;
    public float zRotate;
	
	// Update is called once per frame
	void Update () {
	    transform.Rotate(xRotate * Time.deltaTime, yRotate * Time.deltaTime, zRotate * Time.deltaTime);
	}
}
