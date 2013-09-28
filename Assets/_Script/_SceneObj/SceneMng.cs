
using System.Collections.Generic;
using UnityEngine;

public struct ObjectID {
    public int typeID;
    public int objectID;

    public ObjectID(int _type, int _id) {
        typeID = _type;
        objectID = _id;
    }

    public override bool Equals(object _obj) {
        if (_obj is ObjectID) {
            ObjectID id = (ObjectID)_obj;
            return id.typeID == typeID && id.objectID == objectID;
        }
        return false;
    }

    public bool Equals(ObjectID _obj) {
        return _obj.typeID == typeID && _obj.objectID == objectID;
    }

    public override int GetHashCode() {
        return typeID.GetHashCode() ^ objectID.GetHashCode();
    }
}

public class SceneMng {

    ///////////////////////////////////////////////////////////////////////////////
    // variable
    ///////////////////////////////////////////////////////////////////////////////

    public static SceneMng instance;
    Dictionary<ObjectID, SceneObj> sceneObjs = new Dictionary<ObjectID, SceneObj>();


    ///////////////////////////////////////////////////////////////////////////////
    // public
    ///////////////////////////////////////////////////////////////////////////////

    public SceneMng() {
        instance = this;
    }

    public void Init() {

    }
    
    public void AddSceneObj(SceneObj _obj) {

        SceneObj oldObj = null;
        ObjectID oid = new ObjectID((int)_obj.type, _obj.id);
        if (sceneObjs.TryGetValue(oid, out oldObj)) {
            if (_obj != oldObj) {
                RemoveSceneObj(oldObj);
            }
            else {
                Debug.LogError("AddSceneObj() error, already exists the sceneObj (" + _obj.type + ", " + _obj.id + ").");
                return;
            }
        }
        sceneObjs.Add(oid, _obj);
        _obj.scene = this;

#if DebugOutput
        Debug.Log("add scene obj id:" + _obj.OID.GetHashCode());
#endif
    }

    public void RemoveSceneObj(SceneObj _obj) {
        ObjectID oid = new ObjectID((int)_obj.type, _obj.id);
        _obj.scene = null;
        sceneObjs.Remove(oid);
    }

    public SceneObj GetSceneObj(ObjectID _id) {
        SceneObj obj = null;
        if (sceneObjs.TryGetValue(_id, out obj)) {
            return obj;
        }
        return null;
    }

    public SceneObj GetSceneObj(SceneObjType _type, int _id) {
        ObjectID oid = new ObjectID((int)_type, _id);
        return GetSceneObj(oid);
    }

    public List<T> GetSceneObjs<T>() where T : SceneObj {
        List<T> lst = new List<T>();
        foreach (SceneObj obj in sceneObjs.Values) {
            if (obj is T) {
                lst.Add(obj as T);
            }
        }
        return lst;
    }
}
