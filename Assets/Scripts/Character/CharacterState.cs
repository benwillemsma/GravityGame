using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterState<Data> : BaseState where Data : CharacterData
{
    protected Data data;
    protected Rigidbody rb;
    protected Animator anim;

    public CharacterState(Data characterData) : base(characterData.StateM)
    {
        data = characterData;
        rb = characterData.RB;
        anim = characterData.Anim;
    }

    //State Updates
    protected override IEnumerator UpdateState() { yield return null; }
    protected override IEnumerator UpdatePaused() { yield return null; }
    protected override IEnumerator UpdateTransition() { yield return null; }

    protected abstract void UpdateMovement();
    protected abstract void UpdateAnimator();
    protected override void UpdatePhysics() { }
}
