using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace MultiplayerShooter
{
public class Gun : MonoBehaviour {

	public int currentGun;
	//store all guns
	public Dictionary<int, CustonGun> guns = new Dictionary<int, CustonGun>();
	public bool isLocked = true;
	public GameObject[] bulletPref;
	public GameObject[] spriteWeapons;
	
	public Transform[] spawnBulletTransf;
	
	public bool  m_Shoot;
	
	public float timeBetweenBullets = 0.15f;//delay among the shots
	
	public float timer;

	public GameObject target;

	public float maxStretch =2.0f;

	 PlayerManager playerManager;
	
	public  AudioClip[] shootSound;
	
	public AudioClip reloadSound;
	
	public AudioClip outOfAmmoSound;
	
 /*********************** END GUN EFFECTS VARIABLES ***************************/

	
	

	// Use this for initialization
	void Start () {
		
		isLocked = true;
		SetGuns();
	    playerManager = GetComponentInParent<PlayerManager>();	
	
	}

	public void SetGuns()
	{
		CustonGun gun = new CustonGun ();

		gun.id = 0;
			
		gun.name = "gun1";

		gun.ammoInPaint = 30;

		gun.ammoPerPents = 30;

		gun.maxAmmo = 300;

		gun.currentAmmo = 300;
		
		gun.onInventory = true;
		
		guns.Add (gun.id, gun);

		CustonGun gun2 = new CustonGun ();

		gun2.id = 1;
			
		gun2.name = "gun2";
		
		gun2.onInventory = true;

		gun2.maxAmmo = 300;

		gun2.ammoInPaint = 30;

		gun2.ammoPerPents = 30;

		gun2.currentAmmo = 300;
		
		guns.Add (gun2.id, gun2);

		CustonGun gun3 = new CustonGun ();

		gun3.id = 2;
			
		gun3.name = "gun3";
		
		gun3.onInventory = true;

		gun3.maxAmmo = 300;

		gun3.ammoInPaint = 30;

		gun3.ammoPerPents = 30;

		gun3.currentAmmo = 300;
		
		guns.Add (gun3.id, gun3);
        if(GetComponentInParent<PlayerManager>().isLocalPlayer)
		{
		  HUDWeaponManager.instance.InicializeInventory();	
		  HUDWeaponManager.instance.SetUpCurrentGun(currentGun);
		}
		
	}

	void Update() {

	  timer += Time.deltaTime; //increments the timer
	
	  Shoot(); //calls shoot method

	 
	}
	

	 public void Shoot()
	 {
	 
	   if (!m_Shoot &&GetComponentInParent<PlayerManager>().isLocalPlayer &&
	                                   !GetComponentInParent<PlayerHealth>().isDead)
		{
		
			m_Shoot = Input.GetButton("Fire1");

		}
	 
		
	   if (m_Shoot && timer >= timeBetweenBullets
			&& Time.timeScale != 0) {
			
		  m_Shoot = false;
		  timer = 0f;  //reset the timer

		  if (GetAmmoInPent () > 0 ) {

			   if(currentGun.Equals(0))
		        {

					SpawnBullet(0); //spawn bullets
						  
				}
				if(currentGun.Equals(1))
		        {

					SpawnBullet(1); //spawn bullets
						  
				}
				 if(currentGun.Equals(2))
		        {
					SpawnBullet(2); //spawn bullets		  
				}

				NetworkManager.instance.EmitShoot(currentGun); //update the server
					 

			    SetCurrentAmmo (GetCurrentAmmo () - 1);

			    SetAmmoInPent (GetAmmoInPent () - 1);

				if(GetComponentInParent<PlayerManager>().isLocalPlayer)
				{
				    HUDWeaponManager.instance.UpdateCurrentAmmoInfo();
 
				}
			   
		  }//END_IF
		  //without ammunition in paint but with ammo in inventory
			if (GetAmmoInPent () <= 0 && guns[currentGun].currentAmmo > 0 ) {

				ReloadGun ();


			}//END_IF

			if (guns[currentGun].currentAmmo <= 0) {


                guns[currentGun].currentAmmo = guns[currentGun].maxAmmo;
				//PlayOutOfAmmoSound();
			}//ENDD_IF		
		  
		
		
	   }//END_IF

	 }
	 
   public void SpawnBullet(int _index)
   {
   
    PlayShootSound(_index);
    	
	GameObject bullet = GameObject.Instantiate (bulletPref[_index],spawnBulletTransf[0].position,
	bulletPref[_index].transform.rotation) as GameObject;
				
	Transform cameraTransform = Camera.main.transform;

	bullet.GetComponent<BulletController> ().isLocalBullet = true;
	
	bullet.GetComponent<BulletController> ().Shoot (target.transform.TransformDirection(Vector3.forward),	GetComponentInParent<PlayerManager>().id);
	
	
	GameObject bullet2 = GameObject.Instantiate (bulletPref[_index],
				spawnBulletTransf[1].position,bulletPref[_index].transform.rotation) as GameObject;

	bullet.GetComponent<BulletController> ().isLocalBullet = true;
	
	bullet2.GetComponent<BulletController> ().Shoot (target.transform.TransformDirection(Vector3.forward),	GetComponentInParent<PlayerManager>().id);
		
	GetComponentInParent<PlayerManager>().DisplayEffects();
			 
			 
  }

  

  	/*
	* method called if the weapon belongs to a network player
	*/
	public void NetworkShoot(int _currentGun)
	{
	  
	  SpawnNetworkBullet(_currentGun);
	  	
	}

	
	public void SpawnNetworkBullet(int _index)
	{
	 
	   GameObject bullet = GameObject.Instantiate (bulletPref[_index],spawnBulletTransf[0].position,
	                                                bulletPref[_index].transform.rotation) as GameObject;
	
	   bullet.GetComponent<BulletController> ().Shoot (target.transform.TransformDirection(Vector3.forward),
	   GetComponentInParent<PlayerManager>().id);
	
	
	   GameObject bullet2 = GameObject.Instantiate (bulletPref[_index],
				spawnBulletTransf[1].position,bulletPref[_index].transform.rotation) as GameObject;
	
	   bullet2.GetComponent<BulletController> ().Shoot (target.transform.TransformDirection(Vector3.forward),	GetComponentInParent<PlayerManager>().id);
		
	   GetComponentInParent<PlayerManager>().DisplayEffects();

	   PlayShootSound(_index);
	
	
	}


   /// <summary>
	/// Reloads the weapon.
	/// </summary>
	void ReloadGun()
	{
		//do bals exist in the inventory?
		if (guns[currentGun].currentAmmo != 0 &&  GetComponentInParent<PlayerManager>().isLocalPlayer)
	    {
		  HUDWeaponManager.instance.UpdateCurrentAmmoInfo();
		}

		if (guns[currentGun].currentAmmo - guns[currentGun].ammoPerPents >= 0) {

			//reload weapon paint
			SetAmmoInPent (guns[currentGun].ammoPerPents);
		} else {

			SetAmmoInPent (guns[currentGun].currentAmmo);
		}

	}

	
	
	/// <summary>
	/// Gets the ammo in pent.
	/// </summary>
	/// <returns>The ammo in pent.</returns>
	public int GetAmmoInPent()
	{
		return guns[currentGun].ammoInPaint;
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="_ammo">Ammo.</param>
	public void SetAmmoInPent(int _ammo)
	{
		if (_ammo >= 0) {
			guns[currentGun].ammoInPaint = _ammo;
		}
	}


	/// <summary>
	/// Gets the ammo in paint.
	/// </summary>
	/// <returns>The ammo in paint.</returns>
	public int GetCurrentAmmo()
	{
		return guns[currentGun].currentAmmo;
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="_ammo">Ammo.</param>
	public void SetCurrentAmmo(int _ammo)
	{

		if (_ammo >= 0) {
			guns[currentGun].currentAmmo = _ammo;
		}


	}


	

	//---------------AUDIO METHODS--------
	public void PlayOutOfAmmoSound()
	{
		GetComponent<AudioSource>().PlayOneShot(outOfAmmoSound);
	}

	public void PlayReloadSound()
	{
		GetComponent<AudioSource>().PlayOneShot(reloadSound);
	}

	public void PlayShootSound(int _index)
	{
	   
		GetComponent<AudioSource>().PlayOneShot(shootSound[_index]);
	}
	

}//END_CLASS
}//END_NAMESPACE