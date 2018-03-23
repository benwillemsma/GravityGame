﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : CharacterData
{
    #region Public Variables
    [Header("Movement"), Space(10)]
    [Range(0, 10)]
    public float RunSpeed = 8;
    [Range(4, 10)]
    public float JumpForce = 6;
    [Range(0, 10)]
    public float AirControl= 5;
    public bool toggleCrouch;

    [Header("Camera"), Space(10)]
    [Range(0, 50)]
    public float CameraSensitivity = 30;
    [Range(0, 2)]
    public float CameraOffset = 0.75f;
    public bool InvertedCamera;

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