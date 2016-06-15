using UnityEngine;
using System.Collections;

public class StickyBombObject : BombObject
{
    protected override void PlayVFX()
    {

    }

    public void Latch(Transform parent)
    {
        // Scripting for special cases, for instance, when bombs are latched to a new target,  
        // specific places for specific dinos sound fx, etc.

        transform.parent = parent;
    }

    public void ReleaseTarget()
    {
        rigidbody.AddForce(Vector3.up * 5, ForceMode.Impulse);

        collider.enabled = true;
        rigidbody.isKinematic = false;

        isActive = false;
        latched = false;
        transform.parent = null;
    }

    GameObject currentLatch;
    private bool active = false;
    bool latched = false;
    private bool disowned;
    public bool isDisowned { get { return disowned; } }
    public bool isActive { get { return active; } set { active = value; } }

    void Awake()
    {
        collider.enabled = false;
    }

    public void Disown()
    {
        disowned = true;
    }

    public override void Enable()
    {
        renderer.enabled = true;
        collider.enabled = true;
        rigidbody.isKinematic = false;
        rigidbody.velocity = Vector3.zero;
    }

    public override void Disable()
    {
        collider.enabled = false;
        rigidbody.isKinematic = true;
        renderer.enabled = false;
        transform.parent = null;
    }

    public override void MisFire()
    {
        PlayVFX();
        DestroyMe();
    }

    public override void DestroyMe()
    {
        if (disowned)
            Destroy(gameObject);
        else
            Disable();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (!isActive)
        {
            if (col.gameObject.CompareTag("Track") && !latched)
            {
                rigidbody.isKinematic = true;
                latched = true;
                disowned = true;
                return;
            }
            if ((col.gameObject.CompareTag("Dino") || col.gameObject.CompareTag("Ai")) && latched)
            {
                StickyBombFX countdown = col.gameObject.AddComponent<StickyBombFX>();
                countdown.SetAndActivateTimer(0, this);
                isActive = true;
                collider.enabled = false;
                disowned = true;
            }
        }
    }
}
