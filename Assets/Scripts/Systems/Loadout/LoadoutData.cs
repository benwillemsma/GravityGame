using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Loadout", menuName = "ScriptableObjects/Loadout")]
public class LoadoutData : ScriptableObject
{
    public Gun gun;
    public Gadget gadget;
    public Grenade grenade;
}
