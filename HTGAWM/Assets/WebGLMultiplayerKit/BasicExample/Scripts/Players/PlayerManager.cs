using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///Manage Network player if isLocalPlayer variable is false
/// or Local player if isLocalPlayer variable is true.
/// </summary>
public class PlayerManager : MonoBehaviour {

	public string	id;

	public string name;

	public string avatar;

	public bool isOnline;

	public bool isLocalPlayer;

	Animator myAnim;

	public Rigidbody myRigidbody;

	public GameObject model;

	public Transform cameraToTarget;

	public enum state : int {idle,walk,attack,damage,dead};

	public state currentState;

	//distances low to arrive close to the player
	[Range(1f, 200f)][SerializeField] float minDistanceToPlayer = 10f ;

	public float verticalSpeed = 3.0f;

	public float rotateSpeed = 150f;

	float m_GroundCheckDistance = 1f;

	public float jumpPower = 12f;

	public float jumpTime=0.4f;

	public float jumpdelay=0.4f;

	public bool m_jump;

	public bool isJumping;

	public bool onGrounded;

	public bool isAttack;
   
	public float attackTime;

	public float timeOut;

	float h ;
	
	float v;
	
	bool attack;

	public bool onMobileButton;
	
	public float lastVelocityX = 0f;
	
	[Header("Attack Audio Clip")]
    public AudioClip attackAudioClip;
	
	[Header("Jump Audio Clip")]
    public AudioClip jumpAudioClip;

	// Use this for initialization
	void Awake () {

		myAnim = GetComponent<Animator>();
		myRigidbody = GetComponent<Rigidbody> ();
		lastVelocityX = myRigidbody.velocity.x;
		
	}

	public void Set3DName(string name)
	{
		GetComponentInChildren<TextMesh> ().text = name;

	}



	// Update is called once per frame
	void FixedUpdate () {
		

		if (isLocalPlayer)
		{
			Attack ();
			Move();
		}
		else
		{
			if (myRigidbody.velocity.x != lastVelocityX) {
				lastVelocityX = myRigidbody.velocity.x;
				UpdateAnimator("IsIdle");
			}
			else
			{
				UpdateIdle ();
			}
		}
	
		Jump ();



	}

	void Attack()
	{
	   

			if (isAttack || Input.GetKey (KeyCode.LeftControl))
			{
			
				currentState = state.attack;
				UpdateAnimator ("IsAttack");
				PlayAttackSound();
				NetworkManager.instance.EmitAttack();

				foreach(KeyValuePair<string, PlayerManager> enemy in NetworkManager.instance.networkPlayers)
				{

					if ( !enemy.Key.Equals(id))
					{
					
						Vector3 meToEnemy = transform.position - enemy.Value.transform.position;
						
					    //if i am close to any network player
						if (meToEnemy.sqrMagnitude < minDistanceToPlayer)
						{

						    //damage network player
							NetworkManager.instance.EmitPhisicstDamage (enemy.Key);
						}
					}
				}
			}

		
	}
	
	
	void Move( )
	{

        if(!onMobileButton)
		{
		    // Store the input axes.
            h = Input.GetAxisRaw("Horizontal");
              
		    v = Input.GetAxisRaw("Vertical");


		}
		

		var x = h* Time.deltaTime *  verticalSpeed;
		var y = h * Time.deltaTime * rotateSpeed;
		var z = v * Time.deltaTime * verticalSpeed;



		transform.Rotate (0, y, 0);

		transform.Translate (0, 0, z);

		if (h != 0 || v != 0 || isJumping ) {
		    currentState = state.walk;
			UpdateAnimator ("IsWalk");
			UpdateStatusToServer ();
		}
		else
		{
			if (currentState != state.idle)
			{
			    
				NetworkManager.instance.EmitAnimation ("IsIdle");
			}
			currentState = state.idle;
			UpdateAnimator ("IsIdle");
		}
	
	

	}



	void UpdateStatusToServer ()
	{


		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data["local_player_id"] = id;

		data["position"] = transform.position.x+":"+transform.position.y+":"+transform.position.z;

		data["rotation"] = transform.rotation.y.ToString();

		NetworkManager.instance.EmitMoveAndRotate(data);



	}


	public void UpdateIdle()
	{

		UpdateAnimator ("IsIdle");

	}

	public void UpdatePosition(Vector3 position)
	{
		if (!isLocalPlayer) {

			if (!isJumping)
			{
				UpdateAnimator ("IsWalk");
				transform.position = new Vector3 (position.x, position.y, position.z);
			}
		}

	}

	public void UpdateRotation(Quaternion _rotation)
	{
		if (!isLocalPlayer)
		{
			transform.rotation = _rotation;

		}

	}


public void UpdateAnimator(string _animation)
	{

	
			switch (_animation) { 
			case "IsWalk":
				if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Walk"))
				{
					myAnim.SetTrigger ("IsWalk");
				}
				break;

			case "IsIdle":

				if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
				{
					myAnim.SetTrigger ("IsIdle");
				}
				break;

			case "IsDamage":
				if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Damage") ) 
				{
					myAnim.SetTrigger ("IsDamage");
				}
				break;

			case "IsAttack":
				if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
				{  
				
					myAnim.SetTrigger ("IsAttack");
	
				    if (!isLocalPlayer)
				    {
			
					   StartCoroutine ("StopAttack");
				    }
				}
				break;


			case "IsDead":
				if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
				{
					myAnim.SetTrigger ("IsDead");
				}
				break;

			}
	
	}
	
	// reload your weapon
	IEnumerator StopAttack()
	{
		if (isAttack)
		{
			yield break; // if already attack... exit and wait attack is finished
		}

		isAttack = true; // we are now attack

	
		yield return new WaitForSeconds(attackTime); // wait for set attack animation time
		isAttack = false;


	}


	public void UpdateJump()
	{
		m_jump = true;
	}

	public void Jump()
	{
		RaycastHit hitInfo;

		onGrounded = Physics.Raycast(transform.position + (Vector3.up * 0.1f),
		 Vector3.down, out hitInfo, m_GroundCheckDistance);

		jumpTime -= Time.deltaTime;

		if (isLocalPlayer)
		{
			m_jump = Input.GetKey("space");
		}

		
		if (jumpTime <= 0 && isJumping && onGrounded)
		{

			m_jump = false;
			isJumping = false;
		}
		
		if (m_jump && !isJumping)
		{
			myRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
			
			PlayJumpSound();
			
			jumpTime = jumpdelay;
			
			isJumping = true;
		}

	}
	
	 public void EnableKey(string _key)
	 {
	 
	   onMobileButton = true;
	   switch(_key)
	   {
	   
	     case "up":
		 v = 1;
		 break;
		 case "down":
		 v= -1;
		 break;
		 case "right":
		 h = 1;
		 break;
		 case "left":
		 h = -1;
		 break;
		 case "attack":
		 isAttack  = true;
		 break;
	   }
	 }
	 
	 public void DisableKey(string _key)
	 {
	   onMobileButton = false;
	   switch(_key)
	   {
	    case "up":
		 v = 0;
		 break;
		 case "down":
		 v= 0;
		 break;
		 case "right":
		 h = 0;
		 break;
		 case "left":
		 h = 0;
		 break;
		 case "attack":
		 isAttack  = false;
		 break;
	   }
	 }
	 
	 public void PlayAttackSound()
	{

	   if (!GetComponent<AudioSource> ().isPlaying )
		{
		
		  GetComponent<AudioSource>().PlayOneShot(attackAudioClip);

		}


	}
	 public void PlayJumpSound()
	{

	   if (!GetComponent<AudioSource> ().isPlaying )
		{
		
		  GetComponent<AudioSource>().PlayOneShot(jumpAudioClip);

		}


	}
}
