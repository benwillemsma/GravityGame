using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityGrapple : Gun_Laser
{
    public override void Shoot()
    {
        Ray ray = new Ray(ammoSpawn.position, ammoSpawn.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200))
        {
            Owner.NewAction(new Action(Grapple(hit.point), true, false, true));
        }
    }

    public IEnumerator Grapple(Vector3 GrappleTarget)
    {
        while ((Owner.transform.position - GrappleTarget).magnitude > 2)
        {
            Owner.RB.velocity = (GrappleTarget - Owner.transform.position).normalized * 20;
            yield return null;
        }
    }
}
