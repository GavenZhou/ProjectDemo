using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    ///////////////////////////////////////////////////////////////////////////////
    // Event
    ///////////////////////////////////////////////////////////////////////////////

    public class Event : System.EventArgs
    {
        static public readonly int UNKNOWN = -1;
        static public readonly int NULL = 0;
        static public readonly int FINISHED = 1;
        public const int USER_FIELD = 1000;

        // properties
        public int id = UNKNOWN;

        // functions
        public Event(int _id = -1) {
            id = _id;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // State
    ///////////////////////////////////////////////////////////////////////////////

    [System.Serializable]
    public class State
    {
        public enum Mode
        {
            Exclusive,
            Parallel,
        }

        ///////////////////////////////////////////////////////////////////////////////
        // properties
        ///////////////////////////////////////////////////////////////////////////////

        public string name = "";
        public Mode mode = Mode.Exclusive;
        public List<Transition> transitionList = new List<Transition>();
        protected List<State> currentStates = new List<State>();
        protected List<State> children = new List<State>();

        protected State parent_ = null;
        public State parent {
            set {
                if (parent_ != value) {
                    State oldParent = parent_;

                    while (parent_ != null) {
                        if (parent_ == this) {
                            return;
                        }
                        parent_ = parent_.parent;
                    }

                    //
                    if (oldParent != null) {
                        if (oldParent.initState == this)
                            oldParent.initState = null;
                        oldParent.children.Remove(this);
                    }

                    //
                    if (value != null) {
                        value.children.Add(this);
                        if (value.children.Count == 1)
                            value.initState = this;
                    }
                    parent_ = value;
                }
            }
            get { return parent_; }
        }

        protected Machine machine_ = null;
        public Machine machine {
            get {
                if (machine_ != null)
                    return machine_;

                State last = this;
                State root = parent;
                while (root != null) {
                    last = root;
                    root = root.parent;
                }
                machine_ = last.machine as Machine; // null is possible
                return machine_;
            }
        }

        protected State initState_ = null;
        public State initState {
            get { return initState_; }
            set {
                if (initState_ != value) {
                    if (value != null && children.IndexOf(value) == -1) {
                        Debug.LogError("error: You must use child state as initial state.");
                        initState_ = null;
                        return;
                    }
                    initState_ = value;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // event handles
        ///////////////////////////////////////////////////////////////////////////////

        public System.Action<State /* from */, State /* to */, Event> onEnter;
        public System.Action<State /* from */, State /* to */, Event> onExit;
        public System.Action<State /* current */, Event> onEvent;
        public System.Action<State /* current */> onAction;


        ///////////////////////////////////////////////////////////////////////////////
        // functions
        ///////////////////////////////////////////////////////////////////////////////

        public State(string _name, State _parent = null) {
            name = _name;
            parent = _parent;
        }

        public void ClearCurrentStatesRecursively() {
            currentStates.Clear();
            for (int i = 0; i < children.Count; ++i) {
                children[i].ClearCurrentStatesRecursively();
            }
        }

        // ------------------------------------------------------------------ 
        // Transition
        // ------------------------------------------------------------------

        public T Add<T>(State _targetState) where T : Transition, new() {
            T newTranstion = new T();
            newTranstion.source = this;
            newTranstion.target = _targetState;
            transitionList.Add(newTranstion);
            return newTranstion;
        }

        public T Add<T>(State _targetState, int _id) where T : EventTransition, new() {
            T newTranstion = Add<T>(_targetState);
            newTranstion.eventID = _id;
            return newTranstion;
        }

        public void TestTransitions(ref List<Transition> _validTransitions, Event _event) {
            for (int i = 0; i < currentStates.Count; ++i) {
                State activeChild = currentStates[i];

                bool hasTranstion = false;
                for (int j = 0; j < activeChild.transitionList.Count; ++j) {
                    Transition transition = activeChild.transitionList[j];
                    if (transition.TestEvent(_event)) {
                        _validTransitions.Add(transition);
                        hasTranstion = true;
                        break;
                    }
                }
                if (!hasTranstion) {
                    activeChild.TestTransitions(ref _validTransitions, _event);
                }
            }
        }

        // ------------------------------------------------------------------ 
        // State switch
        // ------------------------------------------------------------------ 

        public void EnterStates(Event _event, State _toEnter, State _toExit) {
            currentStates.Add(_toEnter);
            _toEnter.OnEnter(_toExit, _toEnter, _event);

            if (_toEnter.children.Count != 0) {
                if (_toEnter.mode == State.Mode.Exclusive) {
                    if (_toEnter.initState != null) {
                        _toEnter.EnterStates(_event, _toEnter.initState, _toExit);
                    }
                } else {
                    for (int i = 0; i < _toEnter.children.Count; ++i) {
                        _toEnter.EnterStates(_event, _toEnter.children[i], _toExit);
                    }
                }
            }
        }

        public void ExitStates(Event _event, State _toEnter, State _toExit) {
            _toExit.ExitAllStates(_event, _toEnter);
            _toExit.OnExit(_toExit, _toEnter, _event);
            currentStates.Remove(_toExit);
        }

        protected void ExitAllStates(Event _event, State _toEnter) {
            for (int i = 0; i < currentStates.Count; ++i) {
                State activeChild = currentStates[i];
                activeChild.ExitAllStates(_event, _toEnter);
                activeChild.OnExit(activeChild, _toEnter, _event);
            }
            currentStates.Clear();
        }

        // ------------------------------------------------------------------ 
        // Action
        // ------------------------------------------------------------------ 

        public void OnAction() {
            if (onAction != null) {
                onAction(this);
            }
            for (int i = 0; i < currentStates.Count; ++i) {
                currentStates[i].OnAction();
            }
        }

        public void OnEvent(Event _event) {
            if (onEvent != null) {
                onEvent(this, _event);
            }
            for (int i = 0; i < currentStates.Count; ++i) {
                currentStates[i].OnEvent(_event);
            }
        }

        public void OnEnter(State _from, State _to, Event _event) {
            if (onEnter != null) {
                onEnter(_from, _to, _event);
            }
        }

        public void OnExit(State _from, State _to, Event _event) {
            if (onExit != null) {
                onExit(_from, _to, _event);
            }
        }

        // ------------------------------------------------------------------ 
        // Misc
        // ------------------------------------------------------------------

        public int TotalStates() {
            int count = 1;
            for (int i = 0; i < children.Count; ++i) {
                count += children[i].TotalStates();
            }
            return count;
        }

        public void ShowDebugInfo(int _level, bool _active, GUIStyle _textStyle) {
            _textStyle.normal.textColor = _active ? Color.green : new Color(0.5f, 0.5f, 0.5f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.Label(new string('\t', _level) + name, _textStyle, new GUILayoutOption[] { });
            GUILayout.EndHorizontal();

            for (int i = 0; i < children.Count; ++i) {
                State s = children[i];
                s.ShowDebugInfo(_level + 1, currentStates.IndexOf(s) != -1, _textStyle);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // FinalState
    ///////////////////////////////////////////////////////////////////////////////

    public class FinalState : State
    {
        public FinalState(string _name, State _parent = null)
            : base(_name, _parent) {
            onEnter += OnFinished;
        }

        void OnFinished(State _from, State _to, Event _event) {
            Machine stateMachine = machine;
            if (stateMachine != null) {
                stateMachine.Send(Event.FINISHED);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Machine
    ///////////////////////////////////////////////////////////////////////////////

    public class Machine : State
    {
        public enum MachineState
        {
            Running,
            Paused,
            Stopping,
            Stopped
        }

        //// DEBUG { 
        public bool showDebugInfo = true;
        public bool logDebugInfo = false;
        //// } DEBUG end 

        ///////////////////////////////////////////////////////////////////////////////
        // non-serializable
        ///////////////////////////////////////////////////////////////////////////////

        protected MachineState machineState = MachineState.Stopped;

        // NOTE: startState --transition--> initState 
        protected State startState = new State("startState");
        protected List<Event>[] eventBuffer = new List<Event>[2] { new List<Event>(), new List<Event>() };
        protected int curEventBufferIdx = 0;
        protected int nextEventBufferIdx = 1;
        protected bool isUpdating = false;
        protected List<Transition> validTransitions = new List<Transition>();

        ///////////////////////////////////////////////////////////////////////////////
        // event handler
        ///////////////////////////////////////////////////////////////////////////////

        public System.Action onStart;
        public System.Action onStop;

        ///////////////////////////////////////////////////////////////////////////////
        // functions
        ///////////////////////////////////////////////////////////////////////////////

        public Machine()
            : base("machine") {
        }

        public void Restart() {
            Stop();
            Start();
        }

        public void Start() {
            if (machineState == MachineState.Running ||
                 machineState == MachineState.Paused) {

                Debug.LogError("Error: The machine is running now.");
                return;
            }

            machineState = MachineState.Running;
            if (onStart != null)
                onStart();

            Event nullEvent = new Event(Event.NULL);
            if (mode == State.Mode.Exclusive) {
                if (initState != null) {
                    EnterStates(nullEvent, initState, startState);
                } else {
                    Debug.LogError("Error: can't find initial state in " + name);
                }
            } else {
                for (int i = 0; i < children.Count; ++i) {
                    EnterStates(nullEvent, children[i], startState);
                }
            }
        }

        public void Stop() {
            if (machineState == MachineState.Stopped)
                return;

            if (isUpdating) {
                machineState = MachineState.Stopping;
            } else {
                ProcessStop();
            }
        }

        protected void ProcessStop() {
            eventBuffer[0].Clear();
            eventBuffer[1].Clear();
            ClearCurrentStatesRecursively();

            if (onStop != null)
                onStop();

            machineState = MachineState.Stopped;
        }

        public void Update() {
            if (machineState == MachineState.Paused ||
                 machineState == MachineState.Stopped)
                return;

            isUpdating = true;

            if (machineState != MachineState.Stopping) {
                int tmp = curEventBufferIdx;
                curEventBufferIdx = nextEventBufferIdx;
                nextEventBufferIdx = tmp;

                //
                bool doStop = false;
                List<Event> eventList = eventBuffer[curEventBufferIdx];
                for (int i = 0; i < eventList.Count; ++i) {
                    if (HandleEvent(eventList[i])) {
                        doStop = true;
                        break;
                    }
                }
                eventList.Clear();

                if (doStop) {
                    Stop();
                } else {
                    OnAction();
                }
            }

            isUpdating = false;

            if (machineState == MachineState.Stopping) {
                ProcessStop();
            }
        }

        public void Pause() { machineState = MachineState.Paused; }
        public void Resume() { machineState = MachineState.Running; }

        protected bool HandleEvent(Event _event) {

            OnEvent(_event);

            // 
            validTransitions.Clear();
            TestTransitions(ref validTransitions, _event);

            //
            ExitStates(_event, validTransitions);
            ExecTransitions(_event, validTransitions);
            EnterStates(_event, validTransitions);

            if (_event.id == Event.FINISHED) {
                bool canStop = true;
                for (int i = 0; i < currentStates.Count; ++i) {
                    if ((currentStates[i] is FinalState) == false) {
                        canStop = false;
                        break;
                    }
                }
                if (canStop) {
                    return true;
                }
            }
            return false;
        }

        public void Send(int _eventID) { Send(new Event(_eventID)); }
        public void Send(Event _event) {
            if (machineState == MachineState.Stopped)
                return;
            Debug.Log("Receive event: " + _event.id);
            eventBuffer[nextEventBufferIdx].Add(_event);
        }

        protected void EnterStates(Event _event, List<Transition> _transitionList) {
            for (int i = 0; i < _transitionList.Count; ++i) {
                Transition transition = _transitionList[i];
                State targetState = transition.target;
                if (targetState == null)
                    targetState = transition.source;

                if (targetState.parent != null)
                    targetState.parent.EnterStates(_event, targetState, transition.source);
            }
        }

        protected void ExitStates(Event _event, List<Transition> _transitionList) {
            for (int i = 0; i < _transitionList.Count; ++i) {
                Transition transition = _transitionList[i];

                if (transition.source.parent != null)
                    transition.source.parent.ExitStates(_event, transition.target, transition.source);
            }
        }

        protected void ExecTransitions(Event _event, List<Transition> _transitionList) {
            for (int i = 0; i < _transitionList.Count; ++i) {
                Transition transition = _transitionList[i];
                transition.OnTransition(_event);
            }
        }

        public void ShowDebugGUI(string _name, GUIStyle _textStyle) {
            GUILayout.Label("State Machine (" + _name + ")");
            showDebugInfo = GUILayout.Toggle(showDebugInfo, "Show States");
            logDebugInfo = GUILayout.Toggle(logDebugInfo, "Log States");
            if (showDebugInfo) {
                ShowDebugInfo(0, true, _textStyle);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Transition
    ///////////////////////////////////////////////////////////////////////////////

    [System.Serializable]
    public class Transition
    {
        ///////////////////////////////////////////////////////////////////////////////
        // properties
        ///////////////////////////////////////////////////////////////////////////////

        public State source = null;
        public State target = null;

        public Machine machine {
            get {
                if (source != null)
                    return source.machine;
                return null;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // handler
        ///////////////////////////////////////////////////////////////////////////////


        public delegate bool TestEventHandler(Transition _trans, Event _event);
        public delegate void TransitionHandler(Transition _trans, Event _event);

        public System.Func<Transition, Event, bool> onTestEvent;
        public System.Action<Transition, Event> onTransition;

        ///////////////////////////////////////////////////////////////////////////////
        // functions
        ///////////////////////////////////////////////////////////////////////////////

        public virtual bool TestEvent(Event _event) {
            if (onTestEvent != null) {
                return onTestEvent(this, _event);
            }
            return true;
        }

        public virtual void OnTransition(Event _event) {
            if (onTransition != null) {
                onTransition(this, _event);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // EventTransition
    ///////////////////////////////////////////////////////////////////////////////

    [System.Serializable]
    public class EventTransition : Transition
    {
        public int eventID = -1;

        public EventTransition() { }

        public EventTransition(int _eventID) {
            eventID = _eventID;
        }

        public override bool TestEvent(Event _event) {
            return _event.id == eventID;
        }
    }
}
