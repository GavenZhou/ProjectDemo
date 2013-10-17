using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

    [System.NonSerialized]
    public Camera mainCamera;

    void Start() {
        mainCamera = OrbitCameraCtrl.instance.mainCamera;
    }

    void LateUpdate() {
        if (mainCamera != null)  {
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}
