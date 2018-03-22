using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState : PlayerState<Player>
{
    public PlayerWalkingState(Player player) : base(player) { }
    private Vector3 moveDirection;
    private bool Crouched;
    private bool Sprinting;

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

    //State Updates
    protected override void UpdateState()
    {
        UpdateMovement();
        UpdateAnimator();

        if (!grounded && Input.GetButtonDown("GravChange")) ChangeGravity();
    }

    protected override void UpdateMovement()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            if (data.toggleCrouch)
                Crouched = !Crouched;
            else
            {
                Crouched = true;
                Sprinting = false;
            }
        }
        else if (Input.GetButtonUp("Crouch") && !data.toggleCrouch)
            Crouched = false;

        Sprinting = Input.GetButton("Sprint");
        if (Sprinting) Crouched = false;

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        if (Crouched) moveDirection /= 2;
        else if (Sprinting) moveDirection *= 1.5f;

        UpdateRotation();
    }
    private void UpdateRotation()
    {
        Vector3 newForward = Vector3.ProjectOnPlane(rb.transform.forward, -gravityDirection);
        Quaternion desiredRotation = Quaternion.LookRotation(newForward, -gravityDirection);
        rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, desiredRotation, Time.deltaTime * 6);
        rb.transform.Rotate(rb.transform.up, Input.GetAxis("Mouse X") * Time.deltaTime * 50, Space.World);
    }
    
    protected override void UpdateAnimator()
    {
        anim.SetBool("Crouching", Crouched);
        anim.SetFloat("MoveX", moveDirection.x);
        anim.SetFloat("MoveY", moveDirection.z);
        anim.SetFloat("Speed", moveDirection.magnitude);
    }
    protected override void UpdatePhysics()
    {
        GroundCheck();
        if (grounded)
        {
            rb.velocity = rb.transform.rotation * moveDirection;
            rb.velocity = Vector3.ProjectOnPlane(rb.velocity, gravityDirection) * data.RunSpeed;
            if (Input.GetButtonDown("Jump")) Jump();
        }
        else anim.ResetTrigger("Jump");
        rb.AddForce(gravityDirection * gravityStrength * rb.mass * (grounded ? 20 : 1));
    }

    //Trigger Functions
    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }

    //Colission Functions
    public override void OnCollisionEnter(Collision collision) { }
    public override void OnCollisionStay(Collision collision) { }
    public override void OnCollisionExit(Collision collision) { }

    private void Jump()
    {
        rb.velocity = Vector3.ProjectOnPlane(rb.velocity, rb.transform.up);
        rb.velocity += rb.transform.up * data.JumpForce;
        grounded = false;
        anim.SetTrigger("Jump");
    }

    private void ChangeGravity()
    {
        if (moveDirection.magnitude > 0.2f)
        {
            if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.z))
            {
                if (moveDirection.x > 0)
                    gravityDirection = rb.transform.right;
                else if (moveDirection.x < 0)
                    gravityDirection = -rb.transform.right;
            }
            else
                if (moveDirection.z > 0)
                gravityDirection = rb.transform.forward;
            else if (moveDirection.z < 0)
                gravityDirection = -rb.transform.forward;
        }
        else
            gravityDirection = rb.transform.up;
    }
}
