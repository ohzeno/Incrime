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
namespace ChatSample
{

public class NetworkManager : MonoBehaviour
{

	//useful for any gameObject to access this class without the need of instances her or you declare her
	public static NetworkManager instance;

	//flag which is determined the player is logged in the arena
	public bool onLogged = false;

    string local_player_id;
	
	//store all players in game
	public Dictionary<string, Client> connectedClients = new Dictionary<string, Client>();

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
		
		//player's nickname
		data["name"] = "teste";//CanvasManager.instance.ifLogin.text;

		
		string msg = string.Empty;


		//Identifies with the name "JOIN", the notification to be transmitted to the server
		data["callback_name"] = "JOIN";
			
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
		 * data.pack[2] = " local user avatar index"
		*/
	
	
			var pack = data.Split (Delimiter);
			
		    Debug.Log("Login successful, joining game");

		    // the local player now is logged
		    onLogged = true;
		
			Client client = new Client ();

			client.id = pack [0];//set client id

			client.name = pack [1];//set client name
			
			client.avatar = int.Parse(pack[2]);

			Debug.Log("player instantiated");
			
			//this is local player
			client.isLocalPlayer = true;

			//puts the local player on the list
			connectedClients [client.id] = client;
			
			local_player_id = client.id;

			CanvasManager.instance.SetUpProfile(client.name,client.avatar);
		
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
		 * data.pack[2] = avatar index 
		*/

   
		if (onLogged ) {
			
			var pack = data.Split (Delimiter);

			
			if (!FindPlayer(pack [0])) {

				Client client = new Client ();

			    client.id = pack [0]; // set client id

			    client.name = pack [1]; // set client name
			
			    client.avatar = int.Parse(pack[2]); // set client avatar

			    Debug.Log(" network player instantiated");
			
			    //this is network client
			    client.isLocalPlayer = false;

			    //puts the local player on the list
			    connectedClients [client.id] = client;
				
				CanvasManager.instance.SpawnUser(client.id,client.name,client.avatar);
		

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
	public void EmitMessage()
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		
		string msg = string.Empty;

		//Identifies with the name "MESSAGE", the notification to be transmitted to the server
		data["callback_name"] = "MESSAGE";
		
		data ["id"] = local_player_id;
		
		data ["message"] = CanvasManager.instance.inputFieldMessage.text;
		
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
		 * data.pack[0] = id (network player id)
		 * data.pack[1]= message
		 * data.pack[2] = avatar index
		*/
		
    
		  
		var pack = data.Split (Delimiter);

	
		if (local_player_id.Equals(pack[0])) {
			  
			 CanvasManager.instance.SpawnMyMessage(pack[1]);
			
		}
		else
		{
			CanvasManager.instance.SpawnNetworkMessage(pack[1],int.Parse(pack[2]));
		}
	
			 
	}
	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


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

         CanvasManager.instance.DestroyUser(pack [1]);
		 //remove from the dictionary
		  connectedClients.Remove(pack[1]);
		
		
		

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
			foreach(KeyValuePair<string, Client> entry in connectedClients)
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
