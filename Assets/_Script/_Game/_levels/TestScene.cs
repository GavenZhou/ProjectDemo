using UnityEngine;
using System.Collections;

public class TestScene : MonoBehaviour {

    void Awake() {
        SceneMng scene = new SceneMng();
        scene.Init();
    }

    new void Start() {
        InvokeRepeating("UpdateFPS", 0.0f, 1.0f);
    }

    void OnGUI() {
        GUI.Label(new Rect(0, Screen.height - 20, 100, 20), "fps: " + fps.ToString());
    }

    new void Update() {
        ++frames;
        if (Time.timeScale != timeScale) {
            Time.timeScale = timeScale;
        }
    }

    // ------------------------------------------------------------------ 
    // frame control
    // ------------------------------------------------------------------

    protected int frames = 0;
    protected float fps = 0.0f;
    protected float lastInterval = 0.0f;
    public float timeScale = 1.0f;

    void UpdateFPS () {
        float timeNow = Time.realtimeSinceStartup;
        fps = frames / (timeNow - lastInterval);
        frames = 0;
        lastInterval = timeNow;
    }
}
