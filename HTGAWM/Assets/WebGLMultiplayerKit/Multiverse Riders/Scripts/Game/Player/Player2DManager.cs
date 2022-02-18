using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MultiverseRiders
{
public class Player2DManager : MonoBehaviour
{

    public string	id;

	public string name;

	public int cont;

	public string avatar;

	public bool isOnline;

	public bool isLocalPlayer;
	
	Animator anim;
	public enum statebehaviour : int {IDLE,JUMP,RUN,PUNCH,SHOOT,DAMAGE,DEAD};
	public statebehaviour currentStateBehaviour;
	public float horizontalSpeed;
	public float verticalSpeed;
	BoxCollider2D myBoxCollider2d;
	public Rigidbody2D myRigidbody;
	public bool isMove;
	public bool stop;
	public Transform footer;
	public float MaxDistanceToGo = .1f;
	public float MaxDistance = .1f;
	bool m_jump;
	public float jumpForce;
	public float jumpTime = 0.4f;
	public float jumpDelay = 0.4f;
	public bool jumped = false;
	public bool isGrounded;
	
	public float minDistanceToPlayer = 1f;
	
	public float lastVelocityX =0f;
	
	public float lastVelocityY = 0f;
	
	public Transform foot;
	
	Animator myAnim;
	
	public float punchAnimTime;
	public float shootAnimTime;
	public float jumpAnimTime;
	public bool OnCorroutine;
	
	public bool isMoving;
	
	float h ;
	float v;
	
	public bool onMobileButton;
 	
	public float lastSynchronizationTime;
	
	public float syncDelay;
	
	public float syncTime;
	
	Vector3 syncStartPosition = Vector3.zero;
	
	Vector3 syncEndPosition = Vector3.zero;


	public bool onReceivedPos;
	
	bool onAttack;

	
	// Use this for initialization
	void Start () {

		horizontalSpeed= 4f;
		verticalSpeed= 4f;
		anim = GetComponent<Animator> ();
		myBoxCollider2d = GetComponent<BoxCollider2D> ();
		myRigidbody = GetComponent<Rigidbody2D> ();
		myAnim = GetComponent<Animator>();
		
		//syncStartPosition = transform.position;
		//syncEndPosition = syncStartPosition;
	}
	

	
	// Update is called once per frame
	void FixedUpdate () {

		
		if(!onMobileButton)
		{
		   // Store the input axes.
           h = Input.GetAxisRaw("Horizontal");
              
		   v = Input.GetAxisRaw("Vertical");
		}
		  
		
		if(isLocalPlayer)
		{
		 Move ();
		
		 Attack();
		 
		 if (IsMoving() || jumped) {
		   
		   UpdateStatusToServer ();
		}
		else
		{
		  
		  
		  if (currentStateBehaviour != statebehaviour.IDLE)
		  {
			ShooterNetworkManager.instance.EmitAnimation ("OnIdle");
		  }
			
		}
	   }
		else
		{
		
		  SyncedMovement();
		  
			if (myRigidbody.velocity.x != lastVelocityX) {
				lastVelocityX = myRigidbody.velocity.x;
				
			}
			
			if(myRigidbody.velocity.x == lastVelocityX && myRigidbody.velocity.y == lastVelocityY &&!jumped)
			{
			   UpdateAnimator ("OnIdle");
			}
		}
		
		
		
	}
	
	void SyncedMovement()
	{
	  syncTime +=Time.deltaTime;
	  if(onReceivedPos)
	  {
	    transform.position = new Vector3(Vector3.Lerp(syncStartPosition,syncEndPosition,syncTime/syncDelay).x,Vector3.Lerp(syncStartPosition,syncEndPosition,syncTime/syncDelay).y,transform.position.z);
	  }
	}
	
	void Attack()
	{
	
		
	  if(Input.GetKey (KeyCode.E) || onAttack)
	  {
	   UpdateAnimator("OnShoot");
	   ShooterNetworkManager.instance.EmitAnimation("OnShoot");
	   
	  
	  }
	  
	 
	}



	void Move()
	{
	  
		stop = false;
		isMove = false;

        Vector3 lastPosition2 = transform.position;

		if(h==0)
		{
		  UpdateAnimator("OnIdle");
		}
		if (h > 0 ) 
		{
		
			Vector3 lastPosition = transform.position;
			transform.Translate(Vector2.right *horizontalSpeed * Time.deltaTime);
			transform.eulerAngles = new Vector2(0,0);
			isMove = true;
			
			UpdateAnimator("OnRun");

		}

		if (h < 0 ) 
		{
			
            Vector3 lastPosition = transform.position;
			transform.Translate(Vector2.right *horizontalSpeed * Time.deltaTime);
			transform.eulerAngles = new Vector2(0,180);
			isMove = true;
			UpdateAnimator("OnRun");
		}

		if (v > 0 ) 
		{
		
			Vector3 lastPosition = transform.position;
		
			transform.Translate(Vector2.up *horizontalSpeed * Time.deltaTime);

			
			isMove = true;
			
			UpdateAnimator("OnRun");

		}

		if (v < 0 ) 
		{
			
            Vector3 lastPosition = transform.position;
			transform.Translate(Vector2.up *-horizontalSpeed * Time.deltaTime);

		
			isMove = true;
			UpdateAnimator("OnRun");
		}

	
		
	
		
	}//END_MOVE
	
	void UpdateStatusToServer ()
	{
			
		
		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data["local_player_id"] = id;

		data["dx"] = transform.position.x.ToString();

		data["dy"] = transform.position.y.ToString();

		if (h > 0 ) 
		{
		  data["rotation"] = "r";//right

		}
		else if(h< 0)
		{
		 data["rotation"] = "l";//left
		}
		
	

		ShooterNetworkManager.instance.EmitPosAndRot(data);//call method NetworkSocketIO.EmitPosition for transmit new  player position to all clients in game


		if(IsMoving()&&!jumped)
		{
		 ShooterNetworkManager.instance.EmitAnimation("OnRun");
		}
		

	}
	
	
	bool IsMoving()
	{
	
	  
	  
	  if(h!=0 || v!=0)
	  {
		 return true;
	  }
	  else
	  {
		 return false;
	  }
	 
	
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
				
				if(!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Idle")&& !OnCorroutine)
				{
				   myAnim.SetTrigger ("OnIdle");
				   
				   currentStateBehaviour = statebehaviour.IDLE;
				}
			
				break;

			    case "OnRun":
				//check if the player is already in the current animation
				 if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Run") && !OnCorroutine) {
					
					myAnim.SetTrigger ("OnRun");

					currentStateBehaviour = statebehaviour.RUN;

				 }
				break;

			    case "OnShoot":
				//check if the player is already in the current animation
				if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Shoot")&& !OnCorroutine) {
					
					 StartCoroutine ("RunShootAnim");  
					
				  }
				 break;

			     case "OnJump":
				  //check if the player is already in the current animation
				  if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Jump")&& !OnCorroutine) {
				
					  StartCoroutine ("RunJumpAnim");  
					

				  }
				 break;
	
			     case 	"OnDamage":
				   if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Damage")&& !OnCorroutine) {
					
					     myAnim.SetTrigger ("OnDamage");

					     currentStateBehaviour = statebehaviour.DAMAGE;
				   }
				  break;

			      case"IsDead":
				    if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Dead")) {
					
					   myAnim.SetTrigger ("IsDead");

					   currentStateBehaviour = statebehaviour.DEAD;
				    }
				   break;

			       }//END_SWITCH
		      }//END_IF

	}//END_UPDATE_ANIMATOR
	
	

	/// <summary>
	/// auxiliary coroutine for punch animation
	/// </summary>
	/// <returns>The punch animation.</returns>
	IEnumerator RunJumpAnim()
	{ 



		OnCorroutine = true;
		myAnim.SetTrigger ("OnJump");
		currentStateBehaviour = statebehaviour.JUMP;
		yield return new WaitForSeconds(jumpAnimTime); // wait for set reload time
		OnCorroutine = false;
		m_jump = false;
		UpdateAnimator("OnIdle");

	}
	
	/// <summary>
	/// auxiliary coroutine for punch animation
	/// </summary>
	/// <returns>The punch animation.</returns>
	IEnumerator RunShootAnim()
	{ 



		OnCorroutine = true;
	    Debug.Log("try to shoot");
		currentStateBehaviour = statebehaviour.SHOOT;
		yield return new WaitForSeconds(shootAnimTime); // wait for set reload time
		 Debug.Log("shoot");
		GetComponentInChildren<Gun>().m_Shoot = true;
		OnCorroutine = false;
		

	}
	

	public void UpdatePosition(float _dx, float _dy) 
	{
	  
	  syncEndPosition = new Vector3(_dx,_dy,transform.position.z);
	  
	  syncStartPosition = transform.position;
	  
	  syncTime = 0f;
	  
	  syncDelay = Time.time - lastSynchronizationTime;
	  
	  lastSynchronizationTime = Time.time;
	  
	  onReceivedPos = true;
	
	  // transform.position = new Vector3 (position.x, position.y, position.z);
	}
	

	public void UpdateRotation(string _rotation) 
	{
		if (!isLocalPlayer) 
		{
		    if(_rotation.Equals("r"))
			{
			  transform.eulerAngles = new Vector2(0,0);
			}
			else 
			{
			  transform.eulerAngles = new Vector2(0,180);
			}
			

		}

	}
	
	public void UpdateJump()
	{
	    if(!m_jump)
		{
		 m_jump = true;
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
		 onAttack = true;
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
		 onAttack = false;
		 break;
	   }
	 }


	

}
}