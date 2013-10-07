using UnityEngine;
using System.Collections;

public class TestScene : MonoBehaviour {

    public static TestScene instance;

    public float mobDetectorBegin = 10.0f;
    public float mobDetectorSpan = 2.0f;
    public float maxMobInScene = 10;
    public float totalMobCount = 50;

    void Awake() {
        instance = this;
        SceneMng scene = new SceneMng();
        scene.Init();
    }

    void Start() {
        InvokeRepeating("UpdateFPS", 0.0f, 1.0f);
        InvokeRepeating("UpdateMobDetector", mobDetectorBegin, mobDetectorSpan);
    }

    void OnGUI() {
        GUI.Label(new Rect(0, Screen.height - 20, 100, 20), "fps: " + fps.ToString());
    }

    void Update() {
        ++frames;
        //if (Time.timeScale != timeScale) {
        //    Time.timeScale = timeScale;
        //}


        if (Input.GetKeyDown(KeyCode.F2)) {
            CombatUtility.DropGenerator(SceneMng.instance.mainPlayer.transform.position + Vector3.right * 4);
        }
    }

    // ------------------------------------------------------------------ 
    // frame control
    // ------------------------------------------------------------------

    protected int frames = 0;
    protected float fps = 0.0f;
    protected float lastInterval = 0.0f;
//    public float timeScale = 1.0f;

    void UpdateFPS () {
        float timeNow = Time.realtimeSinceStartup;
        fps = frames / (timeNow - lastInterval);
        frames = 0;
        lastInterval = timeNow;
    }

    void UpdateMobDetector() {
        CombatUtility.MobGeneratorDetector();
    }
}
