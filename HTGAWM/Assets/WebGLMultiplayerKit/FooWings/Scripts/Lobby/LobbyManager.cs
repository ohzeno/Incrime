using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerShooter
{
public class LobbyManager : MonoBehaviour
{
    
	public static LobbyManager  instance;

	[Header("Lobby Room")]
	public Canvas  HUDLobby; //variable that stores the HUDLobby panel
	 
	[Header("Game Canvas")]
	public Canvas  gameCanvas; //variable that stores the Game Canvas panel
	
	public string currentMenu;
    
	[Header("Input Login")]
	public InputField inputLogin;

	public  AudioClip buttonSound;

	 public void Awake()
    {
        
		inputLogin.text = "Player " + UnityEngine.Random.Range(1000, 10000);

    }


   
	
	// Use this for initialization
	void Start () {

		if (instance == null) {

			instance = this;

			OpenScreen("hudlobby");

			
		}
		else
		{
			Destroy(this.gameObject);
		}



	}


	/// <summary>
	/// opens a selected screen.
	/// </summary>
	/// <param name="_current">Current screen</param>
	public void  OpenScreen(string _current)
	{
		switch (_current)
		{
			  //lobby menu
		    case "hudlobby":
			currentMenu = _current;
			HUDLobby.enabled = true;
			gameCanvas.enabled = false;
		
			break;

		    case "gamecanvas":
			currentMenu = _current;
			HUDLobby.enabled = false;
			gameCanvas.enabled = true;
		
		
			break;
	
		}

	}


	public void PlayAudio(AudioClip _audioclip)
	{
		
	   GetComponent<AudioSource> ().PlayOneShot (_audioclip);

	}

	public void PlayButtonSound()
	{
		
	   GetComponent<AudioSource> ().PlayOneShot (buttonSound);

	}

	
}//END_CLASS
}//END_NAMESPACE