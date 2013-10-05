using UnityEngine;
using System.Collections.Generic;

public class ObjPool<T> where T : class, new()
{
    public int size = 0;
    public int idx = 0;
    public T[] data;
    public T[] initData;

    public void Init(int _size) {
        size = _size;
        initData = new T[size];
        data = new T[size];
        for (int i = 0; i < size; ++i) {
            T obj = new T();
            initData[i] = obj;
            data[i] = initData[i];
        }
        idx = size - 1;
    }

    public void Reset() {
        for (int i = 0; i < size; ++i) {
            data[i] = initData[i];
        }
        idx = size - 1;
    }

    public T Request() {
        if (idx < 0) {
            Debug.LogError("Error: the objPool do not have enough free item.");
            return null;
        }

        T result = data[idx];
        --idx;
        return result;
    }

    public void Return(T _item) {
        ++idx;
        data[idx] = _item;
    }
}

[System.Serializable]
public class GameObjectPool {

    ///////////////////////////////////////////////////////////////////////////////
    // variable
    ///////////////////////////////////////////////////////////////////////////////

    public GameObject prefab;
    public int size = 0;

    [System.NonSerialized] public int idx = 0;
    [System.NonSerialized] public GameObject[] data;
    [System.NonSerialized] public GameObject[] initData;


    ///////////////////////////////////////////////////////////////////////////////
    // function
    ///////////////////////////////////////////////////////////////////////////////

    public void Init(bool _donDestroyOnLoad = true) {
        Init(prefab, size, _donDestroyOnLoad);
    }

    public void Init(GameObject _prefab, int _size, bool _donDestroyOnLoad) {
        prefab = _prefab;
        size = _size;
        initData = new GameObject[size];
        data = new GameObject[size];
        if (prefab != null) {
            for (int i = 0; i < size; ++i) {
                GameObject obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                if (_donDestroyOnLoad) GameObject.DontDestroyOnLoad(obj);
                initData[i] = obj;
                data[i] = initData[i];
            }
        }
        idx = size - 1;
    }

    public void Clear() {
        for (int i = 0; i < size; ++i) {
            if (initData[i]) {
                Game.DestroyObject(initData[i]);
            }
        }
        idx = 0;
        initData = null;
        data = null;
    }

    public void Reset() {
        for (int i = 0; i < size; ++i) {
            data[i] = initData[i];
        }
        idx = size - 1;
    }

    public GameObject Request() {
        if (idx < 0) {
            Debug.LogError("Error: the pool do not have enough free item.");
            return null;
        }

        GameObject result = data[idx];
        --idx;
        return result;
    }

    public GameObject Request(Vector2 _pos) {
        GameObject result = Request();
        result.SetActive(true);
        result.transform.position = new Vector3(_pos.x, _pos.y, result.transform.position.z);
        return result;
    }

    public GameObject Request(Vector3 _pos, Quaternion _rot) {
        GameObject result = Request();
        result.transform.position = _pos;
        result.transform.rotation = _rot;
        return result;
    }

    public T Request<T>() where T : MonoBehaviour {
        GameObject go = Request();
        if (go)
            return go.GetComponent<T>();
        return null;
    }

    public T Request<T>(Vector2 _pos) where T : MonoBehaviour {
        GameObject go = Request(_pos);
        if (go)
            return go.GetComponent<T>();
        return null;
    }

    public T Request<T>(Vector3 _pos, Quaternion _rot) where T : MonoBehaviour {
        GameObject go = Request(_pos, _rot);
        if (go) {
            return go.GetComponent<T>();
        }
        return null;
    }

    public void Return(GameObject _item) {
        ++idx;
        data[idx] = _item;
    }
}