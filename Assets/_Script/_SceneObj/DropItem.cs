using UnityEngine;
using System.Collections;

public class DropItem : SceneObj, ISpawn {

    ///////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////

    public float attractRange;
    public float deadRange;
    public float attractDuration;

    bool attracted = false;
    bool isDied = false;


    ///////////////////////////////////////////////////////////////////////////////
    // virtual 
    ///////////////////////////////////////////////////////////////////////////////

    public override void Init(int _id) {
        
        base.Init(_id);
        type = SceneObjType.DropItem;
        attracted = isDied = false;
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

        StartCoroutine(AttractTo_Coroutine(_object));
    }

    public void Dead(SceneObj _object) {

        isDied = true;
        if (_object) {
            _object.SendMessage("OnPickup", this);
        }
        Spawner.instance.DespawnDrop(this);
    }


    ///////////////////////////////////////////////////////////////////////////////
    // auxiliary
    ///////////////////////////////////////////////////////////////////////////////

    IEnumerator AttractTo_Coroutine(SceneObj _object) {

        float timer = 0.0f;
        float duration = attractDuration;
        Vector3 start = transform.position;

        while ( timer <= duration ) {

            if (_object == null) yield break;

            float ratio = timer / duration;
            Vector3 end = _object.transform.position + new Vector3( 0.0f, 1f, 0.0f );
            Vector3 pos = Vector3.Lerp(start, end, ratio);
            transform.position = pos;

            //
            timer += Time.deltaTime;
            yield return 0;
        }
        this.Dead(_object);
    }



    void Update() {

        if (!attracted && !isDied) {
            Actor actor = SceneMng.instance.mainPlayer;
            float magnitude = (actor.transform.position - this.transform.position).sqrMagnitude;
            if (magnitude <= deadRange * deadRange) {
                this.Dead(actor);
            }
            else if (magnitude <= attractRange * attractRange) {
                this.AttractTo(actor);
            }
        }
    }
}
