using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.UI;


/// <summary>
/// Network manager class.
/// </summary>
/// 
namespace FPSExample{

public class NetworkManager : MonoBehaviour {

    
    /*********************** DEFAULT VARIABLES ***************************/

    //useful for any gameObject to access this class without the need of instances her or you declare her
	public static NetworkManager instance;

	//Variable that defines comma character as separator
	static private readonly char[] Delimiter = new char[] {':'};

	//flag which is determined the player is logged in the arena
	public bool onLogged = false;

    [HideInInspector]
	//store localPlayer
	public GameObject localPlayer;

	public string local_player_id;

	//store all players in game
	public Dictionary<string, PlayerManager> networkPlayers = new Dictionary<string, PlayerManager>();

    
    /*************************************************************************************************/

    /*********************** PREFABS VARIABLES ******************************************************/

	//store the local players' models
	public GameObject[] localPlayersPrefabs;

	//store the networkplayers' models
	public GameObject[] networkPlayerPrefabs;

	public GameObject camRigPref;
   
   [HideInInspector]
	public GameObject camRig;

	//stores the spawn points 
	public Transform[] spawnPoints;
	
	public bool isGameOver;

    /*************************************************************************************************/


	// Use this for initialization
	void Start () {

		// if don't exist an instance of this class
		if (instance == null) {

		    //it doesn't destroy the object, if other scene be loaded
		    DontDestroyOnLoad (this.gameObject);
			
			instance = this;// define the class as a static variable
			
			Debug.Log("start mmo game");



		}
		else
		{
			//it destroys the class if already other class exists
			Destroy(this.gameObject);
		}

	}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////PING PONG////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	// <summary>
	/// sends ping message to server.
	/// </summary>
	public void EmitPing() {

		//hash table <key, value>	
		Dictionary<string, string> data = new Dictionary<string, string>();

		//store "ping!!!" message in msg field
		data["msg"] = "ping!!!!";

		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit","PING",new JSONObject(data)); //Identifies with the name "PING", the notification to be transmitted to the server

	}
	
	/// <summary>
	/// Prints the pong message which arrived from server.
	/// </summary>
	/// <param name="_msg">Message.</param>
	public void OnPrintPongMsg(string data)
	{

		/*
		 * data.pack[0]= CALLBACK_NAME: "PONG"
		 * data.pack[1]= "pong message!!!"
		*/
		var pack = data.Split (Delimiter);
		Debug.Log ("received message: "+pack[1] +" from server by callbackID: "+pack[0]);
		CanvasManager.instance.ShowAlertDialog ("received message: "+pack[1] +" from server by callbackID: "+pack[0]);
	}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////



////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////[JOIN] [SPAWN AND RESPAWN] FUNCTIONS///////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	// method triggered by the BtnLogin button
	/// <summary>
	/// EmitToServers the player's name to server.
	/// </summary>
	/// <remarks>
	/// Emit the player's name and position to server.
	/// </remarks>
	public void EmitJoinGame()
	{
	
	
		//hash table <key, value>	
		Dictionary<string, string> data = new Dictionary<string, string>();
		
		//player's nickname
		data["name"] = CanvasManager.instance.inputLogin.text;

		//draw a point to spawn the player
		int index = UnityEngine.Random.Range (0, spawnPoints.Length);

		string msg = string.Empty;

		if (!isGameOver) {

		    //Identifies with the name "JOIN", the notification to be transmitted to the server
			data["callback_name"] = "JOIN";
			
			data["name"] = CanvasManager.instance.inputLogin.text;
			
			data["position"] = UtilsClass.Vector3ToString(spawnPoints[index].position);

			
			//sends to the nodejs server through socket the json package
		    Application.ExternalCall("socket.emit", "JOIN_ROOM",new JSONObject(data));
			
		
		}
		else
		{
			Debug.Log ("EmitToServer respawn");
  
			//sends to the nodejs server through socket the json package
		    Application.ExternalCall("socket.emit", "RESPAWN",new JSONObject(data));

		}
		
		//obs: take a look in server script.
	}

	/// <summary>
	/// method to handle notification that arrived from the server.
	/// </summary>
	/// <remarks>
    /// Joins the local player in game.
    /// </remarks>
	/// <param name="data">received package from server.</param>
	public void OnJoinGame(string data)
	{
		
		/*
		 * data.data.pack[0] = id (local player id)
		 * data.data.pack[1] = name (local player name)
		 * data.data.pack[2]= "position.x;position.y;position.z"
		 * data.data.pack[3]= total players
		 
		*/
		
		
		
		if (!localPlayer) {
		
		    Debug.Log("Login successful, joining game");
			
			var pack = data.Split (Delimiter);

		    // the local player now is logged
		    onLogged = true;
		
			// take a look in NetworkPlayer.cs script
			PlayerManager newPlayer;

			// newPlayer = GameObject.Instantiate( local player avatar or model, spawn position, spawn rotation)
			newPlayer = GameObject.Instantiate (localPlayersPrefabs [0],
				UtilsClass.StringToVector3(pack[2]),Quaternion.identity).GetComponent<PlayerManager> ();

			Debug.Log("player instantiated");
			
			newPlayer.id = pack [0];
			
			//this is local player
			newPlayer.isLocalPlayer = true;

			//now local player online in the arena
			newPlayer.isOnline = true;

			//puts the local player on the list
			networkPlayers [pack [0]] = newPlayer;

			//setup local player
			localPlayer = networkPlayers [pack[0]].gameObject;
			
			local_player_id =  pack [0];

			CanvasManager.instance.txtHealth.text = "HP " + newPlayer.gameObject.GetComponent<PlayerHealth>().health + " / " +
				newPlayer.gameObject.GetComponent<PlayerHealth>().maxHealth;
			
			CanvasManager.instance.txtTotalPlayers.text = pack[3];
	
			//hide the lobby menu (the input field and join buton)
			CanvasManager.instance.OpenScreen(1);
			
			Debug.Log("player in game");
		}//END_IF
		
	}

	/// <summary>
	/// method to handle notification that arrived from the server.
	/// </summary>
	/// <remarks>
	/// Raises the spawn network player event.
	/// </remarks>
	/// <param name="data">received package from server.</param>
	void OnSpawnPlayer(string data)
	{

		/*
		 * data.pack[0] = id (network player id)
		 * data.pack[1] = name
		 * data.pack[2]= "position.x;position.y;position.z"
		 * data.pack[3] =  total players
		*/

        var pack = data.Split (Delimiter);
			
		if (!FindPlayer(pack[0])) {
				
					

				PlayerManager newPlayer;

				// newPlayer = GameObject.Instantiate( network player avatar or model, spawn position, spawn rotation)
				newPlayer = GameObject.Instantiate (networkPlayerPrefabs [0],
					UtilsClass.StringToVector3(pack[2] ),Quaternion.identity).GetComponent<PlayerManager> ();
					
				newPlayer.id = pack [0];

				//it is not the local player
				newPlayer.isLocalPlayer = false;

				//network player online in the arena
				newPlayer.isOnline = true;

				//set the network player 3D text with his name
				newPlayer.Set3DName(pack[1]);
				
				newPlayer.gameObject.name = pack [0];
				
				//puts the local player on the list
				networkPlayers [pack [0]] = newPlayer;
				
				CanvasManager.instance.txtTotalPlayers.text = pack[3];

			}


	}

	/// <summary>
	/// method to handle notification that arrived from the server.
	/// </summary>
	/// <remarks>
	/// respawn local player .
	/// </remarks>
	/// <param name="data">received package from server.</param>
	void OnRespawPlayer(string data)
	{   
		/*
		 * data.pack[0] =  id 
		 * data.pack[1] = name
		 * data.pack[2]= "position.x;position.y;position.z"
		 
		*/
      
		Debug.Log("Respawn successful, joining game");
		
		var pack = data.Split (Delimiter);
		
		CanvasManager.instance.OpenScreen(1);
		
		onLogged = true;
		
		isGameOver = false;

		if (localPlayer == null) {

			// take a look in NetworkPlayer.cs script
			PlayerManager newPlayer;

			// newPlayer = GameObject.Instantiate( local player avatar or model, spawn position, spawn rotation)
			newPlayer = GameObject.Instantiate (localPlayersPrefabs [0],
				UtilsClass.StringToVector3(pack[2]),Quaternion.identity).GetComponent<PlayerManager> ();


			Debug.Log("player instantiated");
			
			newPlayer.id = pack [0];
			
			//this is local player
			newPlayer.isLocalPlayer = true;

			//now local player online in the arena
			newPlayer.isOnline = true;

			//puts the local player on the list
			networkPlayers [pack [0]] = newPlayer;

			localPlayer = networkPlayers [pack[0]].gameObject;
			
			local_player_id =  pack [0];
			
			CanvasManager.instance.healthSlider.value = newPlayer.gameObject.GetComponent<PlayerHealth>().health;

			CanvasManager.instance.txtHealth.text = "HP " + newPlayer.gameObject.GetComponent<PlayerHealth>().health + " / " +
				newPlayer.gameObject.GetComponent<PlayerHealth>().maxHealth;

			//hide the lobby menu (the input field and join buton)
			CanvasManager.instance.OpenScreen(1);
			
			Debug.Log("player in game");

		}//END_IF
		
	}
	
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////




////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////PLAYER POSITION AND ROTATION UPDATES///////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// method to send local player position and rotation update to the server.
	/// </summary>
	/// <param name="id">local player id.</param>
	/// <param name="_pos">local player position.</param>
	/// <param name="_rot">local player rotation.</param>
	public void EmitPosAndRot(Dictionary<string, string> data)
	{
	  //Identifies with the name "POS_AND_ROT", the notification to be transmitted to the server,
	  //and send to the server the player's position and rotation
	  JSONObject jo = new JSONObject (data);

	  //sends to the nodejs server through socket the json package
	  Application.ExternalCall("socket.emit", "POS_AND_ROT",jo);
	}

	/// <summary>
	/// Update the network player position and rotation to local player.
	/// </summary>
	/// <param name="data">received package from server.</param>
	void OnUpdatePosAndRot(string data)
	{
		/*
		 * data.pack[0] = id (network player id)
		 * data.pack[1] = "position.x;position.y;posiiton.z"
		 * data.pack[2] = "rotation.x; rotation.y; rotation.z; rotation.w"

		*/
		
		var pack = data.Split (Delimiter);

		if (networkPlayers [pack [0]] != null)
		{
			PlayerManager netPlayer = networkPlayers[pack[0]];

			//update with the new position
			netPlayer.UpdatePosition(UtilsClass.StringToVector3(pack[1]));

			//update new player rotation
			netPlayer.UpdateRotation(float.Parse(pack[2]));

		}
	}
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////




////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////ANIMATION UPDATES/////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// EmitToServers the local player animation to Server.js.
	/// </summary>
	/// <param name="_animation">Animation.</param>
	public void EmitAnimation(string _animation)
	{
		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data["local_player_id"] = localPlayer.GetComponent<PlayerManager>().id;

		data ["animation"] = _animation;
		
		JSONObject jo = new JSONObject (data);

		//sends to the nodejs server through socket the json package
	    Application.ExternalCall("socket.emit", "ANIMATION",new JSONObject(data));//Identifies with the name "ANIMATION", the notification to be transmitted to the server

	}

	/// <summary>
	///  Update the network player animation to local player.
	/// </summary>
	/// <param name="data">received package from server.</param>
	void OnUpdateAnim(string data)
	{
		/*
		 * data.pack[0] =  id (network player id)
		 * data.pack[1] = animation (network player animation)
		*/
		var pack = data.Split (Delimiter);
		
		//find network player by your id
		PlayerManager netPlayer = networkPlayers[pack[0]];

		//updates current animation
		netPlayer.UpdateAnimator(pack[1]);

	}
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////SHOOT UPDATES/////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    /// <summary>
	///  send to server local player shooting event.
	/// </summary>
	public void EmitShoot(Vector3 _target)
	{
		
		
		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();
		
		data["target"] = _target.x+";"+_target.y+";"+_target.z;


		//sends to the nodejs server through socket the json package
	    Application.ExternalCall("socket.emit","SHOOT",new JSONObject(data)); //Identifies with the name "SHOOT", the notification to be transmitted to the server
		
	
		
	}

    /// <summary>
	///  Update the network playershooting to local player.
	/// </summary>
	/// <param name="data">received package from server.</param>
    void OnUpdateShoot(string data)
	{
		
		/*
		 * data.pack[0] =  id (network player id)
		 * data.pack[1] =  target
		*/
		
		var pack = data.Split (Delimiter);
		
		//find network player by your id
		PlayerManager netPlayer = networkPlayers[pack[0]];

		netPlayer.SpawnNetworkBullet(UtilsClass.StringToVector3(pack[1]));
	}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////DAMAGE UPDATES/////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


	// <summary>
	///  sends a notification of client attack done to the server.
	/// </summary>
	public void EmitShootDamage(string _shooterId, string _targetId)
	{
		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data ["shooterId"] = _shooterId;
		
		data ["targetId"] = _targetId;

		//sends to the nodejs server through socket the json package
	    Application.ExternalCall("socket.emit","SHOOT_DAMAGE",new JSONObject(data)); //Identifies with the name "SHOOT", the notification to be transmitted to the server
		
	}


    /// <summary>
	/// receives from server damage done by an opponent .
	/// </summary>
    /// <param name="data">received package from server.</param>
	void OnUpdatePlayerDamage(string data)
	{
		/*
		 * data.pack[0] = attacker.id or shooter.id (network player id)
		 * data.pack[1] = target.id (network player id)
		 * data.pack[2] = target.health
		 */
		 
		 var pack = data.Split (Delimiter);

		if (networkPlayers [pack [1]] != null) 
		{
			
			PlayerManager PlayerTarget = networkPlayers[pack [1]];
			PlayerTarget. GetComponent<PlayerHealth> ().TakeDamage ();


			if (PlayerTarget.isLocalPlayer)
			{
				CanvasManager.instance.healthSlider.value = float.Parse(pack [2]);
				CanvasManager.instance.txtHealth.text = "HP " + pack [2] + " / " 
					+ PlayerTarget. GetComponent<PlayerHealth> ().maxHealth;
					
				CanvasManager.instance.damaged = true;
				
			}
		}

	
	}
	
	

    //receives death notification from server
	void OnPlayerDeath (string data)
	{
		/*
		 * data.pack[0] = id (network player id)
		 
		*/
		
		var pack = data.Split (Delimiter);

		if (networkPlayers [pack [0]] != null) 
		{
			PlayerManager PlayerTarget = networkPlayers[pack [0]];
			
			// if local player is the target
			if (PlayerTarget.isLocalPlayer) 
			{
				 PlayerTarget.GetComponent<PlayerHealth>().Death();
			 
			}
			else
			{
			  PlayerTarget.GetComponent<PlayerHealth>().DoRagDoll();
		      Destroy( networkPlayers[ PlayerTarget.id].gameObject);
		      networkPlayers.Remove(PlayerTarget.id);
			}

		}

	}







	public void GameOver()
	{
		if (onLogged) {

		Destroy( localPlayer);
		
		networkPlayers.Remove(localPlayer.GetComponent<PlayerManager>().id);
		
		localPlayer = null;
		isGameOver = true;
		CanvasManager.instance.ShowGameOverDialog ();
		CanvasManager.instance.lobbyCamera.GetComponent<Camera> ().enabled = true;
		    
			
		}
	}
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////



////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////DISCONNECTION FUNCTION////////////////////////////////////////////
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

		if (networkPlayers.ContainsKey(pack [0]))
		{


			//destroy network player by your id
			Destroy( networkPlayers[pack[0]].gameObject);


			//remove from the dictionary
			networkPlayers.Remove(pack[0]);

		}

	}

	

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////// HELPERS /////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 
bool FindPlayer(string _id)
{
     bool found = false;

			//verify all players to  prevents copies
			foreach(KeyValuePair<string, PlayerManager> entry in networkPlayers)
			{
				// same id found ,already exist!!! 
				if (entry.Value.id.Equals(_id))
				{
					found = true;
					break;
				}
			}

			return found;
  }
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}//END_OF_CLASS
}//END_OF_NAMESPACE