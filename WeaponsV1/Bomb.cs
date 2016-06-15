using UnityEngine;
using System.Collections;

// Bomb
// base class for all Bomb weapon behaviors
// Inherits from Weapon.cs
public class Bomb : Weapon {

	protected bool canFire;
	protected BombObject[] myBombs;

	public override void Fire ()
	{
		LaunchBomb();
	}

	protected virtual void LaunchBomb(){}
	
	public virtual void ActivateBomb(){}
	
	protected virtual void PlayFX(){}
}
