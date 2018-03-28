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
            Debug.LogWarning(stateManager.gameObject + "has a RogueState: " + this + "\t\tCurrent State:" + stateManager.State, stateManager);
            stopState = true;
        }
        else
        {
            hasStarted = true;
            yield return null;
        }
    }
    public virtual IEnumerator ExitState(BaseState nextState)
    {
        stopState = true;
        yield return null;
    }

    //State Updates
    public virtual void Update()
    {
        if (stateManager.IsPaused)
            UpdatePaused();

        else if (inTransition)
            UpdateTransition();

        else UpdateState();

        elapsedTime += Time.deltaTime;
    }

    public virtual void FixedUpdate()
    {
        if (!stateManager.IsPaused)
            UpdatePhysics();
    }

    protected abstract void UpdatePhysics();
    protected abstract void UpdateState();
    protected abstract void UpdatePaused();
    protected abstract void UpdateTransition();

    //Trigger Functions
    public abstract void OnTriggerEnter(Collider collider);
    public abstract void OnTriggerStay(Collider collider);
    public abstract void OnTriggerExit(Collider collider);

    //Colission Functions
    public abstract void OnCollisionEnter(Collision collision);
    public abstract void OnCollisionStay(Collision collision);
    public abstract void OnCollisionExit(Collision collision);
}