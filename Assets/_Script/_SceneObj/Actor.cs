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

    private bool isDie;
    public bool IsDie {
        get { return isDie; }
        protected set { isDie = value; }
    }

    public virtual void Attack() {

    }

    public virtual void Hurt(SceneObj _object) {
        if (Hp <= 0) {
            Dead(_object);
        }
    }

    public virtual void Dead(SceneObj _object) {
        IsDie = true;
    }
}
