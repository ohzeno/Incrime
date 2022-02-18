using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MultiverseRiders
{
public class ShooterCanvasManager : MonoBehaviour
{
    
	public static ShooterCanvasManager  instance;

	public Canvas  mainPanel;

	public Canvas  lobbyRoom;

	public Canvas  createRoomPanel;

	public Canvas  roomListPanel;
	
	public Canvas  gameCanvas;

	public Canvas btnStartGame;
	
	public Canvas mobileButtons;
	
	public Canvas alertgameDialog;

	public Text alertDialogText;

	public Text messageText;
	
	public Text txtLog;
	
	public string currentMenu;

	public  AudioClip buttonSound;
	
	public InputField inputLogin;

	public InputField inputPrivateRoomID;
	
	public GameObject[] spriteFacesPref;
	
	public Image localPlayerImg;
	
	public Text txtLocalPlayerName;
	
	public Text txtLocalPlayerHealth;

	public Text txtRoomID;

	public Button roomType;

	public Text txtCurrentRoomPlayers;

	public Text txtMaxRoomPlayers;

	public bool CanStartGame;

	public GameObject contentRooms;

	public GameObject roomPrefab;

	public GameObject btnMap1;

	public GameObject btnMap2;

	public GameObject btnMap3;

	public GameObject[] avaliableMaps;

	public GameObject[] maxPlayersBtns;

	public GameObject btnPrivateRoom;

	public GameObject activeBtnSprite;

	public GameObject desabledBtnSprite;

	public string currentMap;

	public int maxPlayers;

	public bool isPrivateRoom;
	
	public bool enabledMobileBtns;
	
	
	
	public float delay = 0f;

	ArrayList rooms;
	
	
	

	
	// Use this for initialization
	void Start () {

		if (instance == null) {

			DontDestroyOnLoad (this.gameObject);

			instance = this;

			rooms = new ArrayList ();
			
			OpenScreen("main_menu");

			CloseAlertDialog ();

			SetMap("map1");
			SetMaxPlayers(1);
			
		}
		else
		{
			Destroy(this.gameObject);
		}



	}

	void Update()
	{
		delay += Time.deltaTime;

		if (Input.GetKey ("escape") && delay > 1f) {

		  switch (currentMenu) {

			case "main_menu":
			 delay = 0f;
			 Application.Quit ();
			break;

		

		 }//END_SWITCH

	 }//END_IF
}
	/// <summary>
	/// Opens the screen.
	/// </summary>
	/// <param name="_current">Current.</param>
	public void  OpenScreen(string _current)
	{
		switch (_current)
		{
		    //lobby menu
		    case "main_menu":
			currentMenu = _current;
			mainPanel.enabled = true;
			lobbyRoom.enabled = false;
			createRoomPanel.enabled = false;
			roomListPanel.enabled = false;
			gameCanvas.enabled = false;
			break;

			case "lobby_room":
			currentMenu = _current;
			mainPanel.enabled = false;
			lobbyRoom.enabled = true;
			createRoomPanel.enabled = false;
			roomListPanel.enabled = false;
			gameCanvas.enabled = false;
			break;


		    case "roomList":
			currentMenu = _current;
			mainPanel.enabled = false;
			lobbyRoom.enabled = false;
			createRoomPanel.enabled =false;
			roomListPanel.enabled = true;
			gameCanvas.enabled = false;
		
			break;
			 case "create_room":
			currentMenu = _current;
			mainPanel.enabled = false;
			lobbyRoom.enabled = false;
			createRoomPanel.enabled = true;
			roomListPanel.enabled = false;
			gameCanvas.enabled = false;
		
			break;
			case "game":
			currentMenu = _current;
			mainPanel.enabled = false;
			lobbyRoom.enabled = false;
			createRoomPanel.enabled = false;
			roomListPanel.enabled = false;
			gameCanvas.enabled = true;
		
			break;

	
		}

	}

	public void SetPrivateRoom()
	{
		isPrivateRoom =!isPrivateRoom;

		if(isPrivateRoom)
		{
          btnPrivateRoom.GetComponent<Image> ().sprite = activeBtnSprite.GetComponent<SpriteRenderer> ().sprite;
		
		}
		else
		{
          btnPrivateRoom.GetComponent<Image> ().sprite = desabledBtnSprite.GetComponent<SpriteRenderer> ().sprite;
		}

		

	}


	public void SetMap(string _map)
	{
		currentMap = _map;
		switch (_map)
		{
		    
		    case "map1":
			
			btnMap1.GetComponent<Image> ().sprite = activeBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			btnMap2.GetComponent<Image> ().sprite = desabledBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			btnMap3.GetComponent<Image> ().sprite =  desabledBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			break;
			 case "map2":
		
			btnMap1.GetComponent<Image> ().sprite =  desabledBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			btnMap2.GetComponent<Image> ().sprite = activeBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			btnMap3.GetComponent<Image> ().sprite =  desabledBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			break;
			 case "map3":
			
			btnMap1.GetComponent<Image> ().sprite = desabledBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			btnMap2.GetComponent<Image> ().sprite = desabledBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			btnMap3.GetComponent<Image> ().sprite =  activeBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			break;
		}
	}

		public void ChooseAvaliableMaps(string _map)
	{
		currentMap = _map;
		ShooterNetworkManager.instance.GetRooms(_map);
		switch (_map)
		{
		    
		    case "map1":
			
			avaliableMaps[0].GetComponent<Image> ().sprite = activeBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			avaliableMaps[1].GetComponent<Image> ().sprite = desabledBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			avaliableMaps[2].GetComponent<Image> ().sprite =  desabledBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			break;
			 case "map2":
		
			avaliableMaps[0].GetComponent<Image> ().sprite =  desabledBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			avaliableMaps[1].GetComponent<Image> ().sprite = activeBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			avaliableMaps[2].GetComponent<Image> ().sprite =  desabledBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			break;
			 case "map3":
			
			avaliableMaps[0].GetComponent<Image> ().sprite = desabledBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			avaliableMaps[1].GetComponent<Image> ().sprite = desabledBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			avaliableMaps[2].GetComponent<Image> ().sprite =  activeBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			break;
		}
	}

	public void SetMaxPlayers(int _max_players)
	{
		maxPlayers =_max_players;

		for(int i =0; i< maxPlayersBtns.Length;i++ )
		{
			int index = _max_players-1;
			if(i.Equals(index))
			{
				maxPlayersBtns[i].GetComponent<Image> ().sprite = activeBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			}
			else
			{
				maxPlayersBtns[i].GetComponent<Image> ().sprite = desabledBtnSprite.GetComponent<SpriteRenderer> ().sprite;
			}
		}
		
	}


	/// <summary>
	/// Shows the alert dialog.
	/// </summary>
	/// <param name="_message">Message.</param>
	public void ShowAlertDialog(string _message)
	{
		alertDialogText.text = _message;
		alertgameDialog.enabled = true;
	}

	
	/// <summary>
	/// Closes the alert dialog.
	/// </summary>
	public void CloseAlertDialog()
	{
		alertgameDialog.enabled = false;
	}
	
		/// <summary>
	/// Shows the alert dialog.Debug.Log
	/// </summary>
	/// <param name="_message">Message.</param>
	public void ShowMessage(string _message)
	{
		messageText.text = _message;
		messageText.enabled = true;
		StartCoroutine (CloseMessage() );//chama corrotina para esperar o player colocar o outro pé no chão
	}
	
	/// <summary>
	/// Closes the alert dialog.
	/// </summary>

	IEnumerator CloseMessage() 
	{

		yield return new WaitForSeconds(4);
		messageText.text = "";
		messageText.enabled = false;
	} 




	public void PlayAudio(AudioClip _audioclip)
	{
		
	   GetComponent<AudioSource> ().PlayOneShot (_audioclip);

	}

	public void SetUpRoom(string room_id, string current_players,string max_players,string map)
	{
	
	  GameManager.instance.SpawnMap(map);
		
	  txtRoomID.text = room_id;
      
	  txtCurrentRoomPlayers.text = current_players;

	  txtMaxRoomPlayers.text = max_players;

	}


	public void UpdateCurrentPlayers( string current_players)
	{
	
	  txtCurrentRoomPlayers.text = current_players;


	}

	/// <summary>
	/// Clears rooms.
	/// </summary>
	public void ClearRooms()
	{
		foreach (GameObject room in rooms)
		{

			Destroy (room.gameObject);
		}

		rooms.Clear ();
	}

	public void SpawnRoom(string id,string name, string current_players, string max_players)
	{
		
       
		GameObject newRoom = Instantiate (roomPrefab) as GameObject;

		newRoom.GetComponent<Room>().id = id;
		newRoom.GetComponent<Room>().txtRoomName.text = name;
		newRoom.GetComponent<Room>().txtPlayers.text = current_players+" / "+max_players;
	
		newRoom.transform.parent = contentRooms.transform;
		newRoom.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);

		rooms.Add (newRoom);
				

	}

	public void JoinToPrivateRoom()
	{
		 ShooterNetworkManager.instance.EmitJoinGame(inputPrivateRoomID.text);
	}
	
	public void SetMobileButtons()
	{
	  enabledMobileBtns=!enabledMobileBtns;
	  if(enabledMobileBtns)
	  {
	    mobileButtons.enabled = true;
	  }
	  else
	  {
	    mobileButtons.enabled = false;
	  }
	}
	
	
}

}
