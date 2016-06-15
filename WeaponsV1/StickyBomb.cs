using UnityEngine;
using System.Collections;

public class StickyBomb : Bomb
{
    protected override void PlayFX()
    {

    }

    public override string GetName()
    {
        return "StickyBomb";
    }

    // Use this for initialization
    void Start()
    {
        myBombs = new BombObject[1];
    }

    void InstantiateNew()
    {
        GameObject obj = ((GameObject)(GameObject.Instantiate(Resources.Load("Weapons/Bombs/StickyBomb"))));
        myBombs[0] = obj.GetComponent<StickyBombObject>();
    }

    protected override void LaunchBomb()
    {
        if (myBombs[0] == null || ((StickyBombObject)(myBombs[0])).isActive)
        {
            InstantiateNew();
        }
        else
        {
            myBombs[0].MisFire();
            InstantiateNew();
        }

        myBombs[0].transform.position = transform.position;

        Vector3 dir = transform.TransformDirection(new Vector3(0, WeaponEngineValues.STICKYBOMB_LAUNCH_Y, 1)).normalized * WeaponEngineValues.STICKYBOMB_LAUNCH_FORCE;
        myBombs[0].transform.rigidbody.AddForce(dir, ForceMode.Impulse);

        Invoke("ActivateBomb", WeaponEngineValues.STICKYBOMB_ACTIVATE_TIME);
    }

    public override void ActivateBomb()
    {
        myBombs[0].setActivated(true);
        myBombs[0].collider.enabled = true;
    }
}
	