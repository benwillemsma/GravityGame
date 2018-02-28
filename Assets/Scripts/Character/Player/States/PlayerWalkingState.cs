using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState : PlayerState<Player>
{
    public PlayerWalkingState(Player player) : base(player) { }
    Vector3 moveDirection;

    public override IEnumerator EnterState(BaseState prevState)
    {
        yield return base.EnterState(prevState);
    }
    public override IEnumerator ExitState(BaseState nextState)
    {
        yield return base.ExitState(nextState);
    }

    //State Updates
    protected override IEnumerator UpdateState()
    {
        UpdateMovement();
        UpdateAnimator();

        if (Input.GetButtonDown("Jump")) Jump();
        else if (!grounded && Input.GetButtonDown("GravChange")) ChangeGravity();

        yield return null;
    }
    protected override IEnumerator UpdatePaused()
    {
        yield return null;
    }
    protected override IEnumerator UpdateTransition()
    {
        yield return null;
    }

    protected override void UpdateMovement()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        UpdateRotation();
    }
    private void UpdateRotation()
    {
        Vector3 newForward = Vector3.ProjectOnPlane(rb.transform.forward, -gravityDirection);
        Quaternion desiredRotation = Quaternion.LookRotation(newForward, -gravityDirection);
        rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, desiredRotation, Time.deltaTime * 8);
        rb.transform.Rotate(rb.transform.up, Input.GetAxis("Mouse X") * Time.deltaTime * 20, Space.World);
    }

    protected override void UpdateAnimator()
    {

    }
    protected override void UpdatePhysics()
    {
        GroundCheck();
        if (grounded) rb.velocity = (rb.transform.rotation * (moveDirection.normalized * data.MaxSpeed)) + Vector3.Project(rb.velocity, gravityDirection);
        rb.AddForce(gravityDirection * gravityStrength * rb.mass);
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
        rb.AddForce(rb.transform.up * 10 * rb.mass, ForceMode.Impulse);
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
