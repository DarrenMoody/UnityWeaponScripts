using UnityEngine;
using System.Collections;

public class AcidSpit : Bomb
{

    protected override void PlayFX()
    {

    }

    public override string GetName()
    {
        return "AcidSpit";
    }

    // Use this for initialization
    void Start()
    {
        myBombs = new BombObject[1];
        GameObject obj = ((GameObject)(GameObject.Instantiate(Resources.Load("Weapons/Bombs/AcidSpit"))));
        myBombs[0] = obj.GetComponent<BombObject>();
        myBombs[0].Disable();
    }

    protected override void LaunchBomb()
    {
        if (myBombs[0].gameObject.activeSelf)
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
                myBombs[0].transform.rotation = Quaternion.LookRotation(transform.forward, hits[i].normal);
                CancelInvoke("ActivateBomb");
                Invoke("ActivateBomb", WeaponEngineValues.ACIDSPIT_ACTIVATE_TIME);
            }
        }
    }

    public override void ActivateBomb()
    {
        myBombs[0].setActivated(true);
        myBombs[0].collider.enabled = true;
    }
}
