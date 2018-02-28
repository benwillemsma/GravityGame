using System.Collections;
using UnityEngine;

public abstract class BaseState
{
    protected StateManager stateManager;

    protected bool hasStarted = false;
    protected bool stopState = false;

    protected float elapsedTime;
    protected bool inTransition = false;
    public bool InTransition
    {
        get {return inTransition; }
        set {inTransition = value; }
    }

    //Constructor
    public BaseState(StateManager stateM)
    {
        stateManager = stateM;
        elapsedTime = 0;
    }

    //Transition Functions
    public virtual IEnumerator EnterState(BaseState prevState)
    {
        if (this != stateManager.State)
        {
            Debug.LogWarning(stateManager.gameObject + "has a RogueState: " + this + "\t\tCurrent State:" + stateManager.State);
            stopState = true;
        }
        else
        {
            hasStarted = true;
            yield return null;
            stateManager.StartState(this);
        }
    }
    public virtual IEnumerator ExitState(BaseState nextState)
    {
        stopState = true;
        yield return null;
    }
    public void StopState()
    {
        stopState = false;
    }

    //State Loops
    public virtual IEnumerator Update()
    {
        while (!stopState)
        {
            if (stateManager.IsPaused)
                yield return stateManager.StartCoroutine(UpdatePaused());
            else if (inTransition)
                yield return stateManager.StartCoroutine(UpdateTransition());
            else
                yield return stateManager.StartCoroutine(UpdateState());

            elapsedTime += Time.deltaTime;
        }
    }

    public virtual IEnumerator FixedUpdate()
    {
        while (!stopState)
        {
            if (!stateManager.IsPaused)
                UpdatePhysics();
            yield return new WaitForFixedUpdate();
        }
    }

    //State Updates
    protected abstract void UpdatePhysics();
    protected abstract IEnumerator UpdateState();
    protected abstract IEnumerator UpdatePaused();
    protected abstract IEnumerator UpdateTransition();

    //Trigger Functions
    public abstract void OnTriggerEnter(Collider collider);
    public abstract void OnTriggerStay(Collider collider);
    public abstract void OnTriggerExit(Collider collider);

    //Colission Functions
    public abstract void OnCollisionEnter(Collision collision);
    public abstract void OnCollisionStay(Collision collision);
    public abstract void OnCollisionExit(Collision collision);
}