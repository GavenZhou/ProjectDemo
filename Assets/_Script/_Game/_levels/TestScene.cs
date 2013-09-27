using UnityEngine;
using System.Collections;

public class TestScene : Level {

    new void Awake() {
        base.Awake();
        levelName = GameLevel.TestScene.ToString();
    }

    void OnGUI() {

        GUI.Label(new Rect(Screen.width / 2 - 40, Screen.height / 2 - 15, 80, 30), levelName);
    }

    protected override void EnterRuningState() {
        base.EnterRuningState();

        if (Input.GetKeyUp(KeyCode.F1)) {
            Game.instance.LoadLevel("Test");
        }
        else if (Input.GetKeyUp(KeyCode.F2)) {
            Game.instance.LoadLevel("Login");
        }
        else if (Input.GetKeyUp(KeyCode.F3)) {
            Game.instance.LoadLevel("Strategy");
        }
        else if (Input.GetKeyUp(KeyCode.F4)) {
            Game.instance.LoadLevel("Battle");
        }
    }
}
