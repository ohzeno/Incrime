using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SavanaIO
{
public class Player2DManager : MonoBehaviour
{
      /*********************** DEFAULT VARIABLES ***************************/
	[Header("ID")]
	[Header("Default variables :")]
    public string	id;

    [Header("Name")]
	public string name;

    [Header("Avatar")]
	public string avatar;

	public bool isOnline;

	public bool isLocalPlayer;

	[Header("player's avatars")]
	public GameObject[] avatars;

    [Header("Particles Prefab")]
	public GameObject particlePref;

    [Header("Evolution Audio clip")]
	public AudioClip evolutionAudioClip;

	 [Header("Regression Audio clip")]
	public AudioClip regressionAudioClip;
    
	[Header("Speed")]
	public float speed;
    
	[Header("Speed")]
	public float maxSpeed;

	float minSpeed;
    
	[Header("Rotation Offset")]
	public float rotationOffset = 270;

    [HideInInspector]
	public Rigidbody2D myRigidbody;
   
	float h ;

	float v;

	/****************************************************************************/


    /****** SYNCHRONIZATION VARIABLES (only for Network Players) *********/
	[HideInInspector]
	public float lastVelocityX =0f;
	
	[HideInInspector]
	public float lastVelocityY = 0f;

	[HideInInspector]
	public float lastSynchronizationTime;
	
	[HideInInspector]
	public float syncDelay;
	
	[HideInInspector]
	public float syncTime;
	
	[HideInInspector]
	Vector3 syncStartPosition = Vector3.zero;
	
	[HideInInspector]
	Vector3 syncEndPosition = Vector3.zero;

    [HideInInspector]
	public bool onReceivedPos;

	/****************************************************************************/


	
	// Use this for initialization
	void Start () {

		myRigidbody = GetComponent<Rigidbody2D> ();
		minSpeed = speed;
	
	}
	

	
	// Update is called once per frame
	void FixedUpdate () {

		
		if(isLocalPlayer)
		{
		  Move ();

		  UpdateStatusToServer ();
		
	   }
		else
		{ // IF IS REMOTE PLAYER
		
		  SyncedMovement();
		  
		  if (myRigidbody.velocity.x != lastVelocityX) {
			
			lastVelocityX = myRigidbody.velocity.x;
				
		  }
			
		
		}
		
		
		
	}
	
	/// <summary>
   /// method to synchronize the position received from the server
   /// only for Network Players
   /// </summary>
	void SyncedMovement()
	{
	  syncTime +=Time.deltaTime;
	  if(onReceivedPos)
	  {
	    transform.position = new Vector3(Vector3.Lerp(syncStartPosition,syncEndPosition,syncTime/syncDelay).x,
		              Vector3.Lerp(syncStartPosition,syncEndPosition,syncTime/syncDelay).y,transform.position.z);
	  }
	}

    /// <summary>
    /// method to handle with player's evolution
    /// </summary>
    /// <param name="_avatar"></param>
	public void Evolution(int _avatar)
	{

	   if(!avatar.Equals(_avatar.ToString()))
	   {
		    PlayAudio( evolutionAudioClip);
	   }

       avatar = _avatar.ToString();


	   GameObject particle = GameObject.Instantiate (particlePref,transform.position,
			  Quaternion.identity);

	   particle.transform.parent = transform;

	   GetComponent<SpriteRenderer> ().sprite = avatars[_avatar].GetComponent<SpriteRenderer> ().sprite;  // change the sprite

		if (isLocalPlayer)
		{
			CanvasManager.instance.xpSlider.value = 0;
			CanvasManager.instance.onTimer = true;
			CanvasManager.instance.progress = 100;

			if(_avatar.Equals(1)) // is lion
			{
               CanvasManager.instance.timerTime = 0.3f;
			   CanvasManager.instance.StartTimer();
			}
			else if(_avatar.Equals(2)) // is croccodile
			{
                CanvasManager.instance.timerTime = 0.2f;
				CanvasManager.instance.StartTimer();
			}	
			    
		}
	}


    /// <summary>
    ///  method to handle with player's regression
    /// </summary>
    /// <param name="_avatar"></param>
	public void Regression(int _avatar)
	{

	   PlayAudio( regressionAudioClip);

       avatar = _avatar.ToString();

	   GameObject particle = GameObject.Instantiate (particlePref,transform.position,
			  Quaternion.identity);

	   particle.transform.parent = transform;
	  
	   GetComponent<SpriteRenderer> ().sprite = avatars[_avatar].GetComponent<SpriteRenderer> ().sprite; 

		if (isLocalPlayer)
		{
			CanvasManager.instance.xpSlider.value = 0;

			if(_avatar>0)// player is not antilope
			{
			  CanvasManager.instance.onTimer = true;
			  CanvasManager.instance.progress = 100;
			  if(_avatar.Equals(1))// lion
			  {
                CanvasManager.instance.timerTime = 0.3f;
				CanvasManager.instance.StartTimer();
			   }
			   else if(_avatar.Equals(2))// crocodille
			   {
                 CanvasManager.instance.timerTime = 0.2f;
				 CanvasManager.instance.StartTimer();
			   }		
			}
		}
	}
	


    /// <summary>
    /// method to move local player
    /// </summary>	
 	void Move()
	{

		if(Input.GetButtonDown ("Fire1"))
		{
          speed = maxSpeed;
		}
	  
	    Vector3 mousePos = Input.mousePosition;
		mousePos.z = 0;
		Vector3 objPos = Camera.main.WorldToScreenPoint(transform.position);

		mousePos.x = mousePos.x - objPos.x;

		mousePos.y = mousePos.y - objPos.y;

		float angle = Mathf.Atan2(mousePos.y,mousePos.x)*Mathf.Rad2Deg;

		transform.rotation = Quaternion.Euler(new Vector3(0,0,angle+rotationOffset));

		Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		targetPos.z = 0;
		transform.position = Vector3.MoveTowards(transform.position,targetPos,speed * Time.deltaTime);

		speed = minSpeed;
	
		
	}//END_MOVE
	
	/// <summary>
	/// send player's position and rotation to the server
	/// </summary>
	void UpdateStatusToServer ()
	{
			
		
		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data["local_player_id"] = id;
		
		data["position"] = transform.position.x+":"+transform.position.y;

		data["rotation"] = transform.rotation.x+":"+transform.rotation.y+":"+transform.rotation.z+":"+
		                                                                                    transform.rotation.w;

		NetworkManager.instance.EmitPosAndRot(data);
	}
	
	
	
    /// <summary>
    /// update remote player's position
    /// </summary>
    /// <param name="_pos"></param>
	public void UpdatePosition(Vector2 _pos) 
	{
	  
	  syncEndPosition = new Vector3(_pos.x,_pos.y,transform.position.z);
	  
	  syncStartPosition = transform.position;
	  
	  syncTime = 0f;
	  
	  syncDelay = Time.time - lastSynchronizationTime;
	  
	  lastSynchronizationTime = Time.time;
	  
	  onReceivedPos = true;
	
	  // transform.position = new Vector3 (position.x, position.y, position.z);
	}
	

    /// <summary>
    /// update remote player's rotation
    /// </summary>
    /// <param name="_rotation"></param>
	public void UpdateRotation(Quaternion _rotation)
	{
		if (!isLocalPlayer)
		{
			transform.rotation = _rotation;

		}

	}

    /// <summary>
    /// detect collision with others players
    /// </summary>
    /// <param name="colisor"></param>
	void OnCollisionEnter2D(Collision2D colisor)
	{

	   if (colisor.gameObject.tag.Equals("Player")&& isLocalPlayer)
		{
			NetworkManager.instance.EmitPlayerDamage(colisor.gameObject.GetComponent<Player2DManager>().id);
			
		}
	}

	public void PlayAudio(AudioClip _audioclip)
	{
		
	   GetComponent<AudioSource> ().PlayOneShot (_audioclip);

	}

		
}
}