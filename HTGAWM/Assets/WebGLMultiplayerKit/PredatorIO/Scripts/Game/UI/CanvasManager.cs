using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SavanaIO
{
public class CanvasManager : MonoBehaviour
{
    
	public static CanvasManager  instance;
    
	[Header("HUDLobby")]
	public Canvas  HUDLobby;
	
	[Header("Game Canvas")]
	public Canvas  gameCanvas;

    [Header("Leader Board")]
	public Canvas leaderBoard;
	
	[Header("Alert Game Dialog")]
	public Canvas alertgameDialog;

    [Header("Alert Game Dialog Text")]
	public Text alertDialogText;
	
	[Header("Message Text")]
	public Text messageText;
	
	[Header("Text Log")]
	public Text txtLog;
	
	[Header("PickUp Audio Clip")]
	public AudioClip pickupAudioClip;

    [HideInInspector]
	public int currentMenu;

    [Header("Input field Login")]
	public InputField inputLogin;
	
	[HideInInspector]
	public float progress = 100;

   	[Header("Timer Delay UI Time")]
	public float timerDelay = 1f;

	[Header("Timer UI Time")]
	public float timerTime = 0.01f;
	
	[HideInInspector]
	public bool onTimer;

    [Header("Timer Slider")]
	public Slider timerSlider;
    
	[Header("XP Slider")]
	public Slider xpSlider;

	[Header("Content Best User")]
	public GameObject contentBestUser;
	
	[Header(" Best User Prefab")]
	public GameObject  bestUserPrefab;

	public ArrayList bestUsers;
	
	public AudioSource musicAudioSource;
	


	
	// Use this for initialization
	void Start () {

		if (instance == null) {

			DontDestroyOnLoad (this.gameObject);

			instance = this;
			
			OpenScreen(0);

			CloseAlertDialog ();

			timerSlider.GetComponent<Canvas>().enabled = false;

			bestUsers = new ArrayList ();

			
		}
		else
		{
			Destroy(this.gameObject);
		}



	}

		
	/// <summary>
	/// Opens the screen.
	/// </summary>
	/// <param name="_current">Current.</param>
	public void  OpenScreen(int _current)
	{
		switch (_current)
		{
		    //lobby menu
		    case 0:
			currentMenu = _current;
			HUDLobby.enabled = true;
			gameCanvas.enabled = false;
			StopMusic();
			break;


		    case 1:
			currentMenu = _current;
			HUDLobby.enabled = false;
			gameCanvas.enabled = true;
			PlayMusic();
		
			break;

	
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

    /// <summary>
    /// starts slider timer
    /// </summary>
    public void StartTimer()
	{
		StopInvoke();
		InvokeRepeating ("UpdateTimerSlider",timerDelay,timerTime);
	}

	public void StopInvoke()
	{
		CancelInvoke();
	}
	
	void UpdateTimerSlider()
	{
       
		//creates the slider decreasing effect of the slide
		if (onTimer && progress >0 && progress <=100) {
		    
			progress = progress - 1;
			timerSlider.GetComponent<Canvas>().enabled = true;
			timerSlider.value = progress;
	        
		}
	    //ends the slider animation
		else if (progress <= 0 && onTimer)
		{
			
			onTimer = false;
			timerSlider.GetComponent<Canvas>().enabled = false;
			progress = 100;
			NetworkManager.instance.EmitRegression();
	
		}
	}




	public void PlayAudio(AudioClip _audioclip)
	{
		
	   GetComponent<AudioSource> ().PlayOneShot (_audioclip);

	}

	public void PlayPickUpAudioClip()
	{

	   if (!GetComponent<AudioSource> ().isPlaying )
		{
		
		  GetComponent<AudioSource>().PlayOneShot(pickupAudioClip);

		}


	}
	
	public void PlayMusic()
	{
	   if (!musicAudioSource.isPlaying )
	   {
	     musicAudioSource.Play();
	   }
	}
	public void StopMusic()
	{
	 musicAudioSource.Stop();
	}


	
	public void SetUpBestKiller(string _name, string _kills, string _ranking)
	{
	    	  
	  	GameObject newUser = Instantiate (bestUserPrefab) as GameObject;

		
		newUser.GetComponent<User>().name.text = _name;
		newUser.GetComponent<User>().kills.text = _kills;
		newUser.GetComponent<User>().ranking.text = _ranking;
		newUser.transform.parent = contentBestUser.transform;
		newUser.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
		bestUsers.Add (newUser);
	}
	

 
	/// <summary>
	/// Clears the leader board.
	/// </summary>
	public void ClearLeaderBoard()
	{
		foreach (GameObject user in bestUsers)
		{

			Destroy (user.gameObject);
		}

		bestUsers.Clear ();
	}
	
	public void OpenLeaderBoard()
	{
	  NetworkManager.instance.EmitGetBestKillers();
	  leaderBoard.enabled = true;
	}
	
	
	
}//END_CLASS
}//END_NAMESPACE
