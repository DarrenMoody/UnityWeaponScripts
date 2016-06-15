using UnityEngine;
using System.Collections;

public class FlamethrowerBurnFX : StatusFX
{
    private float coRoutineBurnNextHit = 0;
    public float timeLeft;

    // Start burnFX
    void StartFX()
    {

    }


    // Stop burn FX
    void EndFX()
    {

    }

    // Use this for initialization
    protected override void Start()
    {

    }

    void Awake()
    {
        coRoutineRate = 1 / WeaponEngineValues.FLAMETHROWER_DOT_DURATION;
        myEngine = gameObject.GetComponent<WeaponEngine>();

    }

    protected override IEnumerator Run()
    {
        StartFX();
        while (coRoutineTime < 1)
        {
            timeLeft = coRoutineBurnNextHit;
            coRoutineTime += Time.deltaTime * coRoutineRate;
            if (coRoutineTime >= coRoutineBurnNextHit)
            {
                coRoutineBurnNextHit += (WeaponEngineValues.FLAMETHROWER_DOT_DURATION / (WeaponEngineValues.FLAMETHROWER_DOT_FREQUENCY - 1) / WeaponEngineValues.FLAMETHROWER_DOT_DURATION);
                ApplyDamage();
            }
            yield return null;
        }
        EndFX();
        DestroyMe();
    }

    public override void ResetTimer()
    {
        coRoutineTime = 0;
        coRoutineBurnNextHit = 0;
    }

    public void SetTimerAndActivate(float t, float inc)
    {
        StopAllCoroutines();
        coRoutineTime = t;
        coRoutineBurnNextHit = inc;
        StartCoroutine("Run");
    }

    void ApplyDamage()
    {
        if (myEngine.CurHealth - WeaponEngineValues.FLAMETHROWER_DOT_DAMAGE <= 0)
        {
            myEngine.ModifyHealth(-WeaponEngineValues.FLAMETHROWER_DOT_DAMAGE);
            DestroyMe();
            return;
        }

        myEngine.ModifyHealth(-WeaponEngineValues.FLAMETHROWER_DOT_DAMAGE);
    }

    void OnCollisionEnter(Collision col)
    {
        if (((col.gameObject.CompareTag("Dino") || col.gameObject.CompareTag("Ai"))) && col.gameObject != gameObject)
        {
            FlamethrowerBurnFX countdown = col.gameObject.GetComponent<FlamethrowerBurnFX>();

            if (countdown == null)
            {
                countdown = col.gameObject.AddComponent<FlamethrowerBurnFX>();
                countdown.SetTimerAndActivate(coRoutineTime, coRoutineBurnNextHit);
            }
        }
    }
}
