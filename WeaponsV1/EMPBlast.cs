using UnityEngine;
using System.Collections;

public class EMPBlast : Bomb
{

    private const int SPHERECAST_Y_MODIFIER = 50;

    protected override void PlayFX()
    {

    }

    public override string GetName()
    {
        return "EMPBlast";
    }

    protected override void LaunchBomb()
    {
        GameObject[] targets = new GameObject[WeaponEngineValues.MAX_PLAYERS];
        if (FindNearbyTargets(ref targets))
        {
            foreach (GameObject curTgrt in targets)
            {
                if (curTgrt != null)
                {
                    WeaponEngine curEngine = curTgrt.GetComponent<WeaponEngine>();

                    if (curEngine != null)
                    {
                        curEngine.ModifyHealth(-WeaponEngineValues.EMPBLAST_DAMAGE);
                        EMPBlastFX effect = curEngine.GetComponent<EMPBlastFX>();

                        if (effect != null)
                            effect.ResetTimer();
                        else
                            curTgrt.AddComponent<EMPBlastFX>();
                    }
                }
            }
        }
    }

    private bool FindNearbyTargets(ref GameObject[] nearbyDinos)
    {
        bool hitTarget = false;
        // Create spherecast ray
        Ray thisRay = new Ray(new Vector3(transform.position.x, transform.position.y + SPHERECAST_Y_MODIFIER, transform.position.z), Vector3.down);

        // Display splash damage radius in scene mode
        Debug.DrawLine(new Vector3(transform.position.x, transform.position.y, transform.position.z + WeaponEngineValues.EMPBLAST_RADIUS), transform.position);
        Debug.DrawLine(thisRay.origin, transform.position);


        // Find nearbyDinos using a spherecast
        RaycastHit[] hits;
        hits = Physics.SphereCastAll(thisRay, WeaponEngineValues.EMPBLAST_RADIUS, SPHERECAST_Y_MODIFIER);

        // Iterate through RaycastHits to save valid nearbyDinos to array, ensure characters are not listed twice
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