using UnityEngine;
using System.Collections;

public class Map : Level
{
    ///////////////////////////////////////////////////////////////////////////////
    // Properties
    ///////////////////////////////////////////////////////////////////////////////


    ///////////////////////////////////////////////////////////////////////////////
    // Function
    ///////////////////////////////////////////////////////////////////////////////
    new void Awake() {
        base.Awake();
        levelName = GameLevel.Map.ToString();
    }

    void OnGUI() {

        if (GUI.Button(new Rect(Screen.width / 2 - 40, Screen.height / 2 - 15, 80, 30), levelName)) {
            Game.instance.LoadLevel(GameLevel.Battle.ToString());
        }
    }
}
