using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SavanaIO
{
public class PowerUp : MonoBehaviour
{
  
	public string name;
	public string id;
	public PowerUpType powerUpType;
	public bool pickedUp = false;
	

	// Use this for initialization
	void Start () {}


	public void Update(){}
	
	void OnTriggerEnter2D(Collider2D colisor)
	{

	    if (colisor.gameObject.tag.Equals("Player") && colisor.gameObject.GetComponent<Player2DManager>().isLocalPlayer)
		{
		
		  NetworkManager.instance.EmitPickUpItem(id);
		    	  
		}

	}

	
	/// <summary>
	/// Picks up item.
	/// </summary>
	public void PickUpItem()
	{
		 PowerUpManager.instance.DestroyPowerUp(this);
			
	}

	
	
		
}//END_CLASS
}//END_NAMESPACE
