using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FPSExample{
public class Bullet3DController : MonoBehaviour {

    /*********************** DEFAULT VARIABLES ***************************/
	[Header("Default variables :")]
    public string shooterID;

	[HideInInspector]
    public string ownersName = "";   // name of player that launched bullet

	public bool isLocalBullet;	

	Rigidbody myRigidybody;
    
	[HideInInspector]
	public bool canMove;

	Vector3 target;

	Vector3 target_position;
		
	public float bulletSpeed;  // set in inspector.

	   
   /****************************************************************************/

  

  /***********************  EFFECTS VARIABLES **********************************/

	[Header("Effects variables :")]

	public GameObject explosionPref;  // set in inspector.

	public AudioClip explosionAudio;   // set in inspector. 

    private void Start ()
    {
		myRigidybody = GetComponent<Rigidbody> ();
      
    }

	// Update is called once per frame
	void FixedUpdate () {

        MoveBullet();
	
	}

	void MoveBullet()
	{
      if (canMove)
		 {
			
			myRigidybody.AddForce (bulletSpeed * target);
		 }
	}
		

	public void Shoot(Vector3 _target , string playerName)
	{
		   canMove = true;
		   target = _target;
		   ownersName = playerName;

	}

		
	void OnTriggerEnter(Collider colisor)
     {
		
		if (colisor.gameObject.GetComponent<PlayerManager>().id  != shooterID 
		&& colisor.gameObject.tag.Equals("NetworkPlayer"))
		{
		 
		  //bullet fired by the local player
		  if(isLocalBullet)
		  {
		     //sends notification to the server with the shooter ID and target ID
		     NetworkManager.instance.EmitShootDamage (shooterID,colisor.gameObject.
			 GetComponent<PlayerManager>().id);
		    
			 //triggers the damage animation on the network player hit
			 colisor.gameObject.GetComponent<PlayerManager>().UpdateAnimator ("OnDamage");

            //instantiate an explosion effect
		   // Instantiate (explosionPref, transform.position, transform.rotation);
		   
		  }//END_IF
		  
		}//END_IF
		
		 Destroy (gameObject);
		
			
	 }


	public void PlayExplisionSound()
	{
	//	GetComponent<AudioSource>().PlayOneShot(explosionAudio);
	}
   

}
}
