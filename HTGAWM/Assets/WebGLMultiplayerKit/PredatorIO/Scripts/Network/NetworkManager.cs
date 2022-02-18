using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;


namespace SavanaIO
{
public class NetworkManager : MonoBehaviour
{
   
	//useful for any gameObject to access this class without the need of instances her or you declare her
	public static NetworkManager instance;
	
	//Variable that defines comma character as separator
	static private readonly char[] Delimiter = new char[] {':'};

	//flag which is determined the player is logged in the arena
	public bool onLogged = false;

	//local player id
	public string myId = string.Empty;
	
	//local player id
	public string local_player_id;
	
	//store localPlayer
	public GameObject myPlayer;
	
	//store all players in game
	public Dictionary<string, Player2DManager> networkPlayers = new Dictionary<string, Player2DManager>();
	
	ArrayList playersNames;
	
	//store the local players' models
	public GameObject playerPrefab;
	
	public GameObject txtPlayerNamePref;
	
	//stores the spawn points 
	public Transform[] spawnPoints;
	
	public Camera2DFollow cameraFollow;
	
	public bool isGameOver;
	
	int index;

	public bool isMasterClient;

	
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
	
	
	

	void Update(){}



	/// <summary>
	///  receives an answer of the server.
	/// from  void OnReceivePing(string [] pack,IPEndPoint anyIP ) in server
	/// </summary>
	public void OnPrintPongMsg(string data)
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

		if (!isGameOver) {


		
		 //player's name	
		 data["name"] = CanvasManager.instance.inputLogin.text;
		  
		if(CanvasManager.instance.inputLogin.text.Equals(string.Empty))
		{
			int rand = UnityEngine.Random.Range (0, 999);
			data["name"] = "guess"+rand;
		}
			  
	     data["avatar"] = "0";
		 
		 //makes the draw of a point for the player to be spawn
		 index = UnityEngine.Random.Range (0, spawnPoints.Length);

		 data["position"] = spawnPoints[index].position.x.ToString()+":"+spawnPoints[index].position.y.ToString();
			  
			
		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", "JOIN_ROOM",new JSONObject(data));
		

		
		}
		else
		{
			
		   isGameOver = false;
		   
			 //player's name	
		    data["name"] = CanvasManager.instance.inputLogin.text;
			  
	        data["avatar"] = "0";
		 
		    //makes the draw of a point for the player to be spawn
		    index = UnityEngine.Random.Range (0, spawnPoints.Length);

		    data["position"] = spawnPoints[index].position.x.ToString()+":"+spawnPoints[index].position.y.ToString();
		
			
			
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

		try{
		Debug.Log ("\n joining ...\n");
		
		
		/*
		 * pack[0] = id (local player id)
		 * pack[1]= name (local player name)
		 * pack[2] = avatar
		 * pack[3] = isMasterClient

		*/

		Debug.Log("Login successful, joining game");
		
		var pack = data.Split (Delimiter);

		
		Debug.Log("Login successful, joining game");


		if (!myPlayer) {
		
		
		  

			// take a look in Player2DManager.cs script
			Player2DManager newPlayer;
			
			// newPlayer = GameObject.Instantiate( local player avatar or model, spawn position, spawn rotation)
			newPlayer = GameObject.Instantiate (playerPrefab,spawnPoints[index].position,
			Quaternion.identity).GetComponent<Player2DManager> ();

			Debug.Log("player instantiated");
			 
			newPlayer.id = pack[0];
			
			newPlayer.name = pack[1];
			
			newPlayer.avatar = pack[2];

			//this is local player
			newPlayer.isLocalPlayer = true;

			//now local player online in the arena
			newPlayer.isOnline = true;
			
			newPlayer.gameObject.name = pack[0];
			  
			//puts the local player on the list
			networkPlayers [ pack[0]] = newPlayer;

			myPlayer = networkPlayers [ pack[0]].gameObject;
			
			cameraFollow.SetTarget(newPlayer.gameObject.transform);
			
			isMasterClient = bool.Parse(pack[3]);
			Debug.Log("isMasterClient: "+isMasterClient);
	
			CanvasManager.instance.OpenScreen(1);
			  	
			Debug.Log("player in game");
		}
		}//END_TRY
		catch(Exception e)
		{
			Debug.LogError(e.ToString());
		}

	}
	
	/// <summary>
	/// Raises the spawn player event.
	/// </summary>
	/// <param name="_data">package received from server.</param>
	void OnSpawnPlayer(string data)
	{

		/*
		 * pack[0] = id (network player id)
		 * pack[1]= name
		 * pack[2]= avatar
		 * pack[3] = position
		*/
		
		try{
		
		//Debug.Log ("\n spawning network player ...\n");
		
		//CanvasManager.instance.txtLog.text = "\n spawning network player ...\n";
	
		var pack = data.Split (Delimiter);


			bool alreadyExist = false;

			
			if(networkPlayers.ContainsKey(pack [0]))
			{
			  alreadyExist = true;
			}
			if (!alreadyExist) {
			
			 //CanvasManager.instance.txtLog.text = "creating a new player";

				//Debug.Log("creating a new player");
				
			

				Player2DManager newPlayer;

				Vector2 _pos = new Vector2(StringToFloat(pack[3]),StringToFloat(pack[4]));
			
			
				// newPlayer = GameObject.Instantiate( network player avatar or model, spawn position, spawn rotation)
				newPlayer =  GameObject.Instantiate (playerPrefab,new Vector3(_pos.x,_pos.y,0),
			  Quaternion.identity).GetComponent<Player2DManager> ();
			  
			  
				//it is not the local player
				newPlayer.isLocalPlayer = false;

				//network player online in the arena
				newPlayer.isOnline = true;

				newPlayer.id = pack[0];

				newPlayer.gameObject.name = pack [0];
				
				newPlayer.name = pack[1];
				
				newPlayer.avatar = pack[2];

				newPlayer.Evolution (int.Parse(pack [2]));

				//puts the local player on the list
				networkPlayers [pack [0]] = newPlayer;
					
			
				GameObject txtName = GameObject.Instantiate (txtPlayerNamePref,new Vector3(0f,0f,-0.1f), Quaternion.identity) as GameObject;
				txtName.name = pack[1];
				txtName.GetComponent<PlayerName> ().setName (pack[1]);
				txtName.GetComponent<Follow> ().SetTarget (newPlayer.gameObject.transform);
				
				playersNames.Add(txtName);
				
				//CanvasManager.instance.txtLog.text = "player in game";
				// Debug.Log("player in game");
			}
			
			}
			catch(Exception e)
			{
			   Debug.Log(e.ToString());
			   CanvasManager.instance.txtLog.text = e.ToString();
			}

	}

	/// <summary>
	/// Raises the spawn powerUp event.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnSpawnPowerUp(string data)
	{
	   /*
		 * pack[0] = id 
		 * pack[1]= type
		 * pack[2]= posx
		 * pack[3] = posy
	
		*/

		var pack = data.Split (Delimiter);

		PowerUpManager.instance.SpawnPowerUp(pack[0],int.Parse(pack[1]),StringToFloat(pack[2]),StringToFloat(pack[3]));
	}

	public void EmitPickUpItem(string _id)
	{
		 //hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data ["id"] = _id;
		
		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", "PICKUP",new JSONObject(data));
		
	}

		/// <summary>
	/// Raises the spawn powerUp event.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnUpdatePickup(string data)
	{
	  /*
		 * pack[0] = player_id
		 * pack[1]= powerUp id
		 
		*/
	
		var pack = data.Split (Delimiter);
		
		PowerUpManager.instance.Pickup(pack[1]);

		if (networkPlayers.ContainsKey(pack [0]))
		{

			if (networkPlayers[pack [0]].isLocalPlayer)// if i'm a target
			{
			    CanvasManager.instance.xpSlider.value += 3; 
				 CanvasManager.instance.PlayPickUpAudioClip();
			}



		}
	}

	

	/// <summary>
	/// Raises the spawn powerUp event.
	/// </summary>
	/// <param name="data">Message received from server.</param>
	void OnUpdateEvolution(string data)
	{
	   /*
		 * pack[0] = player_id
		 * pack[1]= avatar
		 
		*/

		var pack = data.Split (Delimiter);
		
		if (networkPlayers.ContainsKey(pack [0]))
		{
            
			Player2DManager current_player = networkPlayers[pack [0]];
			current_player.Evolution (int.Parse(pack [1]));
		}

	}


	public void EmitRegression()
	{
		 //hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data ["avatar"] = myPlayer.GetComponent<Player2DManager>().avatar;
		
		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", "REGRESSION",new JSONObject(data));
		

	}

		/// <summary>
	/// Raises the spawn powerUp event.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnUpdateRegression(string data)
	{
          /*
		 * pack[0] = player_id
		 * pack[1]= avatar
		 
		*/
		var pack = data.Split (Delimiter);


		if (networkPlayers.ContainsKey(pack [0]))
		{

			Player2DManager current_player = networkPlayers[pack [0]];
			current_player.Regression (int.Parse(pack [1]));
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
		Application.ExternalCall("socket.emit", "POS_AND_ROT",new JSONObject(data));
		

	}

	/// <summary>
	/// Update the network player position and rotation to local player.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnUpdatePosAndRot(string data)
	{
		/*
		 * data.pack[0] = id (network player id)
		 * data.pack[1] = "position.x"
		 * data.pack[2] = "position.y"
		 * data.pack[2] = rotation.x
		 * data.pack[3] = rotation.y
		 * data.pack[4] = rotation.z
	     * data.pack[5] = rotation.w
		*/
		
	     var pack = data.Split (Delimiter);

	
		if (networkPlayers.ContainsKey(pack [0]))
		{

		  
			Player2DManager netPlayer = networkPlayers[pack [0]];
			
			Vector2 pos = new Vector2(StringToFloat(pack[1]),StringToFloat(pack[2]));
			
		
			//update with the new position
			netPlayer.UpdatePosition( pos);
	        //update new player rotation
			Vector4 rot  = new Vector4(StringToFloat(pack[3]),StringToFloat(pack[4]),StringToFloat(pack[5]),StringToFloat(pack[6]));
			netPlayer.UpdateRotation(new Quaternion (rot.x,rot.y,rot.z,rot.w));
			

		}
	}
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////



////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////BEST KILLERS UPDATES/////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


public void EmitGetBestKillers()
{
			
    //sends to the nodejs server through socket the json package
	Application.ExternalCall("socket.emit", "GET_BEST_KILLERS");
		
	
}

void OnClearLeaderBoard()
{
	CanvasManager.instance.ClearLeaderBoard();
}
	
void OnUpdateBestKiller(string data)
{
	    
	    /*
		 * pack[0] = name
		 * pack[1] = ranking
		 * pack[2] = kills
		*/
			
		var pack = data.Split (Delimiter);

	
		//Debug.Log("received best players from server ...");
	
		int ranking = int.Parse (pack[1]) + 1;
		
		CanvasManager.instance.SetUpBestKiller(pack[0], pack[2], ranking.ToString());

}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////





////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////DAMAGE UPDATES/////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  public void EmitPlayerDamage(string _networkPlayerID)
  {
  
   
	   //hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data ["id"] = _networkPlayerID;

		
		//sends to the nodejs server through socket the json package
		Application.ExternalCall("socket.emit", "PLAYER_DAMAGE",new JSONObject(data));
		

  }
  
  
	
	
	  /// <summary>
	///  Update the network player animation to local player.
	/// </summary>
	/// <param name="_msg">Message.</param>
	void OnGameOver(string data)
	{
		 /*
		 * data.data.pack[0] = looser player id
		
		*/
		
		try{
		
	
		var pack = data.Split (Delimiter);
		
		if (networkPlayers.ContainsKey(pack [0]))
		{
				
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
		    ResetGame(); 
		  }
		  else
		  {
		    
		 
		   Destroy (networkPlayers [pack[0]].gameObject);
		  
		   networkPlayers.Remove (pack[0]);
				
		  }
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

		isGameOver = true;
		
		myPlayer = null;

		PowerUpManager.instance.Clear();
		
		
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
		
	
		CanvasManager.instance.OpenScreen(0);
		
		
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
	//desconnect network player
	void OnUserDisconnected(string data)
	{

		/*
		 * data.pack[0] = id (network player id)
		*/

		var pack = data.Split (Delimiter);
		
		//Debug.Log("user: "+networkPlayers[pack[0]].name+" disconnected");
  
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


	 
	public  float StringToFloat(string target ){

		float result;
		
		if(target.ToLower().Contains(","))
		 {
		   CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ",";
			
			result = float.Parse (target,NumberStyles.Any,ci);
			
		 }
		 else
		 {
		   CultureInfo ci2 = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci2.NumberFormat.CurrencyDecimalSeparator = ".";
			
			result = float.Parse (target,NumberStyles.Any,ci2);
			
		 }

		return result;

	}

	

	
	
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
}//END_CLASS
}//END_NAMESPACE
