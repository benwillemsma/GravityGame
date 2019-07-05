using System.Collections;
using UnityEngine;

public abstract class BaseState
{
    protected StateManager stateManager;

    protected bool hasStarted = false;
    protected bool stopState = false;

    protected float elapsedTime;

    //Constructor
    public BaseState(StateManager stateM)
    {
        stateManager = stateM;
        elapsedTime = 0;
    }

    //Transition Functions
    public virtual void EnterState(BaseState prevState)
    {
        if (this != stateManager.State)
        {
            Debug.LogWarning(stateManager.gameObject + "has a RogueState: " + this + "\t\tCurrent State:" + stateManager.State, stateManager);
            stopState = true;
        }
        else hasStarted = true;
    }
    public virtual void ExitState(BaseState nextState)
    {
        stopState = true;
    }

    //State Updates
    public virtual void Update()
    {
        if (stateManager.IsPaused)
            UpdatePaused();
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

    //Trigger Functions
    public abstract void OnTriggerEnter(Collider collider);
    public abstract void OnTriggerStay(Collider collider);
    public abstract void OnTriggerExit(Collider collider);

    //Colission Functions
    public abstract void OnCollisionEnter(Collision collision);
    public abstract void OnCollisionStay(Collision collision);
    public abstract void OnCollisionExit(Collision collision);
}