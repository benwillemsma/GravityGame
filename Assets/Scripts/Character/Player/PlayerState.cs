using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState<Data> : CharacterState<Data> where Data : Player
{
    #region PlayerState Variables

    protected Vector3 gravityDirection;
    protected float gravityStrength;
    protected bool grounded;

    #endregion

    public PlayerState(Data playerData) : base(playerData) { }

    protected void GroundCheck()
    {
        Vector3 start = rb.transform.position + rb.transform.up + rb.transform.forward * 0.2f;
        Vector3 direction = -rb.transform.up;

        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, 1.5f))
        {
            if (hit.distance < 1f) grounded = true;
            gravityDirection = -hit.normal;
        }
        else grounded = false;
    }
}
