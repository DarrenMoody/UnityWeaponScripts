using UnityEngine;
using System.Collections;


// Base Class for all active bomb entities
public class BombObject : MonoBehaviour
{
    protected const int RAYCAST_Y_MODIFIER = 50;
    public bool activated = false;
    protected float neighborRadius = 0;
    protected Collider myCollider;

    // Fire
    // This method is always overridden
    protected virtual void Fire()
    {
    }

    public bool isActivated()
    {
        return activated;
    }

    // setActivated
    public void setActivated(bool value)
    {
        activated = value;
    }

    // Enable
    // This method is usually overridden
    public virtual void Enable()
    {
        collider.enabled = true;
        renderer.enabled = true;
    }

    // Disable
    // This method is usually overridden
    public virtual void Disable()
    {
        collider.enabled = false;
        renderer.enabled = false;
    }

    // MisFire
    // Will play all FX of the bomb detonating but will not cause damage
    public virtual void MisFire()
    {
        PlayVFX();
        DestroyMe();
    }

    // Destroy me
    // Removes the gameobject from scene
    public virtual void DestroyMe()
    {
        Destroy(gameObject);
    }

    // FindNearbyTargets
    // Finds all nearby targets within radius
    protected void FindNearbyTargets(ref GameObject[] nearbyDinos)
    {
        {
            for (int i = 0; i < nearbyDinos.Length; i++)
            {
                nearbyDinos[i] = null;
            }

            // Create spherecast ray
            Ray thisRay = new Ray(new Vector3(transform.position.x, transform.position.y + RAYCAST_Y_MODIFIER, transform.position.z), Vector3.down);
            RaycastHit[] hits;

            float extents = 0;

            if (myCollider.GetType() == typeof(SphereCollider))
                extents = ((SphereCollider)myCollider).radius;

            if (collider.GetType() == typeof(BoxCollider))
                extents = myCollider.bounds.extents.x;

            hits = Physics.SphereCastAll(thisRay, ((transform.localScale.x * extents) + neighborRadius), RAYCAST_Y_MODIFIER);

            // Iterate through RaycastHits to save valid nearbyDinos to array, ensure objects are dinos before adding them
            if (hits.Length != 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.tag == "Dino" || hits[i].collider.tag == "Ai")
                    {
                        bool assigned = false;
                        for (int j = 0; j < nearbyDinos.Length; j++)
                        {
                            if (hits[i].collider.gameObject == nearbyDinos[j])
                            {
                                assigned = true;
                            }
                            if (nearbyDinos[j] == null && assigned == false)
                            {
                                nearbyDinos[j] = hits[i].collider.gameObject;
                                assigned = true;
                            }
                        }
                    }
                }
                return;
            }
        }
    }

    protected virtual void PlayVFX()
    {
    }
}
