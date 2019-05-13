using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState : PlayerState<Player>
{
    public PlayerWalkingState(Player player) : base(player) { }
    private Vector3 moveDirection;
    private bool crouched;
    private bool sprinting;
    private bool aiming;

    public override IEnumerator EnterState(BaseState prevState)
    {
        gravityDirection = -rb.transform.up;
        gravityStrength = 9.8f;

        yield return base.EnterState(prevState);
    }
    public override IEnumerator ExitState(BaseState nextState)
    {
        yield return base.ExitState(nextState);
    }

    // State Updates
    protected override void UpdateState()
    {
        UpdateMovement();
        UpdateAnimator();
        UpdateIK();
    }

    protected override void UpdateMovement()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            if (data.toggleCrouch)
                crouched = !crouched;
            else
            {
                crouched = true;
                sprinting = false;
            }
        }
        else if (Input.GetButtonUp("Crouch") && !data.toggleCrouch)
            crouched = false;

        aiming = Input.GetButton("Aiming");
        sprinting = Input.GetButton("Sprint") && !aiming;
        if (sprinting) crouched = false;

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (moveDirection.magnitude > 1) moveDirection = moveDirection.normalized;

        if (aiming) moveDirection /= 1.5f;
        else if (crouched) moveDirection /= 2;
        else if (sprinting) moveDirection *= 1.5f;

        UpdateRotation();
    }
    private void UpdateRotation()
    {
        Vector3 newForward = Vector3.ProjectOnPlane(rb.transform.forward + rb.transform.up * 0.001f, -gravityDirection).normalized;
        Quaternion desiredRotation = Quaternion.LookRotation(newForward, -gravityDirection);
        rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, desiredRotation, Time.deltaTime * 6);
        rb.transform.Rotate(rb.transform.up, Input.GetAxis("Mouse X") * Time.deltaTime * data.CameraSensitivity, Space.World);
    }

    protected override void UpdatePhysics()
    {
        GroundCheck();

        if (Input.GetButtonDown("GravChange") && canChangeGravity) data.StartCoroutine(ChangeGravity());
        if (grounded)
        {
            rb.velocity = rb.transform.rotation * moveDirection;
            rb.velocity = Vector3.ProjectOnPlane(rb.velocity, gravityDirection) * data.RunSpeed;
            if (Input.GetButtonDown("Jump")) Jump();
        }
        else
        {
            anim.ResetTrigger("Jump");
            rb.position += rb.transform.rotation * moveDirection * 0.01f * data.AirControl;
        }
        rb.AddForce(gravityDirection * gravityStrength * rb.mass * (grounded ? 20 : 1));
    }

    protected override void UpdateAnimator()
    {
        anim.SetBool("Crouching", crouched);
        anim.SetBool("Aiming", aiming);
        anim.SetFloat("MoveX", moveDirection.x);
        anim.SetFloat("MoveY", moveDirection.z);
        anim.SetFloat("Speed", moveDirection.magnitude);
    }

    protected override void UpdateIK()
    {
        if (data.EquipedGun)
        {
            if (aiming) IK.RightHand.weight = Mathf.Lerp(IK.RightHand.weight, 1f, Time.deltaTime * 10); 
            else IK.RightHand.weight = Mathf.Lerp(IK.RightHand.weight, 0, Time.deltaTime * 15);
            IK.RightHand.position = data.GunPivot.position + IK.mainCamera.rotation * data.EquipedGun.GunOffset;
            IK.RightHand.rotation = Quaternion.LookRotation(IK.mainCamera.forward, IK.mainCamera.right);

            IK.LookWeight = IK.RightHand.weight;
            IK.HeadWeight = IK.RightHand.weight / 2;

            if (data.EquipedGun.SecondHand)
            {
                IK.LeftHand.weight = 1;
                IK.LeftHand.position = data.EquipedGun.SecondHand.position;
                IK.LeftHand.rotation = data.EquipedGun.SecondHand.rotation;
            }
            else IK.LeftHand.weight = 0;
        }
    }

    // Trigger Functions
    public override void OnTriggerEnter(Collider collider)
    {
        if (crouched)
        {
            Cover coverObject = collider.gameObject.GetComponent<Cover>();
            if (coverObject) stateManager.ChangeState(new PlayerCoverState(data, coverObject));
        }
    }
    public override void OnTriggerStay(Collider collider)
    {
        if (Input.GetButtonDown("Crouch"))
        {
            Cover coverObject = collider.gameObject.GetComponent<Cover>();
            if (coverObject) stateManager.ChangeState(new PlayerCoverState(data, coverObject));
        }
    }
    public override void OnTriggerExit(Collider collider) { }

    // Colission Functions
    public override void OnCollisionEnter(Collision collision) { }
    public override void OnCollisionStay(Collision collision) { }
    public override void OnCollisionExit(Collision collision) { }

    // State Functions
    private void Jump()
    {
        rb.velocity = Vector3.ProjectOnPlane(rb.velocity, rb.transform.up);
        rb.velocity += rb.transform.up * data.JumpForce;
        grounded = false;
        anim.SetTrigger("Jump");
    }

    private IEnumerator ChangeGravity()
    {
        canChangeGravity = false;
        if (grounded)
        {
            Jump();
            yield return new WaitForSeconds(0.2f);
        }

        Vector3 newGravity = gravityDirection;
        if (moveDirection.magnitude > 0.2f)
        {
            if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.z))
            {
                if (moveDirection.x > 0)
                    newGravity = rb.transform.right;
                else if (moveDirection.x < 0)
                    newGravity = -rb.transform.right;
            }
            else
                if (moveDirection.z > 0)
                newGravity = rb.transform.forward;
            else if (moveDirection.z < 0)
                newGravity = -rb.transform.forward;
        }
        else newGravity = rb.transform.up;

        if (newGravity != gravityDirection)
        {
            float dampenForce = Vector3.Project(rb.velocity, gravityDirection).magnitude;
            data.StartCoroutine(DampenVelocity(-gravityDirection, dampenForce, 0.5f));
            gravityDirection = newGravity;
        }
        yield return new WaitForSeconds(0.2f);
        canChangeGravity = true;
    }

    private IEnumerator DampenVelocity(Vector3 dampenDirection, float force, float duration)
    {
        float time = duration;
        while (time > 0)
        {
            rb.AddForce(dampenDirection * force);
            time -= Time.deltaTime;
            yield return null;
        }
    }
}
