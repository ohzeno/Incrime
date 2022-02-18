using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SavanaIO
{
public class GameManager : MonoBehaviour
{

    //useful for any gameObject to access this class without the need of instances her or you declare her
	public static GameManager instance;
	
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
}
