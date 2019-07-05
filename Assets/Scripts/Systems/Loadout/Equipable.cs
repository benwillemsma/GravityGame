using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipable : MonoBehaviour
{
    public Player Owner { get; private set; }

    public virtual void OnEquip(Player equipedBy)
    {
        Owner = equipedBy;
    }
    public virtual void OnUnequip(Player equipedBy)
    {
        Owner = null;
    }
}
