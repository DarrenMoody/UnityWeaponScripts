using UnityEngine;
using System.Collections;

public class StickyBombFX : StatusFX
{
    public BombObject myBomb;
    private bool canCollide = false;

    private IEnumerator RunVFX()
    {
        while (coRoutineTime < 1)
        {
            // Insert scripting for any cues here
            yield return null;
        }
    }

    protected override void Start()
    {
        myEngine = gameObject.GetComponent<WeaponEngine>();
        coRoutineRate = 1 / WeaponEngineValues.STICKYBOMB_EXPLOSIVE_TIMER;
    }

    protected override IEnumerator Run()
    {
        while (coRoutineTime < 1)
        {
            coRoutineTime += Time.deltaTime * coRoutineRate;
            print(coRoutineTime);
            yield return null;
        }


        if (myEngine != null)
            myEngine.ModifyHealth(-WeaponEngineValues.STICKYBOMB_BOMB_DAMAGE);

        DestroyMe();
        myBomb.DestroyMe();
    }

    public void SetAndActivateTimer(float curT, BombObject entity)
    {
        coRoutineTime = curT;
        Invoke("ActivateCollision", 1);
        myBomb = entity;
        ((StickyBombObject)(myBomb)).Latch(transform);
        StartCoroutine("Run");
        StartCoroutine("RunVFX");
    }

    public override void RemoveFX()
    {
        ((StickyBombObject)(myBomb)).ReleaseTarget();
        DestroyMe();
    }

    private void ActivateCollision()
    {
        canCollide = true;
    }

    void OnCollisionEnter(Collision col)
    {
        string colTag = col.gameObject.tag;
        if ((colTag == "Dino" || colTag == "Ai") && col.gameObject != gameObject && canCollide == true)
        {
            StickyBombFX countdown = col.gameObject.AddComponent<StickyBombFX>();
            countdown.SetAndActivateTimer(coRoutineTime, myBomb);
            DestroyMe();
        }
    }
}
