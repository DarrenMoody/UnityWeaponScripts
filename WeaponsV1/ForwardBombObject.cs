using UnityEngine;
using System.Collections;

public class ForwardBombObject : BombObject
{

    public override void Enable()
    {
        renderer.enabled = true;
        rigidbody.isKinematic = false;
    }

    public override void Disable()
    {
        base.Disable();
        rigidbody.isKinematic = true;
    }

    protected void Explode(GameObject target)
    {
        WeaponEngine trgtEngine = target.GetComponent<WeaponEngine>();

        if (trgtEngine != null)
        {
            trgtEngine.ModifyHealth(-WeaponEngineValues.FORWARDBOMB_DAMAGE);
        }

        PlayVFX();
        Disable();
    }

    void OnCollisionEnter(Collision col)
    {
        string colTag = col.gameObject.tag;
        if ((colTag == "Dino" || colTag == "Ai"))
            Explode(col.gameObject);

    }

    protected override void PlayVFX()
    {
        base.PlayVFX();
    }
}
