using UnityEngine;
using System.Collections;

public class Smite : Melee
{
    public override void PlayVFX()
    {

    }

    void Start()
    {
        sphereCastRadius = WeaponEngineValues.SMITE_RADIUS;
        sphercastRange = WeaponEngineValues.SMITE_RANGE;
        damageApplied = WeaponEngineValues.SMITE_DAMAGE;
        coolDownDuration = WeaponEngineValues.SMITE_COOLDOWN_DURATION;
    }

    public override string GetName()
    {
        return this.ToString();
    }
}
