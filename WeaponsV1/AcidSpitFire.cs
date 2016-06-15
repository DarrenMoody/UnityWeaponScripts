using UnityEngine;
using System;
using System.Collections;

public class AcidSpitFire : Weapon 
{
	private Vector3 ACID_SPAWNER_OFFSET = new Vector3(0,3,3);
	//This script needs to know when the fire button is being continuously pressed and when it has stopped being pressed.
	
	public float fireTime;
	private bool firing = false;
	private bool onCoolDown = false;
	[SerializeField]
	private float coolDownDuration ;

	public bool OnCoolDown{get{return onCoolDown;} set{onCoolDown = value;}}
	public float CoolDownDuration{get{return coolDownDuration;} set{coolDownDuration = value;}}

	private float timeHeld;
	float timeHeldCache;

	void Awake()
	{
		Initialize();
	}

	void Update()
	{

	}
	void Initialize()
	{
		fireTime = WeaponDamage.ACIDSPIT_FIRE_TIME;
		coolDownDuration = WeaponDamage.ACIDSPIT_COOLDOWN_DURATION;
		maxNumberOfUses = WeaponDamage.ACIDSPIT_MAX_USES;
		numberOfUses = maxNumberOfUses;
	}

	void OnEnable() 
	{
		//FireButton.range += GunFire;
//		NumberOfUses = MaxNumberOfUses;
		
	}
	
	void OnDisable() 
	{
		//FireButton.range -= GunFire;
	}


	public override string GetName()
	{
		return this.ToString();
	}

	public override void Fire()
	{
		if(numberOfUses > 0)
		{
			if(OnCoolDown == false)
			{
				if(firing == false)
				{
					StartCoroutine(FireWeapon());
				}
			}
		}
	}
	public override void ReleaseFire()
	{
		StopFire();
	}
	
	public void StopFire()
	{
		if(OnCoolDown == false)
		{
			StartCoroutine(Cooldown());
		}
	}
	
	IEnumerator FireWeapon() 
	{
		firing = true;
		Transform obj = AcidSpitPooling.current.GetPooledObject();
		
		if(obj == null) yield return null;
		obj.position = transform.TransformPoint(ACID_SPAWNER_OFFSET);
		obj.rotation = Quaternion.Euler( transform.TransformDirection(Vector3.forward));
		obj.gameObject.SetActive(true);
		numberOfUses--;
		yield return new WaitForSeconds(fireTime);
		firing = false;
		
	}
	
	IEnumerator Cooldown()
	{
		OnCoolDown = true;
		yield return new WaitForSeconds(CoolDownDuration);
		OnCoolDown = false;
	}
}
