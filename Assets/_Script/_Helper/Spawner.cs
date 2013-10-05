using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // static
    ///////////////////////////////////////////////////////////////////////////////

    public static Spawner instance;

    ///////////////////////////////////////////////////////////////////////////////
    // prefabs
    ///////////////////////////////////////////////////////////////////////////////

    public GameObjectPool mobPool1;
    public GameObjectPool mobPool2;


    ///////////////////////////////////////////////////////////////////////////////
    // funcitons
    ///////////////////////////////////////////////////////////////////////////////

    public void Start() {
        instance = this;
        Init();
    }

    public void Init() {
    
        //
        mobPool1.Init(true);
        mobPool2.Init(true);
    }

    public void Reset() {

        //
        mobPool1.Reset();
        mobPool2.Reset();
    }

    public void Clear() {

        // 
        mobPool1.Clear();
        mobPool2.Clear();
    }


    ///////////////////////////////////////////////////////////////////////////////
    // Spawner & Despawner
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // mob Spawner
    // ------------------------------------------------------------------

    public Mob SpawnMob(int _type, Vector3 _pos) {

        Mob mob;
        if (_type == 1) {
            mob = mobPool1.Request<Mob>(_pos);
        }
        else {
            mob = mobPool2.Request<Mob>(_pos);
        }
        return mob;
    }

    public void DespawnMob(Mob _mob) {
       
        GameObjectPool pool;
        if (_mob.mobTemplate == 1) {
            pool = mobPool1;
        }
        else {
            pool = mobPool2;
        }
        _mob.OnDespawn();
        pool.Return(_mob.gameObject);
    }


}

