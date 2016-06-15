using UnityEngine;
using System.Collections;

public class ForwardBombs : Bomb
{

    protected override void PlayFX()
    {

    }

    public override string GetName()
    {
        return "ForwardBomb";
    }

    // Use this for initialization
    void Start()
    {
        myBombs = new BombObject[WeaponEngineValues.FORWARDBOMB_COUNT];
        for (int i = 0; i < myBombs.Length; i++)
        {
            GameObject e = ((GameObject)(GameObject.Instantiate(Resources.Load("Weapons/Bombs/ForwardBomb"))));
            myBombs[i] = e.GetComponent<BombObject>();
            myBombs[i].Disable();
        }

    }

    protected override void LaunchBomb()
    {
        float launchDelay = 0;
        for (int i = 0; i < myBombs.Length; i++)
        {
            StartCoroutine(PropelBombs(i, launchDelay));
            launchDelay += WeaponEngineValues.FORWARDBOMB_DELAY_TIME;

        }
    }

    private IEnumerator PropelBombs(int index, float time)
    {
        yield return new WaitForSeconds(time);
        PlayFX();
        Vector3 pos = transform.position;
        pos.y += 5;

        myBombs[index].transform.position = pos;
        myBombs[index].Enable();
        Vector3 dir = transform.TransformDirection(new Vector3(0, WeaponEngineValues.FORWARDBOMB_LAUNCH_Y, 1)).normalized * WeaponEngineValues.FORWARDBOMB_LAUNCH_FORCE;
        myBombs[index].transform.rigidbody.AddForce(dir, ForceMode.Impulse);

        yield return new WaitForSeconds(WeaponEngineValues.FORWARDBOMB_ACTIVATE_TIME);
        myBombs[index].setActivated(true);
        myBombs[index].collider.enabled = true;
    }
}
		