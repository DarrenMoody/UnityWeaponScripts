using UnityEngine;
using System.Collections;

public class LandMine : Bomb
{
    const int SPHERECAST_Y_MODIFIER = 5;

    protected override void PlayFX()
    {

    }

    public override string GetName()
    {
        return "LandMine";
    }

    void Start()
    {
        myBombs = new BombObject[1];
        GameObject obj = ((GameObject)(GameObject.Instantiate(Resources.Load("Weapons/Bombs/LandMine"))));

        myBombs[0] = obj.GetComponent<BombObject>();
        myBombs[0].Disable();
    }

    protected override void LaunchBomb()
    {
        if (myBombs[0].isActivated())
            myBombs[0].MisFire();

        myBombs[0].Disable();

        Vector3 pos = transform.position;
        pos.y += 5;
        RaycastHit[] hits = Physics.RaycastAll(pos, Vector3.down, Mathf.Infinity);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.name == "Track")
            {

                pos = hits[i].point;
                myBombs[0].Enable();
                myBombs[0].transform.position = pos;
                CancelInvoke("ActivateBomb");
                Invoke("ActivateBomb", WeaponEngineValues.LANDMINE_ACTIVATE_TIME);
            }
        }

    }

    public override void ActivateBomb()
    {
        myBombs[0].setActivated(true);
    }
}
