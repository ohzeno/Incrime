using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.UI;
using System.Runtime.InteropServices;

/// <summary>
/// Network Manager class.
/// </summary>
///
namespace Tutorial{
public class NetworkManager : MonoBehaviour {


    public static NetworkManager instance; //useful for any gameObject to access this class without the need of instances her or you declare it

	static private readonly char[] Delimiter = new char[] {':'}; 	//Variable that defines ':' character as separator
    
	[HideInInspector]
	public bool onLogged = false; //flag which is determined the player is logged in the game arena

	[HideInInspector]
	public GameObject localPlayer; //store localPlayer

    [HideInInspector]
	public string local_player_id;

	//store all players in game
	public Dictionary<string, PlayerManager> networkPlayers = new Dictionary<string, PlayerManager>();
	
	[Header("Local Player Prefab")]
	public GameObject localPlayerPrefab; //store the local player prefabs

	[Header("Network Player Prefab")]
	public GameObject networkPlayerPrefab; //store the remote player prefabs

	[Header("Spawn Points")]
	public Transform[] spawnPoints; //stores the spawn points

    [Header("Camera Rig Prefab")]
	public GameObject camRigPref;

    [HideInInspector]
	public GameObject camRig;

	[HideInInspector]
	public bool isGameOver; // game over flag



	

	void Awake()
	{
		Application.ExternalEval("socket.isReady = true;");

	}

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



	/// <summary>
	/// Prints the pong message which arrived from server.
	/// </summary>
	/// <param name="_msg">Message.</param>
	public void OnPrintPongMsg(string data)
	{

		/*
		 * data.pack[0]= msg
		*/

		var pack = data.Split (Delimiter);
		Debug.Log ("received message: "+pack[0] +" from server by callbackID: PONG");
	}

	// <summary>
	/// sends ping message to server.
	/// </summary>
	public void EmitPing() {

		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		//store "ping!!!" message in msg field
	    data["msg"] = "ping!!!!";

		JSONObject jo = new JSONObject (data);

		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", "PING",new JSONObject(data));


	}





	//call be  OnClickJoinBtn() method from CanvasManager class
	/// <summary>
	/// Emits the player's name to server.
	/// </summary>
	/// <param name="_login">Login.</param>
	public void EmitJoin()
	{
		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();


		//player's name
		data["name"] = CanvasManager.instance.inputLogin.text;
		
		
		//makes the draw of a point for the player to be spawn
		int index = Random.Range (0, spawnPoints.Length);

		//send the position point to server
		string msg = string.Empty;


		data["name"] = CanvasManager.instance.inputLogin.text;
			
			

		data["position"] = spawnPoints[index].position.x+":"+spawnPoints[index].position.y+":"+spawnPoints[index].position.z;

		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", "LOGIN",new JSONObject(data));


	
		//obs: take a look in server script.
	}

	/// <summary>
	/// Joins the local player in game.
	/// </summary>
	/// <param name="_data">Data.</param>
	public void OnJoinGame(string data)
	{
		Debug.Log("Login successful, joining game");
		
		var pack = data.Split (Delimiter);
		
	
		// the local player now is logged
		onLogged = true;

		/*
		 * pack[0] = id (local player id)
		 * pack[1]= name (local player name)
		 * pack[2] = position.x (local player position x)
		 * pack[3] = position.y (local player position ...)

		*/

			// take a look in NetworkPlayer.cs script
			PlayerManager newPlayer;

			// newPlayer = GameObject.Instantiate( local player avatar or model, spawn position, spawn rotation)
			newPlayer = GameObject.Instantiate (localPlayerPrefab,
				new Vector3(float.Parse(pack[2]), float.Parse(pack[3]),
					float.Parse(pack[4])),Quaternion.identity).GetComponent<PlayerManager> ();


			Debug.Log("player instantiated");

			newPlayer.id = pack [0];
			//this is local player
			newPlayer.isLocalPlayer = true;

			//now local player online in the arena
			newPlayer.isOnline = true;

			//set local player's 3D text with his name
			newPlayer.Set3DName(pack[1]);

			//puts the local player on the list
			networkPlayers [pack [0]] = newPlayer;

			localPlayer = networkPlayers [pack[0]].gameObject;

			local_player_id =  pack [0];

			//spawn camRigPref from Standard Assets\Cameras\Prefabs\MultipurposeCameraRig.prefab
			camRig = GameObject.Instantiate (camRigPref, new Vector3 (0f, 0f, 0f), Quaternion.identity);

			//set local player how  being MultipurposeCameraRig target to follow him
			camRig.GetComponent<CameraFollow> ().SetTarget (localPlayer.transform, newPlayer.cameraToTarget);

			//hide the lobby menu (the input field and join buton)
			CanvasManager.instance.OpenScreen(1);
			Debug.Log("player in game");
		
	}

	/// <summary>
	/// Raises the spawn player event.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnSpawnPlayer(string data)
	{

		/*
		 * pack[0] = id (network player id)
		 * pack[1]= name
		 * pack[3] = position.x
		 * pack[4] = position.y
		 * pack[5] = position.z
		*/

        var pack = data.Split (Delimiter);

		bool alreadyExist = false;

		//verify all players to avoid duplicates 
		if(networkPlayers.ContainsKey(pack [0]))
		{
			alreadyExist = true;
		}
		if (!alreadyExist)
		{
			Debug.Log("received spawn network player");

		  

	
		    PlayerManager newPlayer;

		    // newPlayer = GameObject.Instantiate( network player avatar or model, spawn position, spawn rotation)
		    newPlayer = GameObject.Instantiate (networkPlayerPrefab,
					new Vector3(float.Parse(pack[2]), float.Parse(pack[3]),
						float.Parse(pack[4])),Quaternion.identity).GetComponent<PlayerManager> ();


            Debug.Log("player spawned");

		    newPlayer.id = pack [0];

				
		    newPlayer.isLocalPlayer = false; //it is not the local player
				
		    newPlayer.isOnline = true; //set network player online in the arena

				
		     newPlayer.Set3DName(pack[1]); //set the network player 3D text with his name

		     newPlayer.gameObject.name = pack [0];
				
		     networkPlayers [pack [0]] = newPlayer; //puts the network player on the list
		}

				
	}

	
	
    /// <summary>
    /// send player's position and rotation to the server
    /// </summary>
    /// <param name="data"> package with player's position and rotation</param>
	public void EmitMoveAndRotate( Dictionary<string, string> data)
	{

		JSONObject jo = new JSONObject (data);

		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", "MOVE_AND_ROTATE",new JSONObject(data));
	
	}



	/// <summary>
	/// Update the network player position and rotation to local player.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnUpdateMoveAndRotate(string data)
	{
		/*
		 * data.pack[0] = id (network player id)
		 * data.pack[1] = position.x
		 * data.pack[2] = position.y
		 * data.pack[3] = position.z
		 * data.pack[4] = "rotation.y"
		*/

		Debug.Log("received pos and rot");
		
		var pack = data.Split (Delimiter);
		
		if (networkPlayers.ContainsKey(pack [0]))
		{
		    
			PlayerManager netPlayer = networkPlayers[pack[0]];

			//update with the new position
			netPlayer.UpdatePosition(new Vector3(
				float.Parse(pack[1]), float.Parse(pack[2]), float.Parse(pack[3])));
				
			//update new player rotation
			netPlayer.UpdateRotation(new Quaternion (netPlayer.transform.rotation.x,float.Parse(pack[4]),
			netPlayer.transform.rotation.z,netPlayer.transform.rotation.w));
		
		}
		

	}
	

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


}
}