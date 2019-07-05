using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCoverState : PlayerState<Player>
{
    public PlayerCoverState(Player player, Cover startCover) : base(player)
    { currentCover = startCover; }

    private Cover currentCover;
    private Vector3 moveDirection;
    private bool facingRight = true;
    private bool aiming;

    public override void EnterState(BaseState prevState)
    {
        anim.SetBool("InCover", true);
        base.EnterState(prevState);
    }
    public override void ExitState(BaseState nextState)
    {
        anim.SetBool("InCover", false);
        base.ExitState(nextState);
    }

    //State Updates
    protected override void UpdateState()
    {
        base.UpdateState();
        if (!grounded || Input.GetButtonDown("Crouch"))
            stateManager.ChangeState(new PlayerWalkingState(data));
    }

    protected override void UpdateMovement()
    {
        aiming = Input.GetButton("Aiming");

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        if (aiming) moveDirection *= 0;
        else if (moveDirection.magnitude > 1) moveDirection = moveDirection.normalized;

        UpdateRotation();
    }
    private void UpdateRotation()
    {
        Vector3 newForward = Vector3.ProjectOnPlane(currentCover.transform.forward + rb.transform.up * 0.001f, -gravityDirection).normalized;
        Quaternion desiredRotation = Quaternion.LookRotation(newForward, -gravityDirection);
        rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, desiredRotation, Time.deltaTime * 6);
        rb.transform.Rotate(rb.transform.up, Input.GetAxis("Mouse X") * Time.deltaTime * data.CameraSensitivity, Space.World);
    }

    protected override void UpdatePhysics()
    {
        GroundCheck();
        if (grounded)
        {
            rb.velocity = rb.transform.rotation * moveDirection + rb.transform.forward;
            rb.velocity = Vector3.ProjectOnPlane(rb.velocity, gravityDirection) * data.RunSpeed / 2;
        }
        rb.AddForce(gravityDirection * gravityStrength * rb.mass * (grounded ? 20 : 1));
    }

    protected override void UpdateAnimator()
    {
        anim.SetBool("Crouching", true);
        anim.SetBool("Aiming", aiming);
        anim.SetFloat("MoveX", moveDirection.x);
        anim.SetFloat("MoveY", 0);
        anim.SetFloat("Speed", moveDirection.magnitude);
    }

    protected override void UpdateIK()
    {
        if (data.loadout.gun)
        {
            if (aiming) IK.RightHand.weight = Mathf.Lerp(IK.RightHand.weight, 1f, Time.deltaTime * 10); 
            else IK.RightHand.weight = Mathf.Lerp(IK.RightHand.weight, 0, Time.deltaTime * 15);
            IK.RightHand.position = data.GunPivot.position + IK.mainCamera.rotation * data.loadout.gun.gunOffset;
            IK.RightHand.rotation = Quaternion.LookRotation(IK.mainCamera.forward, IK.mainCamera.right);

            IK.LookWeight = IK.RightHand.weight;
            IK.HeadWeight = IK.RightHand.weight / 2;

            if (data.loadout.gun.secondHand)
            {
                IK.LeftHand.weight = 1;
                IK.LeftHand.position = data.loadout.gun.secondHand.position;
                IK.LeftHand.rotation = data.loadout.gun.secondHand.rotation;
            }
            else IK.LeftHand.weight = 0;
        }
    }

    //Trigger Functions
    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }

    //Colission Functions
    public override void OnCollisionEnter(Collision collision) { }
    public override void OnCollisionStay(Collision collision) { }
    public override void OnCollisionExit(Collision collision) { }
}
