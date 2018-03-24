using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class IKController : MonoBehaviour
{
    protected Animator anim;
    public Transform mainCamera;

    protected IKTarget _RightHand = new IKTarget();
    protected IKTarget _RightFoot = new IKTarget();
    protected IKTarget _LeftHand = new IKTarget();
    protected IKTarget _LeftFoot = new IKTarget();

    protected Vector3 _lookAtPosition;
    protected float _headWeight = 0;
    protected float _lookWeight = 0;
    protected float _headTurnSpeed = 1;

    protected Vector3 _RightElbow;
    protected Vector3 _LeftElbow;
    protected Vector3 _RightKnee;
    protected Vector3 _LeftKnee;

    protected Vector3 CameraForward;

    //Unity Functions
    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        mainCamera = Camera.main.transform;
    }
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(RightHand.position, Vector3.one* 0.1f);
        Gizmos.DrawCube(LeftHand.position, Vector3.one * 0.1f);
        Gizmos.DrawCube(RightFoot.position, Vector3.one * 0.1f);
        Gizmos.DrawCube(LeftFoot.position, Vector3.one * 0.1f);
    }
    protected virtual void Update()
    {
        if (_RightHand.weight == 0 && _LeftHand.weight == 0 && _RightFoot.weight == 0 && _LeftFoot.weight == 0)
            SetBoneTransforms();

        _lookAtPosition = mainCamera.position + mainCamera.forward * 50;
    }
    protected virtual void OnAnimatorIK(int layerIndex)
    {
        _RightElbow = transform.position + transform.up + transform.right * 5.0f;
        _LeftElbow = transform.position + transform.up - transform.right * 5.0f;
        _RightKnee = transform.position + transform.up * 5f + transform.forward * 2f + transform.right * 2f;
        _LeftKnee = transform.position + transform.up * 5f + transform.forward * 2f - transform.right * 2f;

        anim.SetLookAtWeight(_lookWeight, _headWeight / 2, _headWeight, 0, 1);
        anim.SetLookAtPosition(_lookAtPosition);

        //  --Arm IK--
        //RightHand
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, _RightHand.weight);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, _RightHand.weight);
        anim.SetIKPosition(AvatarIKGoal.RightHand, _RightHand.position);
        anim.SetIKRotation(AvatarIKGoal.RightHand, _RightHand.rotation);

        //LeftHand
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, _LeftHand.weight);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, _LeftHand.weight);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, _LeftHand.position);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, _LeftHand.rotation);

        //RightElbow
        anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, _RightHand.weight);
        anim.SetIKHintPosition(AvatarIKHint.RightElbow, _RightElbow);

        //LeftElbow
        anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, _LeftHand.weight);
        anim.SetIKHintPosition(AvatarIKHint.LeftElbow, _LeftElbow);

        //  --Leg IK--
        //RightFoot
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, _RightFoot.weight);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, _RightFoot.weight);
        anim.SetIKPosition(AvatarIKGoal.RightFoot, _RightFoot.position);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, _RightFoot.rotation);

        //LeftFoot
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, _LeftFoot.weight);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _LeftFoot.weight);
        anim.SetIKPosition(AvatarIKGoal.LeftFoot, _LeftFoot.position);
        anim.SetIKRotation(AvatarIKGoal.LeftFoot, _LeftFoot.rotation);

        //RightKnee
        anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, _RightFoot.weight);
        anim.SetIKHintPosition(AvatarIKHint.RightKnee, _RightKnee);

        //LeftKnee
        anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, _LeftFoot.weight);
        anim.SetIKHintPosition(AvatarIKHint.LeftKnee, _LeftKnee);
    }

    //Initial SetupFunctions
    public void SetBoneTransforms()
    {
        _RightHand.Set(anim.GetBoneTransform(HumanBodyBones.RightHand));
        _LeftHand.Set(anim.GetBoneTransform(HumanBodyBones.LeftHand));
        _RightFoot.Set(anim.GetBoneTransform(HumanBodyBones.RightFoot));
        _LeftFoot.Set(anim.GetBoneTransform(HumanBodyBones.LeftFoot));
    }
    public void SetIKPositions(Transform rightH, Transform leftH, Transform rightF, Transform leftF)
    {
        if (rightH)
            _RightHand.Set(rightH);

        if (leftH)
            _LeftHand.Set(leftH);

        if (rightF)
            _RightFoot.Set(rightF);

        if (leftF)
            _LeftFoot.Set(leftF);
    }

    //MoveIK
    public static void MoveIKTarget(IKTarget Target, Transform NewTarget, float delta, float EndWeight, float weightDelta)
    {
        Target.position = Vector3.MoveTowards(Target.position, NewTarget.position, delta);
        Target.rotation = Quaternion.Lerp(Target.rotation, NewTarget.rotation, delta * 4);
        Target.weight = Mathf.MoveTowards(Target.weight, EndWeight, weightDelta);
    }
    public static void MoveIKTarget(IKTarget Target, Transform NewTarget, float delta)
    {
        Target.position = Vector3.MoveTowards(Target.position, NewTarget.position, delta);
        Target.rotation = Quaternion.Lerp(Target.rotation, NewTarget.rotation, delta * 4);
    }

    //LerpIK
    public static void LerpIKTarget(IKTarget Target, IKTarget fromTarget, IKTarget toTarget,float delta, float fromWeight, float toWeight)
    {
        Target.position = Vector3.Lerp(fromTarget.position, toTarget.position, delta);
        Target.rotation = Quaternion.Lerp(fromTarget.rotation, toTarget.rotation, delta);
        Target.weight = Mathf.Lerp(fromWeight, toWeight, delta);
    }
    public static void LerpIKTarget(IKTarget Target, IKTarget fromTarget, IKTarget toTarget, float delta)
    {
        Target.position = Vector3.Lerp(fromTarget.position, toTarget.position, delta);
        Target.rotation = Quaternion.Lerp(fromTarget.rotation, toTarget.rotation, delta);
    }

    // ---------------Getters/Setters---------------
    //Get/Set Look Position
    public Vector3 LookAtPosition
    {
        set { _lookAtPosition = value; }
        get { return _lookAtPosition; }
    }
    
    //Get Hand & Foot
    public IKTarget RightHand
    {
        get {return _RightHand; }
    }
    public IKTarget LeftHand
    {
        get { return _LeftHand; }
    }
    public IKTarget RightFoot
    {
        get { return _RightFoot; }
    }
    public IKTarget LeftFoot
    {
        get { return _LeftFoot; }
    }
   
    //Get/Set Elbow & Knee
    public Vector3 RightElbow
    {
        set { _RightElbow = value; }
        get { return _RightElbow; }
    }
    public Vector3 LeftElbow
    {
        set { _LeftElbow = value; }
        get { return _LeftElbow; }
    }
    public Vector3 RightKnee
    {
        set { _RightKnee = value; }
        get { return _RightKnee; }
    }
    public Vector3 LeftKnee
    {
        set { _LeftKnee = value; }
        get { return _LeftKnee; }
    }

    //Get/set IKWeights
    public float GlobalWeight
    {
        set
        {
            _RightHand.weight = value;
            _LeftHand.weight = value;
            _RightFoot.weight = value;
            _LeftFoot.weight = value;
        }
    }
    public float HeadWeight
    {
        get { return _headWeight; }
        set { _headWeight = value; }
    }
    public float LookWeight
    {
        
        get { return _lookWeight; }
        set { _lookWeight = value; }
    }
}
