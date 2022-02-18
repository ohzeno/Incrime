using System.Collections;
using System.Collections.Generic;
//using System.Globalization;
using System.Text.RegularExpressions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;


namespace MultiverseRiders
{

public class ShooterNetworkManager : MonoBehaviour
{
   
	//useful for any gameObject to access this class without the need of instances her or you declare her
	public static ShooterNetworkManager instance;

	//flag which is determined the player is logged in the arena
	public bool onLogged = false;

	//local player id
	public string myId = string.Empty;
	
	//Variable that defines comma character as separator
	static private readonly char[] Delimiter = new char[] {':'};
	
	//local player id
	public string local_player_id;
	
	//store localPlayer
	public GameObject myPlayer;
	
	//store all players in game
	public Dictionary<string, Player2DManager> networkPlayers = new Dictionary<string, Player2DManager>();
	
	ArrayList playersNames;
	
	//store the local players' sprites
	public GameObject[] localPlayersPrefabs;
	

	//store the networkplayers' sprites
	public GameObject[] networkPlayerPrefabs;
	
	public GameObject txtPlayerNamePref;
	
	//stores the spawn points 
	public Transform[] spawnPoints;
	
	public Camera2DFollow cameraFollow;
	
	public bool isGameOver;
	
	int index;
	
	
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
		
		
		playersNames = new ArrayList();
		
	
		
	 }
	 else
	 {
		//it destroys the class if already other class exists
		Destroy(this.gameObject);
	 }
		
	}
	

	/// <summary>
	///  receives an answer of the server.
	/// from  void OnReceivePing(string [] pack,IPEndPoint anyIP ) in server
	/// </summary>
	public void OnPrintPongMsg()
	{

		/*
		 * data.pack[0]= CALLBACK_NAME: "PONG"
		 * data.pack[1]= "pong!!!!"
		*/

		Debug.Log("receive pong");
		
	}

	// <summary>
	/// sends ping message to UDPServer.
	///     case "PING":
	///     OnReceivePing(pack,anyIP);
	///     break;
	/// take a look in TicTacttoeServer.cs script
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
	

	/// <summary>
	/// Emits the CREATE_ROOM event to Server.
	/// method called by the Create button present in Create room panel
	/// take a look in Server script
	/// </summary>
	public void EmitCreateRoom()
	{
	 
	  //hash table <key, value>	
	  Dictionary<string, string> data = new Dictionary<string, string>();


	  data["map"] = ShooterCanvasManager.instance.currentMap;

      data["max_players"] = ShooterCanvasManager.instance.maxPlayers.ToString();

      data["isPrivateRoom"] = ShooterCanvasManager.instance. isPrivateRoom.ToString();

	  //player's name	
	  data["name"] = ShooterCanvasManager.instance.inputLogin.text;

      //player's character  
	  data["avatar"] = ButtonChooseManager.instance.currentAvatar.ToString();
		 
     //makes the draw of a point for the player to be spawn
	  index = UnityEngine.Random.Range (0, spawnPoints.Length);
        
	  //player spawn x position
      data["dx"] = spawnPoints[index].position.x.ToString();
         
	  //player spawn y position
	  data["dy"] = spawnPoints[index].position.y.ToString();
			  
	 //sends to the nodejs server through socket the json package
	  Application.ExternalCall("socket.emit", "CREATE_ROOM",new JSONObject(data));

	
	}

	/// <summary>
	/// Raises the create room event from Server.
	/// </summary>
	/// <param name="data">Data.</param>
	void OnCreateRoom(string data)
	{
	
	    /*
		 * pack[0] = id (local player id)
		 * pack[1]= current_players 
		 * pack[2]= max_players 
		 * pack[3]= map
		
		*/
		try
		{
		  Debug.Log ("\n joining ...\n");
		

		  Debug.Log("Room created, joining room");

			
		 var pack = data.Split (Delimiter);


		  ShooterCanvasManager.instance.OpenScreen("lobby_room");

		  ShooterCanvasManager.instance.SetUpRoom( pack[0],pack[1],pack[2],pack[3]);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
		

		
	

	}


	/// <summary>
	/// Raises the join game event from Server.
	/// only the first player to connect gets this feedback from the server
	/// </summary>
	/// <param name="data">Data.</param>
	void OnOpenLobbyRoom(string data)
	{
	
	   /*
		 * pack[0] = id (local player id)
		 * pack[1]= current_players 
		 * pack[2]= max_players 
		 * pack[3]= map
		
		*/
		try
		{
		Debug.Log ("\n joining ...\n");
	
		var pack = data.Split (Delimiter);

		ShooterCanvasManager.instance.OpenScreen("lobby_room");

		ShooterCanvasManager.instance.SetUpRoom( pack[0],pack[1],pack[2],pack[3]);

		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}

	}

	/// <summary>
	/// Raises the  event from Server.
	/// </summary>
	/// <param name="data">Data.</param>
	void OnUpdateCurrentPlayers(string data)
	{
	
	 /*
	  * pack[0] = current_players	 	
	*/
	 try
	  {
		Debug.Log ("\n updating current players ...\n");

		var pack = data.Split (Delimiter);

		ShooterCanvasManager.instance.UpdateCurrentPlayers(pack[0]);

		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}

	}


     /// <summary>
	/// method called by BtnStart game.
	/// Emits the START_GAME event to Server.
	/// </summary>
	/// <param name="data">Data.</param>
	public void EmitStartGame()
	{
	  //sends to the nodejs server through socket the json package
	  Application.ExternalCall("socket.emit", "START_GAME");
	  
	}

    /// <summary>
	/// Raises the can start game event from Server
	/// </summary>
	void OnCanStartGame()
	{
		try{
	 	 
		  ShooterCanvasManager.instance.btnStartGame.enabled = true;
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
	

	/// <summary>
	/// Emits the join game to Server.
	/// case "JOIN_GAME":
	///   OnReceiveJoinGame(pack,anyIP);
	///  break;
	/// take a look in Server.cs script
	/// </summary>
	public void EmitJoinGame(string roomID)
	{
	 
	  //hash table <key, value>	
	  Dictionary<string, string> data = new Dictionary<string, string>();

		
		//send the position point to server
		string msg = string.Empty;


		 data["roomID"] = roomID;

		
		 //player's name	
		 data["name"] = ShooterCanvasManager.instance.inputLogin.text;

			  
	     data["avatar"] = ButtonChooseManager.instance.currentAvatar.ToString();
		 
		 //makes the draw of a point for the player to be spawn
		 index = UnityEngine.Random.Range (0, spawnPoints.Length);

		 data["dx"] = spawnPoints[index].position.x.ToString();
			  
		
		  //sends to the nodejs server through socket the json package
	     Application.ExternalCall("socket.emit", "JOIN_ROOM",new JSONObject(data));

		Debug.Log("join room sended");

	}


	/// <summary>
	/// Raises the start game event from Server.
	/// </summary>
	/// <param name="data">Data.</param>
	void OnStartGame(string data)
	{
	
	    /*
		 * pack[0] = id (local player id)
		 * pack[1]= name (local player name)
		 * pack[2]= avatar 
		
		 
		*/
		try{

		Debug.Log ("\n starting game ...\n");

		ShooterCanvasManager.instance.txtLog.text = "\n press E key to attack\n";
		
		var pack = data.Split (Delimiter);
		
		


		if (!myPlayer) {
		
		Debug.Log ("a");
		
			// take a look in Player2DManager.cs script
			Player2DManager newPlayer;
			
			// newPlayer = GameObject.Instantiate( local player avatar or model, spawn position, spawn rotation)
			newPlayer = GameObject.Instantiate (localPlayersPrefabs [int.Parse(pack [2])],spawnPoints[index].position,
			Quaternion.identity).GetComponent<Player2DManager> ();
			
			Debug.Log ("b ");
			 
			newPlayer.id = pack[0];
		
			
			newPlayer.name = pack[1];
			
			newPlayer.avatar = pack[2];
			
			Debug.Log ("e");

			//this is local player
			newPlayer.isLocalPlayer = true;

			//now local player online in the arena
			newPlayer.isOnline = true;
			
			newPlayer.gameObject.name = pack[0];
			  
			//puts the local player on the list
			networkPlayers [ pack[0]] = newPlayer;

			myPlayer = networkPlayers [ pack[0]].gameObject;
			
			Debug.Log ("f");
			
			
			
			cameraFollow.SetTarget(newPlayer.gameObject.transform);
			
			
			ShooterCanvasManager.instance.localPlayerImg.sprite =  ShooterCanvasManager.instance.
			spriteFacesPref[int.Parse(pack[2])].GetComponent<SpriteRenderer> ().sprite;
			
		    ShooterCanvasManager.instance.txtLocalPlayerName.text = pack[1];
			
		    ShooterCanvasManager.instance.txtLocalPlayerHealth.text = "100";
			
			ShooterCanvasManager.instance.OpenScreen("game");
			
			GameObject txtName = GameObject.Instantiate (txtPlayerNamePref,new Vector3(0f,0f,-0.1f), Quaternion.identity) as GameObject;
			
			txtName.name = pack[1];
			txtName.GetComponent<PlayerName> ().setName (pack[1]);
			txtName.GetComponent<Follow> ().SetTarget (newPlayer.gameObject.transform);
			
			playersNames.Add(txtName);
			
			Debug.Log("player instantiated");

			
			Debug.Log("player in game");
		}
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}

	}
	
	/// <summary>
	/// Raises the spawn network players event from server.
	/// </summary>
	void OnSpawnPlayer(string data)
	{
	
	   /*
		 * pack[0] = id
		 * pack[1] = name
		 * pack[2]= avatar
		 * pack[3] = dx
		*/

		try{
		
		Debug.Log ("\n spawning network player ...\n");
		
		//ShooterCanvasManager.instance.txtLog.text = "\n spawning network player ...\n";
	
		var pack = data.Split (Delimiter);


			bool alreadyExist = false;

			
			if(networkPlayers.ContainsKey(pack [0]))
			{
			  alreadyExist = true;
			}
			if (!alreadyExist) {
			
			 //  ShooterCanvasManager.instance.txtLog.text = "creating a new player";

				Debug.Log("creating a new player: "+pack[1]);

				Player2DManager newPlayer;
		
				// newPlayer = GameObject.Instantiate( network player avatar or model, spawn position, spawn rotation)
				newPlayer =  GameObject.Instantiate (networkPlayerPrefabs [int.Parse(pack[2])],new Vector3(float.Parse(pack [3]),-3f,0),
			  Quaternion.identity).GetComponent<Player2DManager> ();
			  
			  
			 // ShooterCanvasManager.instance.txtLog.text = "player spawned";
			  
			 

				//it is not the local player
				newPlayer.isLocalPlayer = false;

				//network player online in the arena
				newPlayer.isOnline = true;

				newPlayer.gameObject.name = pack [0];
				
				newPlayer.name = pack[1];
				
				newPlayer.avatar = pack[2];

				//puts the local player on the list
				networkPlayers [pack [0]] = newPlayer;
					
			
				GameObject txtName = GameObject.Instantiate (txtPlayerNamePref,new Vector3(0f,0f,-0.1f), Quaternion.identity) as GameObject;
				txtName.name = pack[1];
				txtName.GetComponent<PlayerName> ().setName (pack[1]);
				txtName.GetComponent<Follow> ().SetTarget (newPlayer.gameObject.transform);
				
				playersNames.Add(txtName);
				
				// ShooterCanvasManager.instance.txtLog.text = "player in game";
				
			}
			
			}
			catch(Exception e)
			{
			   Debug.Log(e.ToString());
			   ShooterCanvasManager.instance.txtLog.text = e.ToString();
			}

	}

	
	public void EmitChangeAvatar()
	{
	 
	  //hash table <key, value>	
	  Dictionary<string, string> data = new Dictionary<string, string>();

	
	  data["avatar"] = ButtonChooseManager.instance.currentAvatar.ToString();
	
	  //sends to the nodejs server through socket the json package
	  Application.ExternalCall("socket.emit", "CHANGE_AVATAR",new JSONObject(data));
			
	}

	/// <summary>
	/// Gets the rooms from server
	/// </summary>
	public void GetRooms(string _map)
	{

		ShooterCanvasManager.instance.OpenScreen("roomList");

		ShooterCanvasManager.instance.ClearRooms();

		//hash table <key, value>	
	    Dictionary<string, string> data = new Dictionary<string, string>();

	    data["map"] = _map;

	  //sends to the nodejs server through socket the json package
	  Application.ExternalCall("socket.emit","GET_ROOMS",new JSONObject(data));

	}

	

	public void OnReceiveRooms(string data)
	{
	
	    /*
		 * pack[0] = id (room id)
		 * pack[1]= name
		 * pack[2]= current_players 
		 * pack[3]= max_players 
		
		*/
	  try{
	   Debug.Log("roons received");
	   
	 var pack = data.Split (Delimiter);


	   ShooterCanvasManager.instance.SpawnRoom(pack[0],pack[1],pack[2],pack[3]);
    }
	catch(Exception e)
	{
		Debug.Log(e.ToString());
		
	}


  }






////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////PLAYER POSITION AND ROTATION UPDATES///////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//send local player position and rotation to server
	public void EmitPosAndRot(Dictionary<string, string> data)
	{
	    JSONObject jo = new JSONObject (data);

	  //sends to the nodejs server through socket the json package
	  Application.ExternalCall("socket.emit","POS_AND_ROT",new JSONObject(data));

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
		 * pack[2] = dy
		 * pack[3] = rotation
		*/
		
		try{
			
		var pack = data.Split (Delimiter);

		if (networkPlayers [pack [0]] != null)
		{
		  
			Player2DManager netPlayer = networkPlayers[pack [0]];
		
			//update with the new position
			netPlayer.UpdatePosition(float.Parse(pack [1]),float.Parse(pack [2]));
	        //update new player rotation
			netPlayer.UpdateRotation(pack[3]);
			

		}
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////ANIMATION UPDATES/////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Emits the local player animation to Server.js.
	/// </summary>
	/// <param name="_animation">Animation.</param>
	public void EmitAnimation(string _animation)
	{
		
		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data["local_player_id"] = myPlayer.GetComponent<Player2DManager>().id;

		data ["animation"] = _animation;
	
	    //sends to the nodejs server through socket the json package
	   Application.ExternalCall("socket.emit","ANIMATION",new JSONObject(data));

	}

	/// <summary>
	///  Update the network player animation to local player.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnUpdateAnim(string data)
	{
		
		/*
		 * data.pack[0] = id
		 * data.pack[1] = animation
		*/
		
		var pack = data.Split (Delimiter);
		
		//find network player by your id
		Player2DManager netPlayer = networkPlayers[pack[0]];

		//updates current animation
		netPlayer.UpdateAnimator(pack[1]);

	}
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////DAMAGE UPDATES/////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  public void EmitPlayerDamage(string _networkPlayerID)
  {
  
    Debug.Log("emit damage");
    
	//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data ["id"] = _networkPlayerID;

		
		 //sends to the nodejs server through socket the json package
	   Application.ExternalCall("socket.emit","DAMAGE",new JSONObject(data));

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
		var pack = data.Split (Delimiter);
		
		
		//find network player by your id
		Player2DManager netPlayer = networkPlayers[pack[0]];

		if(networkPlayers[pack[0]].isLocalPlayer)
		{
		    ShooterCanvasManager.instance.txtLocalPlayerHealth.text = pack[1];
		}
		
		//updates current animation
		netPlayer.UpdateAnimator("OnDamage");

	}
	
	
	  /// <summary>
	///  Update the network player animation to local player.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnGameOver(string data)
	{
	
	    /*
		 * data.data.pack[0] = id
	
		*/
		
		
		Debug.Log("receive game over");
		
		try{
		
		var pack = data.Split (Delimiter);
		
		
		//find network player by your id
		Player2DManager netPlayer = networkPlayers[pack[0]];
		
		 GameObject name = null; 
		 
		 foreach(GameObject pn in playersNames)
		 {
		     if(pn!=null && pn.name.Equals(netPlayer.name))
		     {
			 
			  Destroy(pn);
		     }
		  }
		  
		

		if(networkPlayers[pack[0]].isLocalPlayer)
		{
			Debug.Log("reset game");
		    ResetGame(); 
		}
		else
		{
		
		  Debug.Log("just destroy player");
		    
		  isGameOver = true;
		
		  Destroy (networkPlayers [pack[0]].gameObject);
		  
		  networkPlayers.Remove (pack[0]);
				
		
		}
		}
		catch(Exception e)
		{
		 Debug.Log(e.ToString());
		}
		
	
	}
	
	void ResetGame()
	{
	    Debug.Log("reset game");
		
		myPlayer = null;
		
		
		foreach(GameObject name in playersNames)
		{
		  Destroy(name);
		}
		
		playersNames.Clear();
		
		//send answer in broadcast
		foreach (KeyValuePair<string, Player2DManager> entry in networkPlayers) {

		  Destroy(entry.Value.gameObject);
		  
		}//END_FOREACH
		
		networkPlayers.Clear();
		
		ShooterCanvasManager.instance.txtLocalPlayerHealth.text = "100";
	
		ShooterCanvasManager.instance.OpenScreen("main_menu");
		
		
	}
	
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////




	void OnApplicationQuit() {

		Debug.Log("Application ending after " + Time.time + " seconds");

	}
	
	/// <summary>
	/// inform the local player to destroy offline network player
	/// </summary>
	/// <param name="_msg">Message.</param>
	//disconnect network player
	void OnUserDisconnected(string data )
	{


		var pack = data.Split (Delimiter);
		
		Debug.Log("user: "+networkPlayers[pack[0]].name+" disconnected");
  
		GameObject name = null; 
	 
		 foreach(GameObject pn in playersNames)
		 {
		     if(pn.name.Equals(networkPlayers[pack[0]].name))
		     {
			 
			  Destroy(pn);
		     }
		  }
		  
		
		if (networkPlayers [pack [0]] != null)
		{


			//destroy network player by your id
			Destroy( networkPlayers[pack[0]].gameObject);


			//remove from the dictionary
			networkPlayers.Remove(pack[0]);

		}
		
		


	}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////HELPERS////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	Vector3 StringToVector3(string target ){

		Vector3 newVector;
		string[] newString = Regex.Split(target,";");
		newVector = new Vector3( float.Parse(newString[0]), float.Parse(newString[1]),float.Parse(newString[2]));

		return newVector;

	}
	
	Vector4 StringToVector4(string target ){

		Vector4 newVector;
		string[] newString = Regex.Split(target,";");
		newVector = new Vector4( float.Parse(newString[0]), float.Parse(newString[1]),float.Parse(newString[2]),float.Parse(newString[3]));

		return newVector;

	}
	
	string Vector3ToString(Vector3 vet ){

		return  vet.x+";"+vet.y+";"+vet.z;

	}
	
	string Vector4ToString(Vector4 vet ){

		return  vet.x+";"+vet.y+";"+vet.z+";"+vet.w;

	}
	
	
	Vector3 JsonToVector3(string target ){

	   
		Vector3 newVector = new Vector3(0,0,0);
		string[] newString = Regex.Split(target,";");
		//#if UNITY_EDITOR 
		newVector = new Vector3( float.Parse(newString[0]), float.Parse(newString[1]),float.Parse(newString[2]));
		
        /*#else
		
		CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
         ci.NumberFormat.CurrencyDecimalSeparator = ",";
		 new Vector3( float.Parse(newString[0],NumberStyles.Any,ci), float.Parse(newString[1],NumberStyles.Any,ci),float.Parse(newString[2],NumberStyles.Any,ci));
	
        #endif
 */
		return newVector;

	}

	Vector4 JsonToVector4(string target ){

		Vector4 newVector = new Vector4(0,0,0,0);
		string[] newString = Regex.Split(target,";");
	//	#if UNITY_EDITOR 
		newVector = new Vector4( float.Parse(newString[0]), float.Parse(newString[1]), float.Parse(newString[2]),float.Parse(newString[3]));
	
     /*   #else
		
		CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
         ci.NumberFormat.CurrencyDecimalSeparator = ",";
		newVector = new Vector4( float.Parse(newString[0],NumberStyles.Any,ci), float.Parse(newString[1],NumberStyles.Any,ci),float.Parse(newString[2],NumberStyles.Any,ci
		),float.Parse(newString[3],NumberStyles.Any,ci));
        #endif
		*/
		return newVector;

	}
	

	
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


	
		
}
}//EN_NAMESPACE