using UnityEngine;
using System.Collections;

public class Actor : SceneObj {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public string Name {
        get;
        set;
    }

    public int Hp {
        get;
        set;
    }

    public int MaxHp {
        get;
        set;
    }

    private bool isDied;
    public bool IsDied {
        get { return isDied; }
        protected set { isDied = value; }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // virtual
    ///////////////////////////////////////////////////////////////////////////////

    public virtual void Attack() {

    }

<<<<<<< HEAD
    public virtual bool Hurt(SceneObj _object, object _param) {
=======
    public virtual void Hurt(SceneObj _object) {
>>>>>>> parent of cde666f... 怪物受精
        if (Hp <= 0) {
            Dead(_object);
        }
    }

    public virtual void Dead(SceneObj _object) {
        IsDied = true;
    }
}
