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
    }

}
