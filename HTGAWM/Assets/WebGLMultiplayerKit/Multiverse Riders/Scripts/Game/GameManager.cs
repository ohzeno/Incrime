using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiverseRiders
{
public class GameManager : MonoBehaviour
{

    //useful for any gameObject to access this class without the need of instances her or you declare her
	public static GameManager instance;
	

	public GameObject[] mapsPref;

	public GameObject map;

	
    // Start is called before the first frame update
    void Start()
    {
         // if don't exist an instance of this class
	 if (instance == null) {

		//it doesn't destroy the object, if other scene be loaded
		DontDestroyOnLoad (this.gameObject);

		instance = this;// define the class as a static variable
	
		
	 }
	 else
	 {
		//it destroys the class if already other class exists
		Destroy(this.gameObject);
	 }
    }

    public void SpawnMap(string _map)
	{
		
		switch (_map)
		{
		    
		    case "map1":
			 map = Instantiate (mapsPref[0], mapsPref[0].transform.position,
            Quaternion.identity);
			
			break;
			 case "map2":
			 map = Instantiate (mapsPref[1], mapsPref[2].transform.position,
            Quaternion.identity);
		
			
			break;
			 case "map3":
			 map = Instantiate (mapsPref[2], mapsPref[2].transform.position,
            Quaternion.identity);
			
		
			break;
		}
	}
}
}