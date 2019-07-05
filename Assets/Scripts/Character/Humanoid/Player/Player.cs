using System;
using System.Collections;
using UnityEngine;

public class Player : HumanoidData
{
    [Header("Movement")]
    [Range(0, 10)]
    public float RunSpeed = 8;
    [Range(4, 10)]
    public float JumpForce = 6;
    [Range(0, 10)]
    public float AirControl= 5;
    public bool toggleCrouch;
    public LayerMask groundMask;

    [Header("Camera")]
    [Range(0, 50)]
    public float CameraSensitivity = 30;
    [Range(0, 2)]
    public float CameraOffset = 0.75f;
    public bool InvertedCamera;

    [Header("Hud References")]
    public Canvas Hud;

    [Header("Loadout References (Don't Touch)")]
    public LoadoutData loadoutRef;
    public Transform GunPivot;
    public Loadout loadout { get; private set; }

    public Action currentAction { get; private set; }
    private bool doingAction;

    protected override void Awake()
    {
        base.Awake();
        if (!GameManager.player)
            GameManager.player = this;
        else
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    private void Start ()
    {
        InitLoadout(loadoutRef);
        m_stateM.State = new PlayerWalkingState(this);
        m_stateM.State.EnterState(null);
    }

    public void InitLoadout(LoadoutData loadoutRef)
    {
        Loadout newLoadout = new Loadout();

        if (loadoutRef.gun)
        {
            Gun gun = Instantiate(loadoutRef.gun);
            Equip(gun, Anim.GetBoneTransform(HumanBodyBones.RightHand));
            newLoadout.gun = gun;
        }

        if (loadoutRef.grenade)
        {
            Grenade grenade = Instantiate(loadoutRef.grenade);
            Equip(grenade, Vector3.zero, Quaternion.identity);
            newLoadout.grenade = grenade;
        }

        if (loadoutRef.gadget)
        {
            Gadget gadget = Instantiate(loadoutRef.gadget);
            Equip(gadget, Vector3.zero, Quaternion.identity);
            newLoadout.gadget = gadget;
        }
        loadout = newLoadout;
    }
    private void Equip(Equipable equipable,Transform transform)
    {
        equipable.transform.parent = transform;
        equipable.transform.localPosition = Vector3.zero;
        equipable.transform.localRotation = Quaternion.identity;
        equipable.OnEquip(this);
    }
    private void Equip(Equipable equipable, Vector3 positionOffset, Quaternion rotationOffset)
    {
        equipable.transform.localPosition = positionOffset;
        equipable.transform.localRotation = rotationOffset;
        equipable.OnEquip(this);
    }

    public void ToggleHud()
    {
        Hud.gameObject.SetActive(!Hud.gameObject.activeSelf);
    }
    public void ToggleHud(bool visible)
    {
        Hud.gameObject.SetActive(visible);
    }

    // Actions
    public void NewAction(Action action)
    {
        if(currentAction != null)
        {
            if (!currentAction.interruptable)
                currentAction = action;
        }
        else
        {
            currentAction = action;
            StartCoroutine(DoAction());
        }
    }

    private IEnumerator DoAction()
    {
        doingAction = true;
        while (currentAction != null && currentAction.routine.MoveNext())
        {
            yield return null;
        }
        doingAction = false;
        currentAction = null;
    }

    public void StopAction()
    {
        currentAction = null;
        doingAction = false;
    }
}

public class Action
{
    public IEnumerator routine;
    public bool interruptable;
    public bool blockInput;
    public bool controlMovement;

    public Action(IEnumerator routine, bool interruptable, bool blockInput, bool controlMovement)
    {
        this.routine = routine;
        this.interruptable = interruptable;
        this.blockInput = blockInput;
        this.controlMovement = controlMovement;
    }
}

public struct Loadout
{
    public Gun gun;
    public Gadget gadget;
    public Grenade grenade;

    public Loadout(Gun gun, Gadget gadget, Grenade grenade)
    {
        this.gun = gun;
        this.gadget = gadget;
        this.grenade = grenade;
    }
}
