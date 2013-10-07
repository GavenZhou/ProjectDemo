using UnityEngine;
using System.Collections;

public class DropItem : SceneObj, ISpawn {

    ///////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////

    public float attractRange = 5.0f;
    public float deadRange = 1.0f;
    public float attractDuration = 0.5f;

    bool attracted = false;
    bool died = false;


    ///////////////////////////////////////////////////////////////////////////////
    // virtual 
    ///////////////////////////////////////////////////////////////////////////////

    public override void Init(int _id) {
        
        base.Init(_id);
        type = SceneObjType.DropItem;
        attracted = died = false;
    }

    public virtual void OnSpawn() {
        Init(CombatUtility.GenNextDropID());
        SceneMng.instance.AddSceneObj(this);
    }

    public virtual void OnDespawn() {
        // 
        gameObject.SetActive(false);
        SceneMng.instance.RemoveSceneObj(this);
    }

    ///////////////////////////////////////////////////////////////////////////////
    // public
    ///////////////////////////////////////////////////////////////////////////////

    public void AttractTo(SceneObj _object) {
        attracted = true;

    }

    public void Dead(SceneObj _object) {
        
        Spawner.instance.DespawnDrop(this);
    }


    void Update() {

        if (!attracted) {
            


        }
    }
}
