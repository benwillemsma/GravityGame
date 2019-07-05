using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    public CollisionDelegate collisionEnter;
    public TriggerDelegate triggerEnter;

    private void OnCollisionEnter(Collision collision)
    {
        collisionEnter.Invoke(collision);
    }
    private void OnTriggerEnter(Collider other)
    {
        triggerEnter.Invoke(other);
    }
}
