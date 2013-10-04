using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

    public GameObjectPool mobPool1;
    public GameObjectPool mobPool2;


    ///////////////////////////////////////////////////////////////////////////////
    // funcitons
    ///////////////////////////////////////////////////////////////////////////////

    public void Init() {
    
        //
        mobPool1.Init();
        mobPool2.Init();
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

    public GameObject SpawnMob(int _type, Vector3 _pos) {

        GameObject go;
        if (_type == 1) {
            go = mobPool1.Request(_pos);
        }
        else {
            go = mobPool2.Request(_pos);
        }
        return go;
    }

    public void DespawnMob(Mob _mob) {
        _mob.OnDespawn();
        mobPool1.Return(_mob.gameObject);
    }
}

