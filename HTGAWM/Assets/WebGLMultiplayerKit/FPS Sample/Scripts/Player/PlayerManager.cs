using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///Manage Network player if isLocalPlayer variable is false
/// or Local player if isLocalPlayer variable is true.
/// </summary>
namespace FPSExample{
public class PlayerManager : MonoBehaviour {

    /*********************** DEFAULT VARIABLES ***************************/
	[Header("Default variables :")]
	public string	id;

	public string name;

	public bool isOnline;

	public bool isLocalPlayer;

	Animator myAnim;

	public float effectsDisplayTime = 0.2f;

	CharacterController characterController;


    /****************************************************************************/

  /*********************** JUMP CONTROL VARIABLES ***************************/
    [Header("Jump control variables :")]
	Rigidbody myRigidbody;

	public bool onGrounded;

	[SerializeField] float m_GroundCheckDistance = 1f;

	public float jumpPower = 12f;

	public float jumpTime=0.4f;

	public float jumpdelay=0.4f;

	public bool m_jump;

	public bool isJumping;

	public bool OnCorroutine;
	
	public float h,v;
	
	public Transform gunTransform;
	
	public GameObject bulletPref; // bullet prefab
	
	/****************************************************************************/

	
	/****** SYNCHRONIZATION VARIABLES (only for Network Players) ***/
	[HideInInspector]
	public float lastSynchronizationTime;

	[HideInInspector]
	public float syncDelay;

	[HideInInspector]
	public float syncTime;
	
	 Vector3 latestPos  = Vector3.zero;

	[HideInInspector]
	Vector3 syncStartPosition = Vector3.zero;

	[HideInInspector]
	Vector3 syncEndPosition = Vector3.zero;
	
	//private Queue<Vector3> positionsQueue;

    [HideInInspector]
	public bool onReceivedPos;
	
	//public float minDistanceToPlayer = 0.4f ;

	Vector3 pos;

	public Vector3 offset;

	[HideInInspector]
	public float lastVelocityX =0f;

	[HideInInspector]
	public float lastVelocityZ = 0f;
	
	bool move;
	
	bool onIdle = true;
	
	/*************************************************************/

	/*********************** AUDIO VARIABLES ***************************/
	[Header("AUDIO VARIABLES")]
	public  AudioClip shootSound;
	
 /*********************** END AUDIO EFFECTS VARIABLES ***************************/

 
	public Transform[] muzzlePoint;

	public Renderer muzzleFlash = null;     // the muzzle flash for this weapon

	public Light[] lightFlash = null;         // the light flash for this weapon

	public GameObject fireParticlePref;

	public float reloadTime,damageAnimTime;
	bool isReloading;
 

  
	// Use this for initialization
	void Awake () {

	 if(!isLocalPlayer)
	 {
	  if ( GetComponentInChildren<Animator> () != null) {
			
			myAnim = GetComponentInChildren<Animator> ();
			
			myRigidbody = GetComponent<Rigidbody> ();
		  
		}
	    
	 }
	 else
	 {
		characterController = GetComponent<CharacterController>();
	 }
		
		
	}

	public void Set3DName(string name)
	{
		GetComponentInChildren<TextMesh> ().text = name;

	}
	
	void Update()
	{
	  if (!isLocalPlayer) {

	   transform.position = new Vector3(transform.position.x,latestPos.y,transform.position.z);
	   SyncedMovement();
	   }
	}

	// Update is called once per frame
	void FixedUpdate () {
	

		if (isLocalPlayer) {

            //send player movement to server
	 	    EmitMove ();

		}
		else
		{
		    //Jump();
		
			if (myRigidbody.velocity.x != lastVelocityX) {
				lastVelocityX = myRigidbody.velocity.x;
				
			}
			if (myRigidbody.velocity.z != lastVelocityZ) {
				lastVelocityZ = myRigidbody.velocity.z;
				
			}
			
		}

	
	}

	

	void EmitMove()
	{
	
		// read inputs
		move = false;
		onIdle = true;
		v = 0;
		h = 0;

		v = Input.GetAxisRaw("Vertical");
		h = Input.GetAxisRaw("Horizontal");
		
		if(v!=0|| h!=0)
		{
		 move = true;
		}
		
		//up button or joystick
		if (Input.GetKey("up"))
		{
		  v = 1;
		  move = true;
		}
		if (Input.GetKey("down"))//down button or joystick
		{
			v=-1;
			move = true;
			
		}
		//up button or joystick
		if (Input.GetKey("left"))
		{
		  h =-1;
		  move = true;
		 
		}
		if (Input.GetKey("right"))//down button or joystick
		{
			h=1;
			move = true;
			
		}

		if (Input.GetButtonDown("Jump") && characterController.isGrounded)
        {
			NetworkManager.instance.EmitAnimation ("OnJump");
        }
			
		if(move)
		{
			GetComponent<FootStepSound> ().isMoving = true;
			//NetworkManager.instance.EmitAnimation("OnRun");
			onIdle = false;
		
			
		}
		if(onIdle)
		{
		   // NetworkManager.instance.EmitAnimation ("OnIdle");
		}
		
		UpdateStatusToServer ();

		
	}

	void UpdateStatusToServer ()
	{
	   if (move ||Input.GetAxisRaw("Mouse X")!=0) {
			
		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data["local_player_id"] = id;

		data["position"] = transform.position.x+";"+transform.position.y+";"+transform.position.z;
		 float rotY  = transform.eulerAngles.y;
	
		data["rotation"] = rotY.ToString();
		
		NetworkManager.instance.EmitPosAndRot(data);
	
		}
		

	}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////NETWORK PLAYERS METHODS ///////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  
  /**
	 * method to synchronize the position received from the server
	 * only for Network Players
   */
	void SyncedMovement()
	{
	  syncTime +=Time.deltaTime;
	  
	  if(onReceivedPos)
	  {
	    transform.position = Vector3.Lerp(syncStartPosition,syncEndPosition,syncTime/syncDelay);
		
	  }
	}
	
    // <summary>
	/// method that receives and handles the update of the position of the Network Player that arrived from the server.
	/// only for Network Players.
	/// </summary>
	public void UpdatePosition(Vector3 position) 
	{
	  //transform.position = new Vector3 (position.x, position.y+offset.y, position.z);
	 
	  	if(!position.Equals(latestPos))
			{
			  GetComponent<FootStepSound> ().isMoving = true;
			}

            latestPos = position;
			
			latestPos =  new Vector3 (latestPos.x, latestPos.y+offset.y, latestPos.z);

			 syncEndPosition = latestPos;
	  
	         syncStartPosition = transform.position;
	  
	         syncTime = 0f;
	  
	        syncDelay = Time.time - lastSynchronizationTime;
	  
	        lastSynchronizationTime = Time.time;
	  
	        onReceivedPos = true;
		

	}
	

	public void UpdateRotation(float _rotY) 
	{
	    transform.eulerAngles =  new Vector3 (transform.eulerAngles.x, _rotY, transform.eulerAngles.z);	
	}

	public void UpdateJump()
	{
		m_jump = true;
	}

	public void Jump()
	{
	   if (!isLocalPlayer) 
		{
			
		RaycastHit hitInfo;

		onGrounded = Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance);

		jumpTime -= Time.deltaTime;

		

		// if the jump time has already occurred and the player is hitting the ground and he was jumping
		if (jumpTime <= 0 && isJumping && onGrounded)
		{

			m_jump = false;
			//mark that the player is not jumping
			isJumping = false;
			UpdateAnimator("OnIdle");
		}


		//check if the user pressed space and he is no longer jumping or if the player is on the canvas and is no longer jumping
		if (m_jump && !isJumping) 
		{	

			//jump effect
			myRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
			//calculates the jump time
			jumpTime = jumpdelay;
			//mark that the player is jumping
			isJumping = true;
			
		}
	}

	}

	public void SpawnNetworkBullet(Vector3 _target)
	{
	  GameObject bullet = GameObject.Instantiate (bulletPref, gunTransform.position, Quaternion.identity) as GameObject;
			 
	  bullet.GetComponent<Bullet3DController> ().isLocalBullet = false;
			 
	  bullet.GetComponent<Bullet3DController> ().shooterID = id; 
			
	  bullet.GetComponent<Bullet3DController> ().Shoot (_target,gameObject.name);

	  StartCoroutine ("ShowEffects");  
	  
	 
		
	}

	/// <summary>
	/// auxiliary coroutine for punch animation
	/// </summary>
	/// <returns>The punch animation.</returns>
	IEnumerator ShowEffects()
	{ 
		
		PlayShootSound();
		 
		lightFlash[0].enabled = true;
		lightFlash[1].enabled = true;
		GameObject newFireParticle = Instantiate (fireParticlePref, muzzlePoint[0].position, fireParticlePref.transform.rotation) as GameObject;
		GameObject newFireParticle2 = Instantiate (fireParticlePref, muzzlePoint[1].position, fireParticlePref.transform.rotation) as GameObject;
		
		
		yield return new WaitForSeconds(effectsDisplayTime); // wait for set reload time
		
		DisableEffects ();

	}
	
	public void DisableEffects ()
    {

	 	lightFlash[0].enabled = false;
		lightFlash[1].enabled = false;
   }

   // reload the gun
	IEnumerator DamageAnim()
	{

		myAnim.SetTrigger ("OnDamage");
	    yield return new WaitForSeconds(damageAnimTime); // wait for set reload time
		myAnim.SetTrigger ("OnIdle");
	

	
	}

	// reload the gun
	IEnumerator reload()
	{

		if (isReloading)
		{
			yield break; // if already reloading... exit and wait till reload is finished
		}
		
		
        isReloading = true; // we are now reloading
	
	
		myAnim.SetTrigger ("OnReload");

	    yield return new WaitForSeconds(reloadTime); // wait for set reload time
		isReloading = false; // done reloading
		myAnim.SetTrigger ("OnIdle");
			 
		

	
	}

		
	


	
	
	/// <summary>
	/// method for managing player animations
	/// </summary>
	/// <param name="_animation">Animation.</param>
	public void UpdateAnimator(string _animation)
	{

		if (myAnim != null) {
			
			switch (_animation) { 

			   case "OnIdle":
				//check if the player is already in the current animation
				if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Idle") && !isReloading) {
					
					myAnim.SetTrigger ("OnIdle");
					
				  }
				break;

			    case "OnRun":
				//check if the player is already in the current animation
				 if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Run")&& !isReloading) {
			
					myAnim.SetTrigger ("OnRun");

				 }
				break;

			     
			     case "OnJump":
				  //check if the player is already in the current animation
				  if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Jump")) {
					
					m_jump = true;
					myAnim.SetTrigger ("OnJump");

				  }
				  break;
			  
			    
			        case "OnDamage":
				      if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Damage")) {


						    StartCoroutine ("DamageAnim");  
					
					       

				       }
				     break;
					 
					  case "OnFire":
				      if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Fire")&& !isReloading) {
					
					        myAnim.SetTrigger ("OnFire");

				       }
				     break;
					 
					  case "OnReload":
				      if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Reload")) {
					
					          StartCoroutine ("reload");  

				       }
				     break;


			       }//END_SWITCH
		      }//END_IF

	}//END_UPDATE_ANIMATOR

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////// AUDIO METHODS ///////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	

	public void PlayShootSound()
	{
		GetComponent<AudioSource>().PlayOneShot(shootSound);
	}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////// END AUDIO METHODS  ///////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
}
}
