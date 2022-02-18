using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

 /// <summary>
/// class to manage all the game's UI's and HUDs.
/// </summary>
/// 
namespace FPSExample {
public class CanvasManager : MonoBehaviour
{
    
	public static CanvasManager  instance;  //useful for any gameObject to access this class without the need of instances her or you declare her

   [Header("Canvas HUD's :")]
	public Canvas  HUDLobby; // set in inspector. stores the Lobby Panel.
	
	public Canvas  gameCanvas; // set in inspector. variable that stores the Game Canvas panel
	
	public Canvas alertgameDialog;// set in inspector. variable that stores the AlertGameDialog panel
	
	public Canvas alertGameOverDialog;
	
	public Text alertGameOverText;
	
	public int currentMenu; //store current HUD screen
	
	public GameObject lobbyCamera;  // variable that stores the Lobby Camera
	
	public InputField inputLogin;   // variable that stores the Input Filed
	
	
	
 /*********************** TEXT VARIABLES ***************************/
	[Header("Text Variables :")]
	public Text txtLocalPlayerHealth; // set in inspector.
	
	public Text alertDialogText; // set in inspector.
	
	public Text messageText; // set in inspector.
	
	public Text txtLog; // set in inspector.
	
	public Text txtTotalPlayers; // set in inspector.
	
	public Text txtKills; // set in inspector.
	
	public  AudioClip buttonSound; // set in inspector.
	
	
	/***********************PANEL GUN ***************************/
	[Header("Gun variables :")]
	public Image currentGunImage; // set in inspector.
	
	public Text txtCurrentAmmo; // set in inspector.

	public Slider currentBulletSlider; // set in inspector.
	
	/***********************************************************/
	
	
	/***********************DAMAGE SKIN***************************/
	[Header("Damage variables :")]
	public Image damageImage; // set in inspector.

	public float flashSpeed = 5f; // set in inspector.

	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);  // set in inspector.

	public Slider healthSlider;  // set in inspector.

	public Text txtHealth;  // set in inspector.

	 [HideInInspector]
	public bool damaged; 
	/***********************************************************/
	

	// Use this for initialization
	void Start () {

		if (instance == null) {

			DontDestroyOnLoad (this.gameObject);

			instance = this;
			
			OpenScreen(0);
		    
			alertGameOverDialog.enabled = false;
			
			CloseAlertDialog ();
		

		}
		else
		{
			Destroy(this.gameObject);
		}



	}

	void Update()
	{
		
	 	/***********************DAMAGE SKIN***************************/
		if(damaged)
		{
			damageImage.color = flashColour;
		}
		else
		{
			damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
		}
		damaged = false;
      /*************************************************************************/
}
	/// <summary>
	/// Opens the screen.
	/// </summary>
	/// <param name="_current">current screen index.</param>
	public void  OpenScreen(int _current)
	{
		switch (_current)
		{
		    //lobby menu
		    case 0:
			currentMenu = _current;
			HUDLobby.enabled = true;
			gameCanvas.enabled = false;
			lobbyCamera.SetActive(true);
			break;


		    case 1:
			currentMenu = _current;
			HUDLobby.enabled = false;
			gameCanvas.enabled = true;
			lobbyCamera.SetActive(false);
			break;

	
		}

	}


	/// <summary>
	/// Shows the alert dialog.
	/// </summary>
	/// <param name="_message">display message.</param>
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
	
	public void ShowGameOverDialog()
	{
		
		alertGameOverText.text = "YOU LOSE!";
		alertGameOverDialog.enabled = true;
	}

	public void CloseGameOverDialog()
	{
		alertGameOverDialog.enabled = false;

		OpenScreen (0);
	}
	
	/// <summary>
	/// Shows the alert dialog.Debug.Log
	/// </summary>
	/// <param name="_message">Message.</param>
	public void ShowMessage(string _message)
	{
		messageText.text = _message;
		messageText.enabled = true;
		StartCoroutine (CloseMessage() );
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
	/// configure the gun pannel
	/// </summary>
	public void SetUpPanelGun(int maxBulets,  int currentAmmo)
	{
	   txtCurrentAmmo.text = currentAmmo.ToString();
	   
	   currentBulletSlider.maxValue = currentAmmo;

	   currentBulletSlider.value = currentAmmo;
	}
	
	/// <summary>
	/// update the weapon's HUD
	/// </summary>
	public void UpdatePanelGun( int currentAmmo)
	{
	   txtCurrentAmmo.text = currentAmmo.ToString();
	   
	   currentBulletSlider.value = currentAmmo;
	   
	}


	public void PlayAudio(AudioClip _audioclip)
	{
	   GetComponent<AudioSource> ().PlayOneShot (_audioclip);
	}
	

}//END_OF_CLASS
}//END_OF_NAMESPACE
