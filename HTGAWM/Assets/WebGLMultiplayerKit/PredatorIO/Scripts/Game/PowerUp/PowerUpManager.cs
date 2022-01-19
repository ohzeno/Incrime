using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace SavanaIO
{
public class PowerUpManager : MonoBehaviour
{

	public static PowerUpManager instance;

	public GameObject[] PowerUpsPref;
	
	public ArrayList spawnedPowerUpList = new ArrayList();

	public ArrayList respawnPowerUpList = new ArrayList();


    // Start is called before the first frame update
    void Start()
    {
		if (instance == null)
		{
			DontDestroyOnLoad(this.gameObject);

			instance = this;

			spawnedPowerUpList = new ArrayList();

			respawnPowerUpList = new ArrayList();

		}
		else
		{
			Destroy(this.gameObject);
		}
    }

  
	/// <summary>
	/// Spawns all power ups when the game starts.
	/// </summary>
	public void SpawnPowerUp(string _id, int _type,float _posX,float _posY)
	{
		
		GameObject newPowerUp = GameObject.Instantiate (PowerUpsPref [_type],
				               new Vector3 (_posX, _posY,0), Quaternion.identity) as GameObject;
						
		newPowerUp.GetComponent<PowerUp> ().id = _id;
		
		switch (_type)
		{
		    
		    case 0:
			newPowerUp.GetComponent<PowerUp> ().powerUpType = PowerUpType.ORANGE;
			break;
			case 1:
			 newPowerUp.GetComponent<PowerUp> ().powerUpType = PowerUpType.YELLOW;
			break;
			case 2:
			newPowerUp.GetComponent<PowerUp> ().powerUpType = PowerUpType.GREEN;
			break;
			case 3:
			newPowerUp.GetComponent<PowerUp> ().powerUpType = PowerUpType.PURPLE;
			break;
		}
		
		spawnedPowerUpList.Add ( newPowerUp.GetComponent<PowerUp> ());
					
	}

	public void Pickup(string _id)
	{
		foreach(PowerUp powerUp in spawnedPowerUpList)
		{
			if(powerUp.id.Equals(_id))
			{   
				powerUp.PickUpItem();
				break;
			}
		}

		spawnedPowerUpList.Remove(_id);

	}


	public void DestroyPowerUp(PowerUp _powerUp)
	{
		 foreach(PowerUp powerUp in spawnedPowerUpList)
		   {
		     if(powerUp.Equals(_powerUp))
		     {
			 
			   Destroy(powerUp.gameObject);
			   spawnedPowerUpList.Remove (powerUp);
			   break;
		     }
		   }

	}

	
	 /// <summary>
	/// Clears .
	/// </summary>
	public void Clear()
	{
			
		foreach(PowerUp powerUp in spawnedPowerUpList)
		{
		  Destroy(powerUp.gameObject);
		}
		spawnedPowerUpList.Clear ();
	}

}//END_CLASS
}//END_NAMESPACE
