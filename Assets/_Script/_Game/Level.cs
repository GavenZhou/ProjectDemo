using UnityEngine;
using System.Collections;
using StateMachine;

public class Level : MyMachine
{
    public enum EventType
    {
        Run = StateMachine.Event.USER_FIELD + 1,
        Pause,
        Resume,
        Over,
        Restart,
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Properties
    ///////////////////////////////////////////////////////////////////////////////
    public string levelName = "none";
    [System.NonSerialized]
    public bool deinited = false;


    ///////////////////////////////////////////////////////////////////////////////
    // Function
    ///////////////////////////////////////////////////////////////////////////////

    protected override void InitStateMachine() {
        base.InitStateMachine();

        State start = new State("start", stateMachine);
        start.onEnter += (_from, _to, _event) => { EnterStartState(); };

        State running = new State("running", stateMachine);
        running.onEnter += (_from, _to, _event) => { EnterRuningState(); };
        running.onExit += (_from, _to, _event) => { ExitRuningState(); };
        running.onAction += (_cur) => { UpdateRunningState(); };

        State pause = new State("pause", stateMachine);
        pause.onEnter += (_from, _to, _event) => { };
        pause.onExit += (_from, _to, _event) => { };

        State over = new State("over", stateMachine);
        over.onEnter += (_from, _to, _event) => { EnterOverState(); };
        over.onExit += (_from, _to, _event) => { };

        start.Add<EventTransition>(running, (int)EventType.Run);
        start.Add<EventTransition>(over, (int)EventType.Over);

        running.Add<EventTransition>(over, (int)EventType.Over);
        running.Add<EventTransition>(pause, (int)EventType.Pause);

        pause.Add<EventTransition>(running, (int)EventType.Resume);
        pause.Add<EventTransition>(over, (int)EventType.Over);

        over.Add<EventTransition>(start, (int)EventType.Restart);
    }

    protected virtual void Deinited()
    {
        Debug.Log("Deinited");
    }

    //protected void OnGUI()
    //{
    //    stateMachine.ShowDebugInfo(0, false, new GUIStyle());
    //}

    public void Run () {
        stateMachine.Send((int)EventType.Run);
    }

    public void Pause () {
        stateMachine.Send((int)EventType.Pause);
    }

    public void Resume () {
        stateMachine.Send((int)EventType.Resume);
    }

    public void Restart () {
        stateMachine.Send((int)EventType.Restart);
    }

    public void Over () {
        stateMachine.Send((int)EventType.Over);
    }

    // ------------------------------------------------------------------ 
    // Action
    // ------------------------------------------------------------------

    protected virtual void EnterStartState() {
        deinited = false;
        Run();
    }
 
    protected virtual void EnterRuningState() {
        Game.instance.gameLevel = this;
    }

    protected virtual void UpdateRunningState() {
    }

    protected virtual void ExitRuningState() {
    }

    protected virtual void EnterOverState() {
        Deinited();
        deinited = true;
    }
}
