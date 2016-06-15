using UnityEngine;
using System.Collections;

public class AcidSpitObject : BombObject
{
    GameObject[] targets;
    const int ONE_HUNDRED = 100;

    protected override void PlayVFX()
    {
        renderer.enabled = false;
    }


    protected void EnabledFX()
    {

    }

    void Start()
    {
        targets = new GameObject[WeaponEngineValues.MAX_PLAYERS];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = null;
        }

        myCollider = transform.GetComponent<SphereCollider>();
    }

    public override void MisFire()
    {
        PlayVFX();
    }

    protected override void Fire()
    {

    }

    IEnumerator ClearGameObject(int e)
    {
        yield return new WaitForSeconds(WeaponEngineValues.ACIDSPIT_SLOW_TIME);
        targets[e] = null;
    }

    void SlowTarget(GameObject e)
    {
        bool assigned = false;
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == e)
                return;
        }

        for (int i = 0; i < targets.Length; i++)
        {

            if (targets[i] == null)
            {
                print(e);
                targets[i] = e;
                WeaponEngine trgt = e.GetComponent<WeaponEngine>();
                if (trgt != null && !trgt.shielded)
                {
                    trgt.ModifyHealth(-WeaponEngineValues.ACIDSPIT_TOUCH_DAMAGE);
                    trgt.getMotion().TopSpeedMod(WeaponEngineValues.ACIDSPIT_SLOW_PERCENTAGE, WeaponEngineValues.ACIDSPIT_SLOW_TIME);
                }

                StartCoroutine(ClearGameObject(i));
                return;
            }

        }
    }
    
    private void OnTriggerStay(Collider col)
    {
        if ((col.CompareTag("Dino") || col.CompareTag("Ai")))
        {
            SlowTarget(col.gameObject);
        }
    }

    public override void Enable()
    {
        renderer.enabled = true;
        Invoke("Disable", WeaponEngineValues.ACIDSPIT_DURATION);
    }

    public override void Disable()
    {
        PlayVFX();
        collider.enabled = false;
    }
}
