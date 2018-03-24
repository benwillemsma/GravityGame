using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HumanoidState<Data> : CharacterState<Data> where Data : HumanoidData
{
    protected IKController IK;

    public HumanoidState(Data playerData) : base(playerData)
    {
        IK = data.IK;
    }

    protected abstract void UpdateIK();
}
