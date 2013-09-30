using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

    [System.NonSerialized]
    public Camera camera;

    void Start() {
        camera = testCameraControl.instance.mainCamera;
    }

    void LateUpdate() {
        if (camera != null)  {
            transform.rotation = camera.transform.rotation;
        }
    }
}
