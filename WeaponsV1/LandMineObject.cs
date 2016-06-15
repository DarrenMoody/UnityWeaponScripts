using UnityEngine;
using System.Collections;

public class LandMineObject : BombObject
{

    protected override void PlayVFX()
    {
        // Consider moving SetActive to after co-routine if there is the object needs to persist
        Disable();
    }

    // Insert code that will excute when the bomb is activated


    void Start()
    {
        neighborRadius = WeaponEngineValues.LANDMINE_SPLASH_RADIUS;
        myCollider = transform.GetComponent<SphereCollider>();
    }

    public override void MisFire()
    {
        PlayVFX();
    }


    // Fire
    // Will Fire the bomb, and deal splash damage to all nearbyDinos
    protected override void Fire()
    {
        setActivated(false);
        // initialize targets
        GameObject[] curTargets = new GameObject[WeaponEngineValues.MAX_PLAYERS];

        // find nearbyDinos
        FindNearbyTargets(ref curTargets);


        // target weapon engines for each target
        for (int i = 0; i < curTargets.Length; i++)
        {
            if (curTargets[i] != null)
            {
                WeaponEngine trgt = curTargets[i].GetComponent<WeaponEngine>();
                if (trgt != null)

                    // modify health
                    trgt.ModifyHealth((int)-(calcSplashDamage(curTargets[i].transform.position) * WeaponEngineValues.LANDMINE_MAX_EXPLOSION_DAMAGE));
            }
        }

        // play vfx and disable prefab
        PlayVFX();

    }

    // calcSplashDamge
    // will calculate the linear falloff damage to the current target dependant on how close they are to the bomb when it detonates
    // closer means more damage applied
    // returns percent of damage applied as a float
    // para: the position of the character
    float calcSplashDamage(Vector3 target)
    {
        float extents = 0;

        if (myCollider.GetType() == typeof(SphereCollider))
            extents = ((SphereCollider)myCollider).radius;

        if (collider.GetType() == typeof(BoxCollider))
            extents = myCollider.bounds.extents.x;

        float dist = Vector3.Distance(transform.position, target);
        float percentApplied = (1 - Mathf.Clamp01(((dist - (extents * transform.localScale.x)) / WeaponEngineValues.LANDMINE_SPLASH_RADIUS)));

        return percentApplied;
    }

    void OnTriggerEnter(Collider col)
    {
        if ((col.CompareTag("Dino") || col.CompareTag("Ai")) && isActivated())
        {
            Fire();
        }
    }
}
