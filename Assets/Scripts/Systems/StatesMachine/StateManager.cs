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
        StartCoroutine(HandleStateTransition(newState));
        CurrentState = State.ToString();
    }
    protected IEnumerator HandleStateTransition(BaseState newState)
    {
        State.InTransition = true;

        RemoveTriggers(State);
        RemoveCollisions(State);

        yield return StartCoroutine(State.ExitState(newState));
        State.InTransition = false;

        BaseState prevState = State;
        State = newState;

        State.InTransition = true;
        yield return StartCoroutine(State.EnterState(prevState));

        SetTriggers(State);
        SetCollisions(State);

        State.InTransition = false;

        prevState = null;
    }

    // Trigger Delegates
    delegate void TriggerDelegate(Collider collider);
    private TriggerDelegate triggerEnter;
    private TriggerDelegate triggerStay;
    private TriggerDelegate triggerExit;
    delegate void CollisionDelegate(Collision collision);
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
        if(!m_isPaused && State != null && !State.InTransition) triggerEnter.Invoke(other);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!m_isPaused && State != null && !State.InTransition) triggerStay.Invoke(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!m_isPaused && State != null && !State.InTransition) triggerExit.Invoke(other);
    }

    // Unity Collision Functions Call Current State Collision Functions
    private void OnCollisionEnter(Collision collision)
    {
        if (!m_isPaused && State != null && !State.InTransition) collisionExit.Invoke(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!m_isPaused && State != null && !State.InTransition) collisionStay.Invoke(collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!m_isPaused && State != null && !State.InTransition) collisionExit.Invoke(collision);
    }
}