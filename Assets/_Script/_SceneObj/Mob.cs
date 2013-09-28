using UnityEngine;
using System.Collections;

public class Mob : Actor {


    public override void Init(int _id) {
        type = SceneObjType.Player;
        base.Init(_id);
    }

}
