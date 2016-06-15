using UnityEngine;
using System.Collections;

public class Flamethrower : Melee
{
    bool inFlame = false;

    void StartFX()
    {
    }

    void EndFX()
    {
    }

    public override string GetName()
    {
        return this.ToString();
    }


    // Use this for initialization
    protected override void Start()
    {
        sphereCastRadius = WeaponEngineValues.FLAMETHROWER_RADIUS;
        sphercastRange = WeaponEngineValues.FLAMETHROWER_RANGE;
        coolDownDuration = WeaponEngineValues.SMITE_COOLDOWN_DURATION;
    }

    protected override void Attack()
    {
        if (!inFlame)
            StartCoroutine("FireFlame");
    }

    // If a target is found in transform.forward, will apply immediate burn damage and apply DOT FX.
    // If DOT FX is already active on target, will reset the DOT timer
    private IEnumerator FireFlame()
    {
        inFlame = true;
        float t = 0;
        float r = 1 / WeaponEngineValues.FLAMETHROWER_INFLAME_DURATION;
        float nextHitTime = 0;
        StartFX();
        while (t < 1)
        {
            t += Time.deltaTime * r;
            if (t >= nextHitTime)
            {
                nextHitTime += (((float)(WeaponEngineValues.FLAMETHROWER_INFLAME_DURATION) / (WeaponEngineValues.FLAMETHROWER_INFLAME_DMG_FREQUENCY - 1)) / (WeaponEngineValues.FLAMETHROWER_INFLAME_DURATION));
                GameObject[] targets = new GameObject[WeaponEngineValues.MAX_PLAYERS];
                if (FindForwardTargets(ref targets))
                {
                    foreach (GameObject curTrgt in targets)
                    {
                        if (curTrgt != null)
                        {
                            curTrgt.GetComponent<WeaponEngine>().ModifyHealth(-WeaponEngineValues.FLAMETHROWER_INFLAME_DAMAGE);
                            FlamethrowerBurnFX burn = curTrgt.GetComponent<FlamethrowerBurnFX>();
                            if (burn != null)
                            {
                                burn.ResetTimer();
                            }
                            else
                            {
                                burn = curTrgt.AddComponent<FlamethrowerBurnFX>();
                                burn.SetTimerAndActivate(0, 0);
                            }
                        }
                    }
                }
            }
            yield return null;
        }
        EndFX();
        StartCoroutine("StartCooldown");
        inFlame = false;
    }
}
