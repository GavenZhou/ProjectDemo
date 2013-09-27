using UnityEngine;
using System.Collections;
using StateMachine;

public enum GameLevel
{
    None,
    Login,
    Map,
    Battle,

    TestScene,
}

public class Game : MyMachine
{
    public enum EventType
    {
        GameLoading = StateMachine.Event.USER_FIELD + 1,
        GameRunning,
        Quit,   // to do
    }

    public enum GameState {
        Start,
        Loading,
        Running,
        Quit
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Properties
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // public
    // ------------------------------------------------------------------
    public static Game instance = null;
    [System.NonSerialized]
    public Level gameLevel = null;
    [System.NonSerialized]
    public Network network = null;
    public GameState curState;


    // ------------------------------------------------------------------ 
    // private
    // ------------------------------------------------------------------


    ///////////////////////////////////////////////////////////////////////////////
    // Function
    ///////////////////////////////////////////////////////////////////////////////

    new void Awake() {
        base.Awake();
        instance = this;
        DontDestroyOnLoad(this);
    }

    new void Start() {
        base.Start();
        InvokeRepeating("UpdateFPS", 0.0f, 1.0f);
    }

    protected override void InitStateMachine() {

        base.InitStateMachine();

        State start = new State("start", stateMachine);
        start.onEnter += (_from, _to, _event) => EnterStartState(_from, _to, _event);
        start.onExit += (_from, _to, _event) => { };

        State loading = new State("loading", stateMachine);
        loading.onEnter += (_from, _to, _event) => { EnterLoadingState(_from, _to, _event); };
        loading.onExit += (_from, _to, _event) => { };

        State running = new State("running", stateMachine);
        running.onEnter += (_from, _to, _event) => { curState = GameState.Running; };
        running.onExit += (_from, _to, _event) => { };
        running.onAction += UpdateRunningState;

        State quit = new State("quit", stateMachine);
        quit.onEnter += (_from, _to, _event) => { curState = GameState.Quit; };

        start.Add<EventTransition>(loading, (int)EventType.GameLoading);
        loading.Add<EventTransition>(running, (int)EventType.GameRunning);
    }

    void OnGUI() {
        GUI.Label(new Rect(0, Screen.height - 20, 100, 20), "fps: " + fps.ToString());
    }

    new void Update() {
        ++frames;
        if (Time.timeScale != timeScale) {
            Time.timeScale = timeScale;
        }
        base.Update();
    }

    // ------------------------------------------------------------------ 
    // frame control
    // ------------------------------------------------------------------

    protected int frames = 0;
    protected float fps = 0.0f;
    protected float lastInterval = 0.0f;
    public float timeScale = 1.0f;

    void UpdateFPS () {
        float timeNow = Time.realtimeSinceStartup;
        fps = frames / (timeNow - lastInterval);
        frames = 0;
        lastInterval = timeNow;
    }

    // ------------------------------------------------------------------ 
    // public
    // ------------------------------------------------------------------

    public bool LoadLevel(string _name) {
        if (gameLevel == null || gameLevel.levelName != _name) {
            StartCoroutine(LoadingCoroutine(_name));
            return true;
        }
        
        Debug.LogError("Already in this level: " + _name);
        return false;
    }


    // ------------------------------------------------------------------ 
    // coroutine
    // ------------------------------------------------------------------

    IEnumerator LoadingCoroutine(string _level) {
        yield return null;

        if (gameLevel != null) {
            gameLevel.Over();
            while (gameLevel != null && gameLevel.deinited == false) { yield return null; }
            gameLevel = null;
        }
        yield return Application.LoadLevelAsync(_level);
        yield return Resources.UnloadUnusedAssets();

        while (gameLevel == null) { yield return null; }
        stateMachine.Send((int)EventType.GameRunning);
    }

    // ------------------------------------------------------------------ 
    // action
    // ------------------------------------------------------------------

    void EnterStartState(State _from, State _to, StateMachine.Event _event) {
        curState = GameState.Start;
        stateMachine.Send((int)EventType.GameLoading);
    }

    void EnterLoadingState(State _from, State _to, StateMachine.Event _event) {

        curState = GameState.Loading;

        GameLevel t = GameLevel.Login;
        LoadLevel(t.ToString());
    }

    void UpdateRunningState(State _state) {
    }
}
