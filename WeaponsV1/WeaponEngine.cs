/*
 * Each dino should have one WeaponEngine component on its main controller/collider, 
 * this will be the main handler for Health, Damage, Respawns, and Pickups
 * 
 * When the script is loaded, it will load the weapons specified in the inspector and 
 * will add the other components.
 * 
 * There are 2 types of Weapons; Bombs and Melees. Specific funtionalities are specified in 
 * the game design document on the google docs. 
 * 
 * Melee weapons are normally cooldown based attacks that rely on spherecasts or raycasts etc
 * to find adjacent targets.
 * Bomb weapons normally rely on spawned entities that will deliver damage and status fx on contact.
 *  
 * Pickups are based on tag collision objects, so no extra scripting should be needed on the the 
 * object itself is with the exception of any graphics fx.
*/

using UnityEngine;
using System.Collections;

public enum PickupType
{
		Health,
		Shield,
		Bomb
}
;

// WeaponEngineValues
// Weapon values specified through scripting

public struct WeaponEngineValues
{
    // General
    public const float PICKUP_SHIELD_DURATION = 5;
    public const int PICKUP_HEALTH_GIVEN = 50;
    public const int MAX_PLAYERS = 4;

    // Diloph
    public const float ACIDSPIT_ACTIVATE_TIME = 1;
    public const float ACIDSPIT_DURATION = 5;
    public const int ACIDSPIT_TOUCH_DAMAGE = 3;
    public const float ACIDSPIT_SLOW_PERCENTAGE = .3f;
    public const float ACIDSPIT_SLOW_TIME = 3;

    public const float TELEPORT_SLAM_RADIUS = 2;
    public const float TELEPORT_SLAM_RANGE = 50;
    public const int TELEPORT_SLAM_DAMAGE = 3;
    public const float TELEPORT_SLAM_COOLDOWN_DURATION = 1;

    // Hesp
    public const float SONIC_SCREAM_RADIUS = 2;
    public const float SONIC_SCREAM_RANGE = 50;
    public const int SONIC_SCREAM_DAMAGE = 10;
    public const float SONIC_SCREAM_COOLDOWN_DURATION = 3;


    // Spino
    public const float ROLL_RADIUS = 2;
    public const float ROLL_RANGE = 100;
    public const int ROLL_DAMAGE = 10;
    public const float ROLL_COOLDOWN_DURATION = 3;

    public const int STICKYBOMB_BOMB_DAMAGE = 20;
    public const float STICKYBOMB_EXPLOSIVE_TIMER = 50;
    public const float STICKYBOMB_LAUNCH_FORCE = 125;
    // Higher y = higher arc
    public const float STICKYBOMB_LAUNCH_Y = .5f;
    // ACTIVATE time, is how long it takes for the bomb to become collidable after launching
    public const float STICKYBOMB_ACTIVATE_TIME = .5f;


    // Raptor
    public const float SMITE_RADIUS = 2;
    public const float SMITE_RANGE = 50;
    public const int SMITE_DAMAGE = 10;
    public const float SMITE_COOLDOWN_DURATION = 1;

    public const int FORWARDBOMB_DAMAGE = 20;
    public const int FORWARDBOMB_COUNT = 3;
    public const float FORWARDBOMB_LAUNCH_FORCE = 125;
    // higher y= higher arc
    public const float FORWARDBOMB_LAUNCH_Y = .5f;
    public const float FORWARDBOMB_ACTIVATE_TIME = .5f;
    // sets the spacing between the launches
    public const float FORWARDBOMB_DELAY_TIME = .5f;

    // TRex
    // Landmine splash damage is applied on a falloff where how close targets are to explosion source determines what percent of damge is applied
    // EX: Triggering character is 0% of radius away from target and thus gets 100% of blast,  neighbor is 75% of radius away, and thus gets 25% of max damage applid
    public const float LANDMINE_ACTIVATE_TIME = 1f;
    public const float LANDMINE_MAX_EXPLOSION_DAMAGE = 100;
    public const int LANDMINE_SPLASH_RADIUS = 20;

    public const float FLAMETHROWER_RADIUS = 4;
    public const float FLAMETHROWER_RANGE = 60;
    public const float FLAMETHROWER_COOLDOWN_DURATION = 5;
    public const float FLAMETHROWER_INFLAME_DURATION = 5;
    public const float FLAMETHROWER_DOT_DURATION = 5;
    public const int FLAMETHROWER_INFLAME_DAMAGE = 10;
    public const int FLAMETHROWER_DOT_DAMAGE = 5;
    // how many times will inFlame damage be applied in the FireDuration
    public const int FLAMETHROWER_INFLAME_DMG_FREQUENCY = 5;
    // how many times will DOT damage be applied in the DOT timeframe
    public const int FLAMETHROWER_DOT_FREQUENCY = 5;

    public const int SHOCKWAVE_DAMAGE = 5;
    public const float SHOCKWAVE_RADIUS = 15;
    // Force applied is based on a linear falloff similar to the Splash Damage bombs
    public const float SHOCKWAVE_PUSH_MAX_FORCE = 6000;
    public const float SHOCKWAVE_COOLDOWN_DURATION = 3;

    public const int EMPBLAST_DAMAGE = 2;
    public const float EMPBLAST_RADIUS = 15;
    public const float EMPBLAST_DISABLE_WEAPON_DURATION = 10;

}

// WeaponEngine
// Main Script for all Weapon, Damage, and Respawn systems.
public class WeaponEngine : MonoBehaviour
{
    enum WeaponName
    {
        Flamethrower,
        LandMine,
        Roll,
        StickyBomb,
        Smite,
        ForwardBombs,
        TeleportSlam,
        AcidSpit,
        SonicScream,
        EggDrop,
        Shockwave,
        EMPBlast

    }
		;

    enum WeaponSlot
    {
        Melee,
        Bomb
    }
		;

    [SerializeField]
    WeaponName
            meleeWeapon;
    [SerializeField]
    WeaponName
            bombWeapon;
    [SerializeField]
    int
            curHealth;
    public int maxHealth = 100;
    private MotionControl mc;
    private DinoRagdoll myRagdoll;

    public int CurHealth { get { return curHealth; } }


    // Primary and secondary weapons (assigned through inspector)
    private delegate void FireWeapon();
    private FireWeapon melee;
    private FireWeapon bomb;

    public bool canBomb = true;
    public bool shielded = false;

    // Modify Health
    public void ModifyHealth(int value)
    {
        // if shielded, unless its a health pickup, do no modify health
        if (shielded && value < 0)
            return;

        curHealth = Mathf.Clamp(curHealth + value, 0, maxHealth);

        // if player runs out of health
        if (curHealth <= 0)
        {
            StatusFX[] anyFX = gameObject.GetComponents<StatusFX>();
            if (anyFX.Length != 0)
            {
                foreach (StatusFX fx in anyFX)
                {
                    fx.RemoveFX();
                }
            }
            onRespawn();
        }

    }

    private void onRespawn()
    {
        curHealth = maxHealth;
    }

    void Start()
    {
        Initialize();
    }

    // Initialize
    // Will take weapon information from inspector and assign proper components for each,. Will assign starting health and save pointers for frequently used components for simpler referencing.
    void Initialize()
    {
        curHealth = maxHealth;
        mc = gameObject.GetComponent<MotionControl>();
        AttachWeaponComponent(meleeWeapon, WeaponSlot.Melee);
        AttachWeaponComponent(bombWeapon, WeaponSlot.Bomb);
        EnableWeapons();
    }

    public MotionControl getMotion()
    {
        if (mc != null)
            return mc;

        return null;
    }

    // Attach Weapon Component
    // Will assign the weapon component and entities for trigger events
    // parameters:  1, name of weapon, which slot (primary or secondary)
    private void AttachWeaponComponent(WeaponName newWeapon, WeaponSlot assignTo)
    {
        Weapon newComponent = null;

        switch (newWeapon)
        {
            case WeaponName.Flamethrower:
                newComponent = gameObject.AddComponent<Flamethrower>();
                break;
            case WeaponName.LandMine:
                newComponent = gameObject.AddComponent<LandMine>();
                break;
            case WeaponName.Roll:
                newComponent = gameObject.AddComponent<Roll>();
                break;
            case WeaponName.StickyBomb:
                newComponent = gameObject.AddComponent<StickyBomb>();
                break;
            case WeaponName.Smite:
                newComponent = gameObject.AddComponent<Smite>();
                break;
            case WeaponName.ForwardBombs:
                newComponent = gameObject.AddComponent<ForwardBombs>();
                break;
            case WeaponName.TeleportSlam:
                newComponent = gameObject.AddComponent<TeleportSlam>();
                break;
            case WeaponName.AcidSpit:
                newComponent = newComponent = gameObject.AddComponent<AcidSpit>();
                break;
            case WeaponName.SonicScream:
                newComponent = newComponent = gameObject.AddComponent<SonicScream>();
                break;
            case WeaponName.EggDrop:
                newComponent = gameObject.AddComponent<EggDrop>();
                break;
            case WeaponName.Shockwave:
                newComponent = gameObject.AddComponent<Shockwave>();
                break;
            case WeaponName.EMPBlast:
                newComponent = gameObject.AddComponent<EMPBlast>();
                break;
        }
        if (newComponent != null)
        {
            switch (assignTo)
            {
                case WeaponSlot.Melee:
                    melee = newComponent.Fire;
                    break;
                case WeaponSlot.Bomb:
                    bomb = newComponent.Fire;
                    break;
            }
        }
    }

    // Enable Weapons
    public void EnableWeapons()
    {
        UsedMelee += melee;
        UsedBomb += bomb;
    }

    // DisableWeapons
    public void DisableWeapons()
    {
        UsedMelee -= melee;
        UsedBomb -= bomb;
    }

    // Update is called once per frame
    void Update()
    {

        // These input controls are for testing purposes only
        if (Input.GetMouseButtonDown(0) && UsedMelee != null)
        {
            UsedMelee();
        }


        if (Input.GetMouseButtonDown(1) && UsedBomb != null)
        {
            if (canBomb)
            {
                UsedBomb();
                canBomb = false;
            }
        }
    }

    // GiveItem
    // will call correct function when player pickups object 
    // par:  the type of item picked up
    void GiveItem(PickupType type)
    {
        switch (type)
        {
            case PickupType.Bomb:
                canBomb = true;
                return;

            case PickupType.Health:
                ModifyHealth(WeaponEngineValues.PICKUP_HEALTH_GIVEN);
                return;

            case PickupType.Shield:
                StopCoroutine("ActivateShield");
                StartCoroutine("ActivateShield");
                return;
        }
    }


    // IEnumerator
    // will activate and de-activate the shield after shield duration has passed
    IEnumerator ActivateShield()
    {
        shielded = true;
        yield return new WaitForSeconds(WeaponEngineValues.PICKUP_SHIELD_DURATION);
        shielded = false;
    }



    void OnTriggerEnter(Collider col)
    {
        // Pickups are based on collisions and tags,.
        if (col.gameObject.tag == "Health")
            GiveItem(PickupType.Health);

        if (col.gameObject.tag == "Shield")
            GiveItem(PickupType.Shield);

        if (col.gameObject.tag == "Bomb")
            GiveItem(PickupType.Bomb);
    }
}
