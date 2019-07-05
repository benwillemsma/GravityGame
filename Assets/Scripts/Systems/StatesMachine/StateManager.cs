using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public BaseState State;
    private bool m_isPaused;
    public bool IsPaused
    {
        get {return m_isPaused; }
        set { m_isPaused = value; }
    }

    [Space(10),SerializeField,Header("Debuging Info")]
    private string CurrentState;

    protected void Start()
    {
        //Initialize trigger Delegates
        SetTriggers(State);
        SetCollisions(State);

        State.EnterState(null);
        CurrentState = State.ToString();
    }

    protected void Update()
    {
        State.Update();
    }

    protected void FixedUpdate()
    {
        State.FixedUpdate();
    }

    //State Functions
    public void ChangeState(BaseState newState)
    {
        HandleStateTransition(newState);
        CurrentState = State.ToString();
    }
    protected void HandleStateTransition(BaseState newState)
    {
        //Exit State
        State.ExitState(newState);

        RemoveTriggers(State);
        RemoveCollisions(State);

        //Set New State
        BaseState prevState = State;
        State = newState;

        //Enter State
        SetTriggers(newState);
        SetCollisions(newState);

        State.EnterState(prevState);

        prevState = null;
    }

    // Trigger Delegates
    private TriggerDelegate triggerEnter;
    private TriggerDelegate triggerStay;
    private TriggerDelegate triggerExit;
    private CollisionDelegate collisionEnter;
    private CollisionDelegate collisionStay;
    private CollisionDelegate collisionExit;
    
    private void RemoveTriggers(BaseState state)
    {
        triggerEnter -= state.OnTriggerEnter;
        triggerStay -= state.OnTriggerStay;
        triggerExit -= state.OnTriggerExit;
    }
    private void SetTriggers(BaseState state)
    {
        triggerEnter += state.OnTriggerEnter;
        triggerStay += state.OnTriggerStay;
        triggerExit += state.OnTriggerExit;
    }

    private void RemoveCollisions(BaseState state)
    {
        collisionEnter -= state.OnCollisionEnter;
        collisionStay -= state.OnCollisionStay;
        collisionExit -= state.OnCollisionExit;
    }
    private void SetCollisions(BaseState state)
    {
        collisionEnter += state.OnCollisionEnter;
        collisionStay += state.OnCollisionStay;
        collisionExit += state.OnCollisionExit;
    }

    // Unity Trigger Functions Call Current State Tirgger Functions
    private void OnTriggerEnter(Collider other)
    {
        if(!m_isPaused && State != null) triggerEnter.Invoke(other);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!m_isPaused && State != null) triggerStay.Invoke(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!m_isPaused && State != null) triggerExit.Invoke(other);
    }

    // Unity Collision Functions Call Current State Collision Functions
    private void OnCollisionEnter(Collision collision)
    {
        if (!m_isPaused && State != null) collisionExit.Invoke(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!m_isPaused && State != null) collisionStay.Invoke(collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!m_isPaused && State != null) collisionExit.Invoke(collision);
    }
}