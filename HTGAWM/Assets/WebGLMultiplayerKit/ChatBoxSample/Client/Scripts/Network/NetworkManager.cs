using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.UI;
using System.Runtime.InteropServices;

/// <summary>
/// Class to manage the game client's network communication.
/// </summary>
/// 
namespace ChatBox
{

public class NetworkManager : MonoBehaviour
{

	//useful for any gameObject to access this class without the need of instances her or you declare her
	public static NetworkManager instance;

	//flag which is determined the player is logged in the arena
	public bool onLogged = false;

    string local_player_id;

	
	[HideInInspector]
	public GameObject localPlayer; //store localPlayer

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


	//Variable that defines ':' character as separator
	static private readonly char[] Delimiter = new char[] {':'};
	
	
	
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
		
			
			 Debug.Log(" ------- chat started ------");
			

		}
		else
		{
			//it destroys the class if already other class exists
			Destroy(this.gameObject);
		}

    }
	
	

    

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////[JOIN] [SPAWN AND RESPAWN] FUNCTIONS///////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	// method triggered by the BtnLogin button
	/// <summary>
	/// EmitToServers the player's name to server.
	/// </summary>
	public void EmitJoinRoom()
	{
	
	
		//hash table <key, value>	
		Dictionary<string, string> data = new Dictionary<string, string>();
		
	
		string msg = string.Empty;


		//Identifies with the name "JOIN", the notification to be transmitted to the server
		data["callback_name"] = "JOIN";

		//makes the draw of a point for the player to be spawn
		int index = Random.Range (0, spawnPoints.Length);

		data["position"] = spawnPoints[index].position.x+":"+spawnPoints[index].position.y+":"+spawnPoints[index].position.z;

			
		data["name"] = CanvasManager.instance.ifLogin.text;
		
		data["avatar"] = CanvasManager.instance.currentSkin.ToString();
			
		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", data["callback_name"],new JSONObject(data));
		
		
	
		//obs: take a look in server script.
	}

	/// <summary>
	/// method to handle notification that arrived from the server.
	/// </summary>
	/// <remarks>
    /// Joins the local player in game.
    /// </remarks>
	/// <param name="_data">Data.</param>
	public void OnJoinGame(string data)
	{
		
		/*
		 * data.pack[0] = id (local player id)
		 * data.pack[1]= username (local player name)
		 * data.pack[2] = position.x
		 * data.pack[3] = position.y
		 * data.pack[4] = position.z
		 * data.pack[5] = " local user avatar index"
		*/
	
	
			var pack = data.Split (Delimiter);
			
		    Debug.Log("Login successful, joining game");

		    // the local player now is logged
		    onLogged = true;
		
				// take a look in NetworkPlayer.cs script
			PlayerManager newPlayer;

			// newPlayer = GameObject.Instantiate( local player avatar or model, spawn position, spawn rotation)
			newPlayer = GameObject.Instantiate (localPlayerPrefab,
				new Vector3(float.Parse(pack[2]), float.Parse(pack[3]),
					float.Parse(pack[4])),Quaternion.identity).GetComponent<PlayerManager> ();


			Debug.Log("player instantiated");

			newPlayer.id = pack [0];

			newPlayer.name = pack[1];

			newPlayer.avatar =  int.Parse(pack[5]);

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

			Debug.Log("player in game");
		
			//hide the lobby menu (the input field and join buton)
			CanvasManager.instance.OpenScreen("room");
			
			Debug.Log("player in game");
			
	
	}


     /// <summary>
	/// method to handle notification that arrived from the server.
	/// </summary>
	/// <remarks>
	/// Raises the spawn player event.
	/// </remarks>
	/// <param name="data">received package.</param>
	void OnSpawnPlayer(string data)
	{

		/*
		 * data.pack[0] = id (network player id)
		 * data.pack[1]= name
		 * data.pack[2] = position.x
		 * data.pack[3] = position.y
		 * data.pack[4] = position.z
		 * data.pack[5] = " local user avatar index"
		*/

		if(onLogged)
		{
			
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

			newPlayer.avatar =  int.Parse(pack[5]);

			newPlayer.name = pack[1];
				
		    newPlayer.isLocalPlayer = false; //it is not the local player
				
		    newPlayer.isOnline = true; //set network player online in the arena
	
		    newPlayer.Set3DName(pack[1]); //set the network player 3D text with his name

		    newPlayer.gameObject.name = pack [0];
				
		    networkPlayers [pack [0]] = newPlayer; //puts the network player on the list

			Debug.Log("player configured");

			CanvasManager.instance.SpawnUser(newPlayer.id, newPlayer.name, newPlayer.avatar);

			
		
		}

		}

	}
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////MESSAGE FUNCTIONS////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


 /// <summary>
	/// method to emit message to the server.
	/// </summary>
	public void EmitOpenChatBox(string _player_id)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		
		string msg = string.Empty;

		//Identifies with the name "MESSAGE", the notification to be transmitted to the server
		data["callback_name"] = "SEND_OPEN_CHAT_BOX";
		
		data ["player_id"] = _player_id;
		
		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", data["callback_name"],new JSONObject(data));
		
	
	}


	

	
	 /// <summary>
	/// method to handle notification that arrived from the server.
	/// </summary>	
	/// <param name="data">received package from server.</param>
	void OnReceiveOpenChatBox(string data)
	{
	
	/*
		 * data.pack[0] = host id 
		 * data.pack[1]= guest id
		*/
		
    
		  
		var pack = data.Split (Delimiter);

		if(local_player_id.Equals(pack[0]))
		{
			//spawn new chatbox
		    CanvasManager.instance.SpawnChatBox( pack[0],pack[0],pack[1], networkPlayers[pack[1]].name, networkPlayers[pack[1]].avatar);

		}
		else
		{
			CanvasManager.instance.SpawnChatBox( pack[0],pack[1],pack[0], networkPlayers[pack[0]].name, networkPlayers[pack[0]].avatar);


		}

	

	
			 
	}
	

     /// <summary>
	/// method to emit message to the server.
	/// </summary>
	public void EmitMessage(string _message,string _chat_box_id, string _gest_id)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		
		string msg = string.Empty;

		//Identifies with the name "MESSAGE", the notification to be transmitted to the server
		data["callback_name"] = "MESSAGE";

		data["chat_box_id"] = _chat_box_id;
		
		data ["guest_id"] = _gest_id;
		
		data ["message"] = _message;
		
		CanvasManager.instance.inputFieldMessage.text = string.Empty;
			
		
		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", data["callback_name"],new JSONObject(data));
		
	
	}
	
	 /// <summary>
	/// method to handle notification that arrived from the server.
	/// </summary>	
	/// <param name="data">received package from server.</param>
	void OnReceiveMessage(string data)
	{
	
	/*
		 * data.pack[0] = guest (network player id)
		 * data.pack[1]= message
		*/
		
    
		  
		var pack = data.Split (Delimiter);

	    if( CanvasManager.instance.chatBoxes.ContainsKey(pack [0]))
		{
		  if (local_player_id.Equals(pack[1])) {
			  
			 
			CanvasManager.instance.chatBoxes[pack[0]].SpawnMyMessage(pack[2]);
			
		   }
		   else
		   {
			 CanvasManager.instance.chatBoxes[pack[0]].SpawnNetworkMessage(pack[2]);
		   }
		}
	
			 
	}
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


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
	


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////DISCONNECTION FUNCTION////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
	 /// <summary>
	/// method to handle notification that arrived from the server.
	/// </summary>
	/// <remarks>
	/// inform the local player to destroy offline network player
	/// //disconnect network player
	/// </remarks>
	/// <param name="data">received package.</param>
	void OnUserDisconnected(string data)
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
////////////////////////////////////////////// HELPERS ////////////////////////////////////////////
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
