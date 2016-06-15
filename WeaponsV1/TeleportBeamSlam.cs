using UnityEngine;
using System.Collections;

public class TeleportBeamSlam : Weapon
{
		const float TELEPORT_DISTANCE = 7;
		private Vector2 AnimationTime = new Vector2 (.1f, .3f);
		private Vector3 WEAPON_SPAWNER_OFFSET = new Vector3 (0, 3, 0);
		private Transform mesh;
		private RaycastHit[] sphereHits;
		[SerializeField]
		private float
				theRadius;
		[SerializeField]
		private float
				theRange;
		[SerializeField]
		private float
				theDuration;
//	[SerializeField]
		//private int maxNumberOfUses;
		//[SerializeField]
		//private int numberOfUses;

		[SerializeField]
		private ParticleSystem
				slam;
		private bool onCoolDown = false;
		[SerializeField]
		private float
				coolDownDuration;
	
		public float TheRadius{ get { return theRadius; } set { theRadius = value; } }

		public float TheRange{ get { return theRange; } set { theRange = value; } }

		public float TheDuration{ get { return theDuration; } set { theDuration = value; } }

		public int MaxNumberOfUses{ get { return maxNumberOfUses; } set { maxNumberOfUses = value; } }

		public int NumberOfUses{ get { return numberOfUses; } set { numberOfUses = value; } }

		public ParticleSystem Slam{ get { return slam; } set { slam = value; } }
	
		public bool OnCoolDown{ get { return onCoolDown; } set { onCoolDown = value; } }

		public float CoolDownDuration{ get { return coolDownDuration; } set { coolDownDuration = value; } }
	
		void OnEnable ()
		{
//		Slam.Stop();
//		FireButton.melee += UseAttack;
//		NumberOfUses = MaxNumberOfUses;
		}

		void Awake ()
		{
				Initialize ();
		}
	
		void OnDisable ()
		{
				//FireButton.melee -= UseAttack;
		}

		public override string GetName ()
		{
				return this.ToString ();
		}

		void Initialize ()
		{
				mesh = transform.parent.FindChild ("dilophosaurus").transform;
				theRadius = WeaponDamage.TELEPORT_SLAM_RADIUS;
				theRange = WeaponDamage.TELEPORT_SLAM_RANGE;
				theDuration = WeaponDamage.TELEPORT_SLAM_DURATION;
				coolDownDuration = WeaponDamage.TELEPORT_SLAM_COOLDOWN_DURATION;
				maxNumberOfUses = WeaponDamage.TELEPORT_SLAM_MAX_USES;
				numberOfUses = maxNumberOfUses;
				GameObject t = ((GameObject)(GameObject.Instantiate (Resources.Load ("VFX/Slam/Prefab/slam"))));
				t.transform.parent = transform;
				t.transform.localPosition = WEAPON_SPAWNER_OFFSET;
				t.transform.localRotation = Quaternion.Euler (90, -180, 0);
				Slam = t.GetComponent<ParticleSystem> ();
				slam.Stop ();
		}
	
		public override void Fire ()
		{
				if (onCoolDown || numberOfUses <= 0)
						return;
				print ("Attack");


				if (OnCoolDown != true) {
						if (NumberOfUses > 0) {
								StartCoroutine (Attack ());
								StartCoroutine (MoveModel());
						} else {
								Debug.Log ("Out of uses.");
						}
				}
		}

		IEnumerator MoveModel ()
		{
				float t = 0;
				float r = 1 / AnimationTime.x;
				Vector3 start = mesh.localPosition;

				while (t<1) {
						t += Time.deltaTime * r;
						mesh.localPosition = Vector3.Lerp (start, new Vector3 (start.x, start.y, start.z + TELEPORT_DISTANCE), t);
						yield return null;
				}

				t = 0;
				r = 1 / AnimationTime.y;
				start = mesh.localPosition;

				while (t<1) {
						t += Time.deltaTime * r;
						mesh.localPosition = Vector3.Slerp (start, Vector3.zero, t);
						yield return null;
				}
		

		}

		IEnumerator Attack ()
		{
				Slam.Play ();
				sphereHits = Physics.SphereCastAll (new Vector3 (transform.position.x, transform.position.y, transform.position.z), TheRadius, transform.forward, TheRange);
		
				Vector3 forward = transform.TransformDirection (Vector3.forward) * TheRange;
				Debug.DrawRay (transform.position, forward, Color.red, TheDuration);
		
				foreach (RaycastHit hit in sphereHits) {
						if (hit.transform.gameObject.GetComponent<RacerInteractionManager> () != null) {
								RacerInteractionManager.hitSomething (hit.transform, transform);
						}
			
				}
				NumberOfUses--;
				StartCoroutine (Cooldown ());
				yield return new WaitForSeconds (TheDuration);
				Slam.Stop ();
		
		}
	
		IEnumerator Cooldown ()
		{
				OnCoolDown = true;
				yield return new WaitForSeconds (CoolDownDuration);
				OnCoolDown = false;
		}
}
