﻿using UnityEngine;
using System.Collections;

public class Shockwave : Melee
{
    private const int SPHERECAST_Y_MODIFIER = 50;

    public override void PlayVFX()
    {

    }

    public override string GetName()
    {
        return this.ToString();
    }

    // Use this for initialization
    void Start()
    {
        coolDownDuration = WeaponEngineValues.SMITE_COOLDOWN_DURATION;
    }

    protected override void Attack()
    {
        if (onCooldown)
            return;

        GameObject[] targets = new GameObject[WeaponEngineValues.MAX_PLAYERS];
        if (FindNearbyTargets(ref targets))
        {
            foreach (GameObject curTgrt in targets)
            {
                if (curTgrt != null)
                {
                    WeaponEngine curEngine = curTgrt.GetComponent<WeaponEngine>();

                    if (curEngine != null)
                        curEngine.ModifyHealth(-WeaponEngineValues.SHOCKWAVE_DAMAGE);

                    ApplyShockForce(curTgrt.transform);
                }
            }
        }
        PlayVFX();
        StartCooldown();
    }

    private void ApplyShockForce(Transform target)
    {
        float percentApplied = (1 - Mathf.Clamp01(Vector3.Distance(transform.position, target.position) / WeaponEngineValues.SHOCKWAVE_RADIUS));

        Vector3 dir = ((target.position - transform.position).normalized) * WeaponEngineValues.SHOCKWAVE_PUSH_MAX_FORCE;
        dir = Vector3.ClampMagnitude(dir, Mathf.Clamp(WeaponEngineValues.SHOCKWAVE_PUSH_MAX_FORCE * percentApplied, 0, WeaponEngineValues.SHOCKWAVE_PUSH_MAX_FORCE));

        target.rigidbody.AddForce(dir, ForceMode.VelocityChange);
    }

    private bool FindNearbyTargets(ref GameObject[] nearbyDinos)
    {
        bool hitTarget = false;
        Ray thisRay = new Ray(new Vector3(transform.position.x, transform.position.y + SPHERECAST_Y_MODIFIER, transform.position.z), Vector3.down);

        // display splash damage radius in scene
        Debug.DrawLine(new Vector3(transform.position.x, transform.position.y, transform.position.z + WeaponEngineValues.SHOCKWAVE_RADIUS), transform.position);
        Debug.DrawLine(thisRay.origin, transform.position);

        RaycastHit[] hits;
        hits = Physics.SphereCastAll(thisRay, WeaponEngineValues.SHOCKWAVE_RADIUS, SPHERECAST_Y_MODIFIER);

        if (hits.Length != 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if ((hits[i].collider.tag == "Dino" || hits[i].collider.tag == "Ai") && hits[i].collider.gameObject != gameObject)
                {
                    bool assigned = false;
                    for (int j = 0; j < nearbyDinos.Length; j++)
                    {
                        if (hits[i].collider.gameObject == nearbyDinos[j])
                        {
                            assigned = true;
                        }

                        if (nearbyDinos[j] == null && assigned == false)
                        {
                            nearbyDinos[j] = hits[i].collider.gameObject;
                            assigned = true;
                            hitTarget = true;
                        }
                    }
                }
            }
            return hitTarget;
        }
        else
        {
            return false;
        }

    }
}
