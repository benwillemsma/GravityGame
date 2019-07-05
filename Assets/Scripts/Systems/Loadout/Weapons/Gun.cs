using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Equipable
{
    public GameObject ammoPrefab;
    public Transform secondHand;
    public Transform ammoSpawn;
    public Vector3 gunOffset;

    public virtual void Shoot()
    {
    }
}
