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

    public virtual bool Hurt(SceneObj _object) {
        if (Hp <= 0) {
            Dead(_object);
			return true;
        }
		return false;
    }

    public virtual void Dead(SceneObj _object) {
        IsDied = true;
    }
}
