using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Cinemachine;

namespace MultiplayerShooter
{
public class NetworkManager :  MonoBehaviour
{

		//useful for any gameObject to access this class without the need of instances her or you declare her
	public static NetworkManager instance;
	
	//Variable that defines comma character as separator
	static private readonly char[] Delimiter = new char[] {':'};

	/*********************** PREFABS VARIABLES ******************************************************/
   [Header("Players Prefabs")]
   //store the local players' models
   public GameObject playersPrefab;
 
  /*************************************************************************************************/

   //stores the spawn points 
   public Transform[] spawnPoints;


   //store all players in game
   public Dictionary<string, PlayerManager> networkPlayers = new Dictionary<string, PlayerManager>();

   public GameObject localPlayer;

   bool isGameOver;

	[Header("Camera")]
   public CinemachineVirtualCamera cinemachineVirtualCam;
   

	void Awake()
	{
		Application.ExternalEval("socket.isReady = true;");
		
	}

   
	
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

	
	/// <summary>
	/// Emits the join game to Server.
	/// case "JOIN_GAME":
	///   OnReceiveJoinGame(pack,anyIP);
	///  break;
	/// take a look in Server.cs script
	/// </summary>
	public void EmitJoinGame()
	{
	 
	  //hash table <key, value>	
	  Dictionary<string, string> data = new Dictionary<string, string>();

		
		//send the position point to server
		string msg = string.Empty;

		if (!isGameOver) 
		{
			
		 //player's name	
		 data["name"] = LobbyManager .instance.inputLogin.text;
			  
	     data["avatar"] = ButtonChooseManager.instance.currentWings.ToString();
		 
		 //makes the draw of a point for the player to be spawn
		 int index = UnityEngine.Random.Range (0, spawnPoints.Length);

		 data["position"] = spawnPoints[index].position.x+":"+spawnPoints[index].position.y+":"+spawnPoints[index].position.z;
			  
			
		 //sends to the nodejs server through socket the json package
		 Application.ExternalCall("socket.emit", "JOIN_ROOM",new JSONObject(data));

		}
		else
		{
			
		    isGameOver = false;
		   
		    //player's name	
		    data["name"] = LobbyManager .instance.inputLogin.text;
			   
	        data["avatar"] = ButtonChooseManager.instance.currentWings.ToString();
		 
		    //makes the draw of a point for the player to be spawn
		    int index = UnityEngine.Random.Range (0, spawnPoints.Length);

		    data["position"] = spawnPoints[index].position.x+":"+spawnPoints[index].position.y+":"+spawnPoints[index].position.z;
			  
			//sends to the nodejs server through socket the json package
		    Application.ExternalCall("socket.emit", "RESPAWN",new JSONObject(data));

		}

		
		


	}

		/// <summary>
	/// Raises the join game event from Server.
	/// only the first player to connect gets this feedback from the server
	/// </summary>
	/// <param name="data">Data.</param>
	void OnJoinGame(string data)
	{
			/*
		 * pack[0] = id (local player id)
		 * pack[1]= name (local player name)
		 * pack[2]= avatar 
		
		 
		*/
		
		Debug.Log ("\n joining ...\n");
		
		var pack = data.Split (Delimiter);

		
		Debug.Log("Login successful, joining game");
		Debug.Log("pack[1]: "+pack[1]);
		Debug.Log("pack[1]: "+pack[2]);


		if (!localPlayer) {
		
		  
			// take a look in PlayerManager.cs script
		    PlayerManager newPlayer;
			
			newPlayer = GameObject.Instantiate (playersPrefab,
		new Vector3(float.Parse(pack[3]), float.Parse(pack[4]),
					float.Parse(pack[5])), Quaternion.identity).GetComponent<PlayerManager> ();

		    Debug.Log("player instantiated");

			newPlayer.SetUpPlayer(pack[0], pack[1], pack[2],true);
			  
			//puts the local player on the list
			networkPlayers [ pack[0]] = newPlayer;

			localPlayer = newPlayer.gameObject;

		    cinemachineVirtualCam.LookAt = newPlayer.target.transform;
		    
			cinemachineVirtualCam.Follow = newPlayer.target.transform;

			LobbyManager.instance.OpenScreen("gamecanvas");
				
			Debug.Log("player in game");
		}

	}

		/// <summary>
	/// Raises the spawn player event.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnSpawnPlayer(string data)
	{

		/*
		 * pack[0] = id
		 * pack[1] = name
		 * pack[2]= avatar
		 * pack[3] = position.x
		 * pack[4] = position.y
		 * pack[5] = position.z
		*/
		
		var pack = data.Split (Delimiter);

		Debug.Log ("\n spawning network player ...\n");
		

	    bool alreadyExist = false;

			
	    if(networkPlayers.ContainsKey(pack [0]))
	    {
			alreadyExist = true;
		}
		if (!alreadyExist) {
			
			
		 PlayerManager newPlayer;

		  newPlayer =  GameObject.Instantiate (playersPrefab,
		  new Vector3(float.Parse(pack[3]), float.Parse(pack[4]),
					float.Parse(pack[5])),
			  Quaternion.identity).GetComponent<PlayerManager> ();	   
			   
		  newPlayer.SetUpPlayer(pack[0], pack[1],  pack[2],false);
			  
		  //puts the local player on the list
		  networkPlayers [pack [0]] = newPlayer;

		  Debug.Log ("\n  network player in game\n");
			
		}//END_IF
			
	

	}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////PLAYER POSITION AND ROTATION UPDATES///////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// 
	//send local player position and rotation to server
	public void EmitPosAndRot(Dictionary<string, string> data)
	{
	    JSONObject jo = new JSONObject (data);

	    //sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", "POS_AND_ROT",new JSONObject(data));

	}

	/// <summary>
	/// Update the network player position and rotation to local player.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnUpdatePosAndRot(string data)
	{
		
	    /*
		 * pack[0] = id
		 * pack[1] = dx
		 * pack[2] = dz
		 * pack[3] = rotation
		*/

	  if(networkPlayers.Count > 0)
	  {


		var pack = data.Split (Delimiter);

	   if (networkPlayers [pack [0]] != null)
		{
		 
		  
		  PlayerManager netPlayer = networkPlayers[pack [0]];
		 
		  //update with the new position
		  netPlayer.UpdatePosition(UtilsClass.StringToVector3(pack[1]));
		  Vector4 rot = UtilsClass.StringToVector4(pack[2]);// convert string to Vector4
	      //update new player rotation
		  netPlayer.UpdateRotation(new Quaternion(rot.x,rot.y,rot.z,rot.w));

		  
			
		}
	
	 }
	}
	
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////SHOOT UPDATES/////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////



	public void EmitShoot(int _currentGun)
	{
		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data["currentGun"] = _currentGun.ToString();

		 //sends to the nodejs server through socket the json package
		 Application.ExternalCall("socket.emit", "SHOOT",new JSONObject(data));
		
	}

    void OnUpdateShoot(string data)
	{
		
		/*
		 * pack[0] = id
		 * pack[1] = currentGun
		
		*/
		
		var pack = data.Split (Delimiter);

		if( networkPlayers[pack[0]]!=null)
		{

		   PlayerManager netPlayer = networkPlayers[pack[0]];
		   netPlayer.gameObject.GetComponentInChildren<Gun> ().NetworkShoot(int.Parse(pack[1]));
	

		}
	}


	

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////// BUSTER EFFECT /////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////



	public void EmitChangeBuster()
	{
	

		 //sends to the nodejs server through socket the json package
		 Application.ExternalCall("socket.emit", "CHANGE_BUSTER");
		
	}

    void OnUpdateBuster(string data)
	{
		
		/*
		 * pack[0] = id
		
		*/
		
		var pack = data.Split (Delimiter);

		if( networkPlayers[pack[0]]!=null)
		{
			//Debug.Log("update buster");

		   PlayerManager netPlayer = networkPlayers[pack[0]];
		   netPlayer.ChangeBuster();
	

		}
	}


	

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////



////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////DAMAGE UPDATES/////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  public void EmitPlayerDamage(string _shooter_id, string _target_id)
  {
  
	 //hash table <key, value>
	 Dictionary<string, string> data = new Dictionary<string, string>();

	 data ["shooter_id"] = _shooter_id;
		
	 data ["target_id"] = _target_id;
	 
	  //sends to the nodejs server through socket the json package
	  Application.ExternalCall("socket.emit", "DAMAGE",new JSONObject(data));
	 

  }
  
  /// <summary>
	///  Update the network player animation to local player.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnUpdatePlayerDamage(string data)
	{
		
	 /*
		 * data.pack[0] = id
		 * data.pack[1] = health
		
		*/
		
	  if(networkPlayers.Count > 0)
	  {
		var pack = data.Split (Delimiter);
		
	   //find network player by your id
	   PlayerManager netPlayer = networkPlayers[pack[0]];

	   if(networkPlayers[pack[0]].isLocalPlayer)
	   {
		    GameCanvas.instance.txtLocalPlayerHealth.text = pack[1];
			 GameCanvas.instance.healthSlider.value = float.Parse(pack[1]);
			
			GameCanvas.instance.damaged = true;
		
	   }
	   else
	   {
		    networkPlayers[pack[0]].GetComponent<PlayerHealth>().TakeDamage (float.Parse(pack[1]));
		
	   }
	  }
		
	}

		  /// <summary>
	///  Update the network player animation to local player.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnGameOver(string data)
	{
		
		/*
		 * pack[0] = target_id,
		 * pack[1] = shooter_id
		 * pack[2] = shooter_kills
	
		*/
	if(networkPlayers.Count > 0)
	{
	  var pack = data.Split (Delimiter);
	  
	
		//find target by your id
		PlayerManager target = networkPlayers[pack[0]];
		
		PlayerManager shooter = networkPlayers[pack[1]];
		
		
		if(networkPlayers[pack[0]].isLocalPlayer)
		{
		    ResetGame(); 
		}
		else
		{
		   
		  
		  Destroy (networkPlayers [pack[0]].gameObject);
		  
		  networkPlayers.Remove (pack[0]);
				
		}
		
		
	}
		
	}
	
	void ResetGame()
	{
	    Debug.Log("reset game");
		
		isGameOver = true;
		
		GameCanvas.instance.GameOver();
		
		localPlayer = null;
		
		//send answer in broadcast
		foreach (KeyValuePair<string, PlayerManager> entry in networkPlayers) {

		  Destroy(entry.Value.gameObject);
		  
		}//END_FOREACH
		
		networkPlayers.Clear();
		
		GameCanvas.instance.healthSlider.value = 100;
	
		LobbyManager.instance.OpenScreen("hudlobby");
		
		
	}
	
	
   
    
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////



	/// <summary>
	/// inform the local player to destroy offline network player
	/// </summary>
	/// <param name="_msg">Message.</param>
	//desconnect network player
	void OnUserDisconnected(string data )
	{

		/*
		 * data.pack[0] = id (network player id)
		*/

		var pack = data.Split (Delimiter);
		
		Debug.Log("user: "+networkPlayers[pack[0]].name+" disconnected");
 
		if (networkPlayers [pack [0]] != null)
		{


			//destroy network player by your id
			Destroy( networkPlayers[pack[0]].gameObject);


			//remove from the dictionary
			networkPlayers.Remove(pack[0]);

		}
		
	}

}//END_CLASS
}//END_NAMESPACE