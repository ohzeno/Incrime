using UnityEngine;
using System.Collections;
using System;

namespace MultiverseRiders
{
public class Gun : MonoBehaviour {

	public string gunName;
	public int ammo;
	public int maxAmmo = 30;
	public bool isLocked = true;
	public GameObject bulletPref;
	public Transform weaponTransformReference;

	public float shootVolume  = 0.4f;
	public  AudioClip shootSound;
	private  AudioSource shootSoundSource;

	public AudioClip reloadSound;
	private AudioSource reloadSoundSource;

	public AudioClip outOfAmmoSound ;
	private AudioSource outOfAmmoSoundSource ;


	private float reloadTimer ;
	bool reloading;

	public Transform direction;
	
	public bool  m_Shoot;
	
	public float timeBetweenBullets = 0.15f;//delay among the shots
	
	float timer;

	// Use this for initialization
	void Start () {
		ammo = 30;
		isLocked = true;
	
	}
	
	// Update is called once per frame
	void Update () {
	  Shoot();
	}

	
	public void Shoot()
	{
	   try{
	   timer += Time.deltaTime;// looping the time
	   
	  
	   if (m_Shoot && timer >= timeBetweenBullets
			&& Time.timeScale != 0) {
			
			Debug.Log("shooting");
			
			timer = 0f;
			m_Shoot  = false;

		  if (GetAmmo () > 0) {
				
			 SetCurrentAmmo (GetAmmo () - 1);
			 
             GameObject bullet = GameObject.Instantiate (bulletPref, transform.position, Quaternion.identity) as GameObject;
			 if(GetComponentInParent<Player2DManager>().isLocalPlayer)
			 {
			   bullet.GetComponent<BulletController> ().isLocalBullet = true;
			 }
			 bullet.GetComponent<BulletController> ().shooterID = GetComponentInParent<Player2DManager>().id; 
			 bullet.GetComponent<BulletController> ().Shoot (direction);
			 
			 PlayShootSound ();
			
		  }
		  else
		  {
		  // PlayOutOfAmmoSound();
		  }

	   }
	   }
	   catch(Exception e)
	   {
	    Debug.Log(e.ToString());
	   }

	}
	public int GetAmmo()
	{
		return ammo;
	}
	public void SetCurrentAmmo(int _ammo)
	{
		ammo = _ammo;
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

	public void PlayShootSound()
	{
		GetComponent<AudioSource>().PlayOneShot(shootSound);
	}
}
}
