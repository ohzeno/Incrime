using UnityEngine;
using System.Collections;


namespace MultiverseRiders
{
public class BulletController : MonoBehaviour {

	public bool canMove;
	public bool isLocalBullet;
	public string shooterID;
	public int ammotype;
	public Transform direction;
	
	Rigidbody2D myRigdbody2D;
	CircleCollider2D mycircleCollider2D;
	public float force = 100f;
	
	public GameObject explosionPref;
	public GameObject myExplosionPref;
	public GameObject enemyExplosionPref;

	public float destroyAfter;


	// Use this for initialization
	void Start () {
		myRigdbody2D = GetComponent<Rigidbody2D> ();
		mycircleCollider2D = GetComponent<CircleCollider2D> ();
		Destroy(gameObject, destroyAfter);
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		if (canMove)
		{
		  if(gameObject!=null)
		  {
			if(transform.position.x > direction.position.x)
		    {
		      transform.eulerAngles = new Vector2(0,180);
		    }
		    myRigdbody2D.AddForce(direction.TransformDirection(Vector3.right) * force);
		
		  }

		  }
		 
	}

	public void Shoot(Transform _direction)
	{
		canMove = true;
		direction = _direction;
	}
	
	void OnTriggerEnter2D(Collider2D colisor)
	{

	
		if (colisor.gameObject.name != shooterID 
		&& colisor.gameObject.tag.Equals("NetworkPlayer") && isLocalBullet)
		{
		  
		  //damage on network player
		  ShooterNetworkManager.instance.EmitPlayerDamage (colisor.gameObject.name);
		  Instantiate (explosionPref, transform.position, transform.rotation);
		  Destroy (gameObject);
		  
		  

		}
		if(!isLocalBullet)
		{
		  if (colisor.gameObject.tag.Equals("Player") )
		{
		  
		  Instantiate (explosionPref, transform.position, transform.rotation);
		  Destroy (gameObject);
		}
		}
		
		
		if (colisor.gameObject.name == "wall") {



			
		}

	}

}
}