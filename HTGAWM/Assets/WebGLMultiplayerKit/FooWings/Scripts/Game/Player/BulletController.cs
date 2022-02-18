using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace MultiplayerShooter
{
public class BulletController :MonoBehaviour {

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

	public float offsetY;
	
	public float damage = 10;

	Vector3 latestPos  = Vector3.zero;

	   
   /****************************************************************************/

  

  /***********************  EFFECTS VARIABLES **********************************/

	[Header("Effects variables :")]

	public GameObject explosionPref;  // set in inspector.

	public GameObject mediumExplosionPref;  // set in inspector.

	public AudioClip explosionAudio;   // set in inspector. 

    private void Awake ()
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
		

	public void Shoot(Vector3 _target , string _shooterID)
	{
		   canMove = true;
		   target = _target;
		   target_position = new Vector3(target.x,target.y,target.z);
		   shooterID = _shooterID;
		  
	}



		
	void OnTriggerEnter(Collider colisor)
     {
		
		if (colisor.gameObject.GetComponentInParent<PlayerManager>().id != shooterID 
		&& colisor.gameObject.tag.Equals("Player") && isLocalBullet)
		{
		  Instantiate (explosionPref, transform.position, transform.rotation);
		  NetworkManager.instance.EmitPlayerDamage (shooterID,colisor.gameObject.GetComponentInParent<PlayerManager>().id);
		  Destroy (gameObject);
		  
		  

		}
		if(!isLocalBullet)
		{
		  if (colisor.gameObject.GetComponentInParent<PlayerManager>().id != shooterID 
		  && colisor.gameObject.tag.Equals("Player") )
		   {
		  
		     Instantiate (explosionPref, transform.position, transform.rotation);
		     Destroy (gameObject);
		   }
		}

		if ( colisor.gameObject.tag.Equals("Floor"))
		{
		  Instantiate (mediumExplosionPref, transform.position, transform.rotation);

		  Destroy (gameObject);
		  
		  

		}
	
			
	 }


	

}//END_CLASS
}//END_NAMESPACE