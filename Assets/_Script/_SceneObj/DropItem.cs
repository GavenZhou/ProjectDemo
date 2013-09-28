using UnityEngine;
using System.Collections;

public class DropItem : SceneObj {

    public override void Init(int _id) {
        type = SceneObjType.DropItem;
        base.Init(_id);
    }

}
