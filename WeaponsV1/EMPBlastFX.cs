using UnityEngine;
using System.Collections;

//EMPBlastFX
//Will temporarily disable weapons for the afflicted avatar when called
public class EMPBlastFX : StatusFX
{

    protected override IEnumerator RunVFX()
    {
        return base.RunVFX();
    }

    protected override void Start()
    {
        coRoutineTime = 0;
        coRoutineRate = 1 / WeaponEngineValues.EMPBLAST_DISABLE_WEAPON_DURATION;
        myEngine = gameObject.GetComponent<WeaponEngine>();
        StartCoroutine("Run");
        StartCoroutine("RunVFX");
    }

    protected override IEnumerator Run()
    {
        if (myEngine != null)
        {
            myEngine.DisableWeapons();
        }

        while (coRoutineTime < 1)
        {
            coRoutineTime += Time.deltaTime * coRoutineRate;
            yield return null;
        }

        if (myEngine != null)
            myEngine.EnableWeapons();

        DestroyMe();
    }

    public override void ResetTimer()
    {
        coRoutineTime = 0;
    }

    public override void RemoveFX()
    {
        myEngine.EnableWeapons();
        DestroyMe();
    }
}
	
