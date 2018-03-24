using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform SecondHand;
    public Transform projectileSpawn;
    public Vector3 GunOffset;

    public virtual void Shoot()
    {
        Instantiate(projectilePrefab, projectileSpawn.position, projectileSpawn.rotation);
    }
}
