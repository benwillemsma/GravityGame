using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : CharacterData
{
    #region Public Variables
    [Space(10)]
    public float RunSpeed;
    public float JumpForce;
    public bool toggleCrouch;

    #endregion

    #region Trigger Input
    private InputAxis rightTriggerState = new InputAxis("RightTrigger", AxisType.Trigger);
    private InputAxis leftTriggerState = new InputAxis("LeftTrigger", AxisType.Trigger);

    public InputAxis RightTrigger
    {
        get { return rightTriggerState; }
    }
    public InputAxis LeftTrigger
    {
        get { return leftTriggerState; }
    }
    #endregion

    #region Main
    protected override void Awake()
    {
        base.Awake();
        if (!GameManager.player)
        {
            GameManager.player = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            StopAllCoroutines();
            DestroyImmediate(gameObject);
        }
    }

    private void Start ()
    {
        m_stateM.State = new PlayerWalkingState(this);
        StartCoroutine(m_stateM.State.EnterState(null));
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
    #endregion
}