using UnityEngine;
using System.Collections;


public enum SceneObjType {
    None,
    InteractiveObj,
    DropItem,
    Mob,
    Player,
}

public class SceneObj : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // variable
    ///////////////////////////////////////////////////////////////////////////////
    
    [System.NonSerialized]
    public SceneObjType type;
    [System.NonSerialized]
    public int id;

    // ------------------------------------------------------------------ 
    // reference
    // ------------------------------------------------------------------
    [System.NonSerialized] public SceneMng scene;


    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public ObjectID OID {
        get { return new ObjectID((int)type, id); }
    }


    ///////////////////////////////////////////////////////////////////////////////
    // mono
    ///////////////////////////////////////////////////////////////////////////////

    protected void OnDestroy() {
        Destroy(gameObject);
    }

    
    ///////////////////////////////////////////////////////////////////////////////
    // public
    ///////////////////////////////////////////////////////////////////////////////

    public virtual void Init(int _id) {
        id = _id;
    }

    public void Destroy() {
#if DebugOutput
        Debug.Log("Destroy SceneObj (" + type + ", " + id + ")");
#endif
        GameObject.Destroy(this);
    }
    

    ///////////////////////////////////////////////////////////////////////////////
    // static
    ///////////////////////////////////////////////////////////////////////////////

    public static void SetLayer(GameObject _gameObject, int _layer) {
        _gameObject.layer = _layer;
    }

    public static void SetTag(GameObject _gameObject, string _tagName) {
        _gameObject.tag = _tagName;
    }
}
