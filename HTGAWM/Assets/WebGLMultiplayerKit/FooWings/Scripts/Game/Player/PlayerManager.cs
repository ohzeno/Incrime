using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MultiplayerShooter
{
public class PlayerManager : MonoBehaviour {

    /*********************** DEFAULT VARIABLES ***************************/
	[Header("Default variables :")]
	public string	id;

	public string name;

	public bool isOnline;

	public bool isLocalPlayer;
	
	public GameObject target;
	
	public float rotateSpeed = 150f;

	public bool isDead;

    [Header("Local Player Text Color")]
	public Color localPlayerTextColour = new Color(1f, 0f, 0f, 0.1f);  // set in inspector.
    
	[Header("Remote Player Text Color")]
	public Color remotePlayerTextColour = new Color(1f, 0f, 0f, 0.1f);  // set in inspector.
   
   [Header("player GameObject")]
    public GameObject player;

    [Header("sphere game object")]
    public Rigidbody sphere;

    [Header("normal game object")]
	public Transform normal;

    [Header("model game object")]
	public Transform model;
   

	 [Header("Arrow")]
	public GameObject arrow;

	 public LayerMask layerMask;

	public float gravity = 10f;

	public float maxAcceleration = 30f;

    [HideInInspector]
	public float acceleration = 0f;

	float speed, currentSpeed;

	float rotate, currentRotate;
   
    float h,v;

	
   // Maximum turn rate in degrees per second.
   public float turningRate = 30f; 
 
   Vector3 targetPosition;
 
   public bool buster;
	
   public ParticleSystem[] busterParticles;
 
   float effectsDisplayTime = 0.2f;
 
   public float followSpeed = 1.0f;

   public GameObject[] wings; //helicopters available for choice
	

/****************************************************************************/


/****************************************************************************/
   public float minZ ;
   public float maxZ ;
   public float minY ;
   public float maxY ;
   public float minX ;
   public float maxX ;
   public float bank = 1;
   public float turnLR = 1;
   public float turnUD = 1;
   float angleZ;
   float angleY ;
   float angleX ;
   Quaternion originalRot;
 

  
	// Use this for initialization
	void Awake () {
	
	    originalRot = model.transform.rotation;
	
	}

	
	public void Set3DName(string name)
	{
		if(GetComponentInChildren<TMP_Text> ()!=null)
		{
		  if(isLocalPlayer)
		  {
			   GetComponentInChildren<TMP_Text> ().color = localPlayerTextColour;

			   GetComponentInChildren<TMP_Text> ().text = string.Empty;
		  }
		  else
		  {
			GetComponentInChildren<TMP_Text> ().color = remotePlayerTextColour;  

			GetComponentInChildren<TMP_Text> ().text = name;
		  }
		}
		

	}

   /*
	* configure the new player for the players who are already in the room
	*/
	public void SetUpPlayer(string _id, string _name, string _avatar,bool _isLocalPlayer)
	{
	
		id = _id;
		gameObject.name = _id;
		name = _name;
		isOnline = true;
		
		if(_isLocalPlayer)
	    {
		  isLocalPlayer = true; //this is local player
		  arrow.SetActive(false);
	    
	    }
		else
		{
		   isLocalPlayer = false;
		   MeshRenderer targetMeshRend = target.GetComponentInChildren<MeshRenderer> ();
		   targetMeshRend.enabled = false;

		}

		Set3DName(name);

		SetWings(int.Parse( _avatar));

		
	}


   
   // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {

	     //Follow Collider
         player.transform.position = Vector3.MoveTowards(player.transform.position,  sphere.transform.position, Time.deltaTime * followSpeed);
		
		  // Store the input axes.
         h = Input.GetAxisRaw("Horizontal");
              
		 v = Input.GetAxisRaw("Vertical");
		
		 if (Input.GetKeyUp (KeyCode.Space))
		 {
		   
		   ChangeBuster();
		   NetworkManager.instance.EmitChangeBuster();
		
		 }
		 if(buster)
		 {
		  acceleration = maxAcceleration;  
		 }
		 else
		 {
		  acceleration = maxAcceleration/2; 
		 }

		  speed = acceleration;
		 
	
		 /********************************/
		 //bank the spacecraft on x, y, z axis when moving
 
        float forceB = Input.GetAxis("Horizontal");
        float forceTLR = Input.GetAxis("Horizontal");
        float forceTUD = Input.GetAxis("Vertical");
   
        forceB = bank * forceB* Time.deltaTime;
        forceTLR = turnLR * forceTLR* Time.deltaTime;
        forceTUD = turnUD * forceTUD* Time.deltaTime;
   
        model.transform.Rotate (0, 0, -forceB);
        model.transform.Rotate (0, forceTLR, 0);
        model.transform.Rotate (-forceTUD, 0, 0);
	
	   if(h==0 && v==0)
	   {
	  
	      Turning();
	    
	   }
	
	  //restrict the degrees of banking
      angleZ = model.transform.rotation.z;
      ClampAngleZ();
      model.transform.rotation = new Quaternion (model.transform.rotation.x,model.transform.rotation.y,angleZ,model.transform.rotation.w);
	
      angleY = model.transform.rotation.y;
      ClampAngleY();
      model.transform.rotation = new Quaternion (model.transform.rotation.x,angleY,model.transform.rotation.z,model.transform.rotation.w);
   
      angleX = model.transform.rotation.x;
      ClampAngleX();
      model.transform.rotation = new Quaternion (angleX,model.transform.rotation.y,model.transform.rotation.z,model.transform.rotation.w);
	
		 
	  /******************************************************************************************************************************/

       var y = h * Time.deltaTime * rotateSpeed;
		 
	   var x = v * Time.deltaTime * rotateSpeed;
		 
	   float minRotation = -45;
       float maxRotation = 45;
		
       Vector3 currentRotation =  model.transform.localRotation.eulerAngles;
       currentRotation.x = ConvertToAngle180(currentRotation.x);
       currentRotation.x = Mathf.Clamp(currentRotation.x, minRotation, maxRotation);
       model.transform.localRotation = Quaternion.Euler(currentRotation);

       currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); 
        
    }//END_IF

 }
	
  
  

	
	void Turning()
	{

		Transform cameraTransform = Camera.main.transform;
			
	    Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 100.0f, Color.yellow);
			
	    targetPosition = cameraTransform.forward * 100.0f;   
		
		model.transform.rotation = Quaternion.RotateTowards(model.transform.rotation, Quaternion.LookRotation (targetPosition), Time.deltaTime * turningRate);
		  
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
        
		if (isLocalPlayer) {

            //manage player movement
		    Move();
			UpdateStatusToServer ();//send player's position and rotation to the server

		}
		
	
	}

	

	/// <summary>
	/// method for managing player movement
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	public void Move()
	{
       
		//Debug.Log("current speed: "+currentSpeed);
		//Forward Acceleration
        sphere.AddForce(model.transform.forward * currentSpeed, ForceMode.Acceleration);
		
        
        //Gravity
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        //Steering
        player.transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, 
        new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);

        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up*.1f), Vector3.down, out hitOn, 1.1f,layerMask);
        Physics.Raycast(transform.position + (transform.up * .1f)   , Vector3.down, out hitNear, 2.0f, layerMask);

        //Normal Rotation
        normal.up = Vector3.Lerp(normal.up, hitNear.normal, Time.deltaTime * 8.0f);
        normal.Rotate(0, transform.eulerAngles.y, 0);
	
										   
	}

     /// <summary>
    /// send player position and rotation to the server
   /// </summary>
	void UpdateStatusToServer ()
	{

		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data["local_player_id"] = id;

		data["position"] = player.transform.position.x+";"+player.transform.position.y+";"+player.transform.position.z;

		data["rotation"] = model.transform.rotation.x+";"+model.transform.rotation.y+";"+model.transform.rotation.z+";"+model.transform.rotation.w;

		NetworkManager.instance.EmitPosAndRot(data);



	}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////REMOTE PLAYERS METHODS ///////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	
  /**
	 * method that receives and handles the update of the position of the Network Player that arrived from the server
   */
	public void UpdatePosition(Vector3 _position) 
	{
	  
		if (!isLocalPlayer) 
		{
	      player.transform.position = _position;
		}
	}
	
   /**
	 * method that receives and handles the update of the rotation of the Network Player that arrived from the server
   */
	public void UpdateRotation(Quaternion _rotation) 
	{
		if (!isLocalPlayer) 
		{
			model.transform.rotation = _rotation;
		}

	}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  
	
	public void DisplayEffects()
	{
	     
	   StartCoroutine ("ShowEffects");  
	 
	}
	
	/// <summary>
	/// auxiliary coroutine for punch animation
	/// </summary>
	/// <returns>The punch animation.</returns>
	IEnumerator ShowEffects()
	{ 
		
		
		 
		GetComponentInChildren<WeaponEffects>().gunLight1.enabled = true;
		
		GetComponentInChildren<WeaponEffects>().gunLight2.enabled = true;

		GetComponentInChildren<WeaponEffects>().gunParticles1.Stop ();

	    GetComponentInChildren<WeaponEffects>().gunParticles1.Play ();
		
		GetComponentInChildren<WeaponEffects>().gunParticles2.Stop ();

	    GetComponentInChildren<WeaponEffects>().gunParticles2.Play ();
		
		
		yield return new WaitForSeconds(effectsDisplayTime); // wait for set reload time
		
		DisableEffects ();

	}
	
	public void DisableEffects ()
    {

	 GetComponentInChildren<WeaponEffects>().gunLight1.enabled = false;
	 GetComponentInChildren<WeaponEffects>().gunLight2.enabled = false;
	 GetComponentInChildren<WeaponEffects>().gunParticles1.Stop ();
	 GetComponentInChildren<WeaponEffects>().gunParticles2.Stop ();
   }
   
  
	
	
	
	
	/// <summary>
	/// method for managing player animations
	/// </summary>
	/// <param name="_animation">Animation.</param>
	public void ChangeBuster()
	{
	  buster =!buster;
	  if(buster)
	  {
	    foreach(ParticleSystem busterParticle in busterParticles)
		      {
		       
				 // Setting the particle to enabled
                var emission = busterParticle.emission;
                emission.enabled = true;
			
		      }
	  }
	  else
	  {
	   foreach(ParticleSystem busterParticle in busterParticles)
		      {
		         
				 // Setting the particle to enabled
                var emission = busterParticle.emission;
                emission.enabled = false;
				
		      }
	  }
	   
	}
	
	
	

		
	/// <summary>
	///   helper method to set player character
	/// </summary>
	/// <param name="index">current character index</param>
    /**
	 * method to defines the ship to be displayed according to the player's choice
	 *
     */
	void SetWings(int index)
	{
	 
	  for(int i =0;i< wings.Length;i++)
	  {
	    if(i.Equals(index))
		{
		   wings[index].SetActive(true);  //displays the helicopter chosen by the player
		}
		else
		{
		   wings[i].SetActive(false);  //hides the other helicopters
		}
	  }
	}


	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////// HELPERS METHODS ///////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	
	public float ConvertToAngle180(float input)
     {       
         while (input > 360)
         {
             input = input - 360;
         } 
         while (input < -360)
         {
             input = input + 360;
         }
         if (input > 180)
         {
             input = input - 360;        
         }
         if (input < -180)
             input = 360+ input;
         return input;
     }

	
//the functions for restricting the degrees of banking (see above)
void ClampAngleZ () {
   
   if (angleZ < -360) {
      angleZ += 360;
   }
   
   else if (angleZ > 360) {
      angleZ -= 360;
   }    
 
   angleZ = Mathf.Clamp (angleZ, minZ, maxZ);
}
 
void ClampAngleY () {
   
   if (angleY < -360) {
      angleY += 360;
   }
   
   else if (angleY > 360) {
      angleY -= 360;
   }    
 
   angleY = Mathf.Clamp (angleY, minY, maxY);
}
 
void ClampAngleX () {
   
   if (angleX < -360) {
      angleX += 360;
   }
   
   else if (angleX > 360) {
      angleX -= 360;
   }    
 
   angleX = Mathf.Clamp (angleX, minX, maxX);
}
	
	
	
}//END_CLASS
}//END_NAMESPACE