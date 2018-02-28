using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState<Data> : CharacterState<Data> where Data : Player
{
    #region PlayerState Variables

    protected static Vector3 gravityDirection = Vector3.down;
    protected static float gravityStrength = 9.8f;
    protected static bool grounded = true;

    #endregion

    public PlayerState(Data playerData) : base(playerData) { }

    protected void GroundCheck()
    {
        Vector3 start = rb.transform.position + rb.transform.up;
        Vector3 direction = -rb.transform.up;

        RaycastHit hit;
        Debug.DrawRay(start, direction * 1.5f, Color.red, 0.1f);
        if (Physics.Raycast(start, direction, out hit, 1.5f))
        {
            grounded = true;
            gravityDirection = -hit.normal;
        }
        else grounded = false;
    }
}
