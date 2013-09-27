using System.Collections;
using StateMachine;
using UnityEngine;

public class MyMachine : MonoBehaviour
{
    [System.NonSerialized]
    public Machine stateMachine = null;

    ///////////////////////////////////////////////////////////////////////////////
    // Function
    ///////////////////////////////////////////////////////////////////////////////

    protected void Awake() {
        stateMachine = new Machine();
        InitStateMachine();
    }

    protected void Start() {
        if (stateMachine != null) {
            stateMachine.Start();
        }
    }

    protected void Update () {
        if (stateMachine != null) {
            stateMachine.Update();
        }
	}

    protected virtual void InitStateMachine() {}

    public virtual void Reset() {
        if (stateMachine != null) {
            stateMachine.Restart();
        }
    }
}
