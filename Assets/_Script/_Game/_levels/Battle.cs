using UnityEngine;
using System.Collections;


public class Battle : Level
{
    ///////////////////////////////////////////////////////////////////////////////
    // Properties
    ///////////////////////////////////////////////////////////////////////////////
    public static Battle instance = null;
    public static GameInput input = null;
    public static GameObject mainPlayer = null; // 
    public static Camera mainCamera = null;

    ///////////////////////////////////////////////////////////////////////////////
    // Function
    ///////////////////////////////////////////////////////////////////////////////
    new void Awake() {
        base.Awake();
        levelName = GameLevel.Battle.ToString();
        instance = this;
        input = GetComponent<GameInput>();
        input.enabled = false;
    }

    void OnGUI() {
        if (GUI.Button(new Rect(Screen.width / 2 - 40, Screen.height / 2 - 15, 80, 30), levelName)) {
            Game.instance.LoadLevel(GameLevel.Map.ToString());
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // action
    ///////////////////////////////////////////////////////////////////////////////
    protected override void EnterRuningState() {
        base.EnterRuningState();

        mainPlayer = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        input.enabled = true;
    }
}
