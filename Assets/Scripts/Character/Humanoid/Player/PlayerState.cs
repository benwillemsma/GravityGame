﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState<Data> : HumanoidState<Data> where Data : Player
{
    #region PlayerState Variables

    protected Vector3 gravityDirection;
    protected float gravityStrength;
    protected bool grounded;
    protected bool canChangeGravity = true;

    #endregion

    public PlayerState(Data playerData) : base(playerData) { }

    protected void GroundCheck()
    {
        Vector3 start = rb.transform.position + rb.transform.up + rb.transform.forward * 0.2f;
        Vector3 direction = -rb.transform.up;

        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, 1.25f, ~data.groundMask))
        {
            if (hit.distance < 1.05f) grounded = true;
            if (canChangeGravity) gravityDirection = -hit.normal;
            anim.SetBool("Grounded", true);
        }
        else
        {
            anim.SetBool("Grounded", false);
            grounded = false;
        }
    }
}
