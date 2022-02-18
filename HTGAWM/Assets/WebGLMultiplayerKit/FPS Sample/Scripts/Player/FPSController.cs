using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class to control the local player .
/// </summary>

namespace FPSExample{

[RequireComponent(typeof (CharacterController))]
public class FPSController : MonoBehaviour
{

   PlayerManager playerManager;
  /*********************** MOVE VARIABLES ***************************/
    [Header("MOVE VARIABLES")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
	public Camera deathCamera;

	public Transform cameraToTarget;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

	
    /*********************** END MOVE VARIABLES ***************************/
	
	
	 /*********************** DEFAULT GUN VARIABLES ***************************/
	[Header("DEFAULT GUN VARIABLES")]
	public string gunName;
	
	//total ammo in Weapon Paint
	public int ammoInPent;

	// current ammo in inventory
	public int currentAmmo;

	// max ammunition for Pents
	public int ammoPerPents;

	// max ammo in inventory
	public int maxAmmo;

	public bool  m_Shoot;  // fire shot trigger

	public bool  unlockShoot; // avoid continuous shooting
	
	public float timeBetweenBullets = 0.15f;  //delay among the shots
	
	public Transform spawnBulletTransf;

	public GameObject fireParticlePref;
	
	public GameObject bulletPref; // bullet prefab
	
	public Animator gunAnimator;
	
	public float reloadTime = 4.15f; // time it takes to reload the weapon
	
	[HideInInspector]
    public bool isReloading = false; // am I in the process of reloading
	
	float timer; // time counter
	
	[HideInInspector]
	public bool isAim;
	
	 [System.Serializable]
    public class MouseInput{
     
	  public bool lockMouse;
    }
	
	[SerializeField] MouseInput mouseControl;
  
	/*********************** GUN EFFECTS VARIABLES ***************************/
	[Header("GUN EFFECTS VARIABLES")]
	
	public  LayerMask[] whatsIsHit; // identifies the layer of objects that the player is clicking
	
	RaycastHit shootHit; //identifies the objects on the target
	
	public float camRayLength = 100f;  //the ray of reach of the mouse click
	
	public Transform weaponTransformReference;// the muzzle point of this weapon

	public Transform[] muzzlePoint;

	public Renderer muzzleFlash = null;     // the muzzle flash for this weapon

	public Light[] lightFlash = null;         // the light flash for this weapon
 

	public GameObject bulletHole = null;    // bullet hole for raycast bullet types
	
	public float shootAnimTime;


	
	/*********************** AUDIO VARIABLES ***************************/
	[Header("AUDIO VARIABLES")]
	public  AudioClip shootSound;
	
	public AudioClip reloadSound;
	
	public AudioClip outOfAmmoSound;
	
 /*********************** END AUDIO EFFECTS VARIABLES ***************************/

  
    void Start()
    {

        characterController = GetComponent<CharacterController>();

        // Lock cursor
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
		
		if(mouseControl.lockMouse)
	    {
	      Cursor.visible = false;
	      Cursor.lockState = CursorLockMode.Locked;
		  Screen.lockCursor = false;
	    }
		
		CanvasManager.instance.SetUpPanelGun(maxAmmo,  GetAmmoInPent ());
		
		playerManager = GetComponent<PlayerManager>();

		
		
		
    }
	
	// update weapon flashes after checking user inout in update function
	void LateUpdate()
	{
		//EnableEffects();
	}

    void Update()
    {
	 if(!GetComponent<PlayerHealth>().isDead)
	 {
		Shoot(); //calls shoot method
        Move();	
	 }
	  
	  
    }
	
	
   /// <summary>
   /// method to manage the shots fired by the weapon .
  /// </summary>
	public void Shoot()
	{

	    timer += Time.deltaTime;   //increments the timer
	    
		// if player pressing right mouse button
	    if (Input.GetMouseButton(1) ) 
		{
		  AimDownSights.instance.EnableAim();
		  isAim = true;
		}
		else
		{
		  if(isAim)
		  { //resets the aim
		    isAim = false;
			AimDownSights.instance.DisableAim();
		  }
		}
		//if the player pressed the left button
	    if( Input.GetButtonDown ("Fire1")&&unlockShoot)
		{
		  m_Shoot = true; // enable shooting
		  unlockShoot = false;
		}
		//if the player took the finger off the trigger
		if(Input.GetButtonUp("Fire1"))
	    {
			unlockShoot = true;
			DisableEffects();
	    }
		
	
	   if (m_Shoot && timer >= timeBetweenBullets
			&& Time.timeScale != 0) {

		   m_Shoot = false; // disable shooting
			
		  timer = 0f;  //reset the timer

		  if (GetAmmoInPent() > 0 && !isReloading) {

			CameraShake.Shake (0.2f, 0.03f);

			 PlayShootSound();

			 SpawnBullet(); //spawn bullets
			 
			 SpawnEffects();

			 EnableEffects();
			   
		     SetCurrentAmmo(GetCurrentAmmo()- 1);

	         SetAmmoInPent (GetAmmoInPent () - 1);
			 
			 UpdateAnimator("OnFire");

			 CanvasManager.instance.UpdatePanelGun(GetAmmoInPent ()); //updates the gun's HUD

			 
			
					
		  }
		  //without ammunition in paint but with ammo in inventory
		  if (GetAmmoInPent () <= 0 && currentAmmo > 0 && !isReloading) {
           
				ReloadWeapon ();


			}//END_IF

		  if (currentAmmo <= 0) {

			PlayOutOfAmmoSound ();	
		   }//ENDD_IF

		}//END_IF

	
	}
	
	/// <summary>
	/// Reloads the weapon.
	/// </summary>
	void ReloadWeapon()
	{
        
		if(isAim)
		{ //resets the aim
		    isAim = false;
			AimDownSights.instance.DisableAim();
		}

		//do bals exist in the inventory?
		if (currentAmmo != 0) {

			// if out of bullets reload
			 UpdateAnimator("OnReload");
			
		}


		if (currentAmmo - ammoPerPents >= 0) {

			//reload weapon paint
			SetAmmoInPent (ammoPerPents);
		} else {

			SetAmmoInPent (currentAmmo);
		}

	}
	
	
	void Move()
	{
	   // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
       
        float curSpeedX =  runningSpeed* Input.GetAxis("Vertical");
        float curSpeedY =  runningSpeed * Input.GetAxis("Horizontal");
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

	

		if(curSpeedX!=0 || curSpeedY!=0 && !Input.GetMouseButton(1))
		{
		 UpdateAnimator("OnRun");
		}
		else if(curSpeedX ==0 &&
		!m_Shoot &&!Input.GetMouseButton(1)|| curSpeedX ==0 &&!m_Shoot 
		                              &&!Input.GetMouseButton(1) &&!isAim&&!isReloading)
		{
		   UpdateAnimator("OnIdle");
		}

        if (Input.GetButton("Jump") && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
			NetworkManager.instance.EmitAnimation("OnJump");
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

     
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
		
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
	}
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////HELPERS ;) ////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	
	public void SpawnBullet()
	{
	    GameObject bullet = GameObject.Instantiate (bulletPref, spawnBulletTransf.position, Quaternion.identity) as GameObject;
			 
	    bullet.GetComponent<Bullet3DController> ().isLocalBullet = true;
			 
	    bullet.GetComponent<Bullet3DController> ().shooterID = playerManager.id; 

		Ray camRay = Camera.main.ScreenPointToRay (new Vector3(Screen.width/2,Screen.height/2,Screen.height/2));
			
	    bullet.GetComponent<Bullet3DController> ().Shoot (camRay.direction,gameObject.name);
		
		
		NetworkManager.instance.EmitShoot(camRay.direction); //update the server
			 
			 
	}
	
	public void SpawnEffects()
	{
	  //it doesn't import the movement the mouse will be pointed always for the center of the screen, 
		//soon we obtain the objects that is in the center of the screen
		Ray camRay = Camera.main.ScreenPointToRay (new Vector3(Screen.width/2,Screen.height/2,Screen.height/2));

		for (int k = 0; k < whatsIsHit.Length; k++)
		{
			// camRay: circle detecting, shootHit: object detected  camRayLength: max Ray the detection sphere, 
			//whatsIsHit [k] = layer of the object detected
		    if (Physics.Raycast (camRay, out shootHit, camRayLength, whatsIsHit [k])) 
			{
				 Quaternion newRotation; 
				    
				 Vector3 playerToMouse;

		
						
				 switch (shootHit.collider.gameObject.transform.tag)
				  {

				      case "bullet":
					  // do nothing if 2 bullets collide
					  break;
				      case "Obstacle":
					  break;
					  case "wood":
					  // add wood impact effects
					  break;
					  case "stone":
					  // add stone impact effect
					  break;
					  case "ground":
					 
					  break;
				      
				      case "Floor":
					  // add dirt or ground  impact effect
					   
					    GameObject newBulletHole = Instantiate (bulletHole, shootHit.point, Quaternion.FromToRotation (Vector3.up, shootHit.normal)) as GameObject;
					

					   break;
					   default: // default impact effect and bullet hole
					   break;
				   }//END_ SWITCH
			 }//END_IF
			 
			}//END_FOR
			

	}
	
	
   
   
	
	public void EnableEffects()
	{
	
		//muzzleFlash.transform.localRotation = Quaternion.AngleAxis(Random.value * 57.3f, Vector3.forward);
		//muzzleFlash.enabled = true;// enable the muzzle and light flashes
		lightFlash[0].enabled = true;
		lightFlash[1].enabled = true;
		GameObject newFireParticle = Instantiate (fireParticlePref, muzzlePoint[0].position, fireParticlePref.transform.rotation) as GameObject;
		GameObject newFireParticle2 = Instantiate (fireParticlePref, muzzlePoint[1].position, fireParticlePref.transform.rotation) as GameObject;
		
	
	}

	public void DisableEffects()
	{
		//muzzleFlash.enabled = false; // disable the light and muzzle flashes
		lightFlash[0].enabled = false;
		lightFlash[1].enabled = false;
		
	}
	
	
	/// <summary>
	/// Gets the ammo in pent.
	/// </summary>
	/// <returns>The ammo in pent.</returns>
	public int GetAmmoInPent()
	{
		return ammoInPent;
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="_ammo">Ammo.</param>
	public void SetAmmoInPent(int _ammo)
	{
		if (_ammo >= 0) {
			ammoInPent = _ammo;
		}
	}


	/// <summary>
	/// Gets the ammo in paint.
	/// </summary>
	/// <returns>The ammo in paint.</returns>
	public int GetCurrentAmmo()
	{
		return currentAmmo;
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="_ammo">Ammo.</param>
	public void SetCurrentAmmo(int _ammo)
	{

		if (_ammo >= 0) {
			currentAmmo = _ammo;
		}


	}

	
	
	// reload the gun
	IEnumerator reload()
	{

		if (isReloading)
		{
			yield break; // if already reloading... exit and wait till reload is finished
		}
		
		
        isReloading = true; // we are now reloading
	
		PlayReloadSound ();
		
		gunAnimator.SetTrigger ("OnReload");
		NetworkManager.instance.EmitAnimation ("OnReload");
	    yield return new WaitForSeconds(reloadTime); // wait for set reload time
		isReloading = false; // done reloading
		CanvasManager.instance.UpdatePanelGun(GetAmmoInPent ()); //updates the gun's HUD
			 
		

	
	}
	
	
	
	/// <summary>
	/// method for managing player animations
	/// </summary>
	/// <param name="_animation">Animation.</param>
	public void UpdateAnimator(string _animation)
	{

		if (gunAnimator != null) {
			
			switch (_animation) { 

			   case "OnIdle":
				//check if the player is already in the current animation
				if (!gunAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Idle") && !isReloading &&! m_Shoot) {
					
					gunAnimator.SetTrigger ("OnIdle");
					NetworkManager.instance.EmitAnimation ("OnIdle");
				

				  }
				break;

				case "OnRun":
				//check if the player is already in the current animation
				if (!gunAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Run") && !isReloading&& ! m_Shoot) {
					
					gunAnimator.SetTrigger ("OnRun");
					NetworkManager.instance.EmitAnimation("OnRun");
				

				  }
				break;

			    case "OnFire":
				//check if the player is already in the current animation
				 if (/*!gunAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Fire")&&*/!isReloading){
					
				    gunAnimator.SetTrigger ("OnFire");
					NetworkManager.instance.EmitAnimation("OnFire");
				
				 }
				break;

			    case "OnReload":
				    if (!gunAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Reload")&&!isReloading) {

					    StartCoroutine ("reload");  
					         
				    }
				break;

			     
			       }//END_SWITCH
		      }//END_IF

	}//END_UPDATE_ANIMATOR


	public void PlayDeathCam()
	{
	   deathCamera.gameObject.SetActive(true);
	   playerCamera.gameObject.SetActive(false);
	   GetComponent<FPSController> ().deathCamera.GetComponent<CameraFollow> ().SetTarget (gameObject.transform, cameraToTarget);

	}
	

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////// END HELPERS  /////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////// AUDIO METHODS ////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////// END AUDIO METHODS  ///////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}//END_OF_CLASS
}//END_OF_NAMESPACE