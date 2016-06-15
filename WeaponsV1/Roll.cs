﻿using UnityEngine;
using System.Collections;

public class Roll : Melee
{

    protected override void Start()
    {
        sphereCastRadius = WeaponEngineValues.ROLL_RADIUS;
        sphercastRange = WeaponEngineValues.ROLL_RANGE;
        damageApplied = WeaponEngineValues.ROLL_DAMAGE;
        coolDownDuration = WeaponEngineValues.ROLL_COOLDOWN_DURATION;

    }

    public override string GetName()
    {
        return this.ToString();
    }

    // aesthetic scripting for when the player attacks


    protected override void Attack()
    {
        base.Attack();
    }

    public override void ApplyDamage(ref GameObject[] targets)
    {
        base.ApplyDamage(ref targets);
    }

    public override void PlayVFX()
    {

    }
}
