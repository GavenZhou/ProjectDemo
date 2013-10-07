using UnityEngine;
using System.Collections.Generic;

public interface ISpawn {
    void OnSpawn();
    void OnDespawn();
}

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
    public GameObjectPool dropItemPool;


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
        dropItemPool.Init(true);
    }

    public void Reset() {

        //
        mobPool1.Reset();
        mobPool2.Reset();
        dropItemPool.Reset();
    }

    public void Clear() {

        // 
        mobPool1.Clear();
        mobPool2.Clear();
        dropItemPool.Clear();
    }


    ///////////////////////////////////////////////////////////////////////////////
    // Spawner & Despawner
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // mob spawner
    // ------------------------------------------------------------------

    public Mob SpawnMob(int _type, Vector3 _pos) {
        Mob mob;
        if (_type == 0) {
            mob = mobPool1.Request<Mob>(_pos);
        }
        else {
            mob = mobPool2.Request<Mob>(_pos);
        }
        mob.OnSpawn();
        return mob;
    }

    public void DespawnMob(Mob _mob) {
       
        GameObjectPool pool;
        if (_mob.mobTemplate == 0) {
            pool = mobPool1;
        }
        else {
            pool = mobPool2;
        }
        _mob.OnDespawn();
        pool.Return(_mob.gameObject);
    }

    // ------------------------------------------------------------------ 
    // drop spawner
    // ------------------------------------------------------------------

    public DropItem SpawnDrop(Vector3 _pos) {

        DropItem drop = dropItemPool.Request<DropItem>(_pos);
        drop.OnSpawn();
        return drop;
    }

    public void DespawnDrop(DropItem _drop) {
       
        _drop.OnDespawn();
        dropItemPool.Return(_drop.gameObject);
    }
}

