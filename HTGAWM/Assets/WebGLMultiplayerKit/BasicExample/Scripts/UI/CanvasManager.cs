using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour {

	public static  CanvasManager instance;

	public Canvas gameCanvas;

	public Canvas pLobby;

	public Canvas alertGameOverDialog;

	public InputField inputLogin;

	public Text alertGameOverText;

	public Text alertDialogText;

	public GameObject lobbyCamera;

	public int currentMenu;

	public Slider healthSlider;

	public Text txtHealth;

	public Canvas mobileButtons;
	
	public bool enabledMobileBtns;
	

	// Use this for initialization
	void Start () {

		if (instance == null) {

			DontDestroyOnLoad (this.gameObject);
			instance = this;
			alertGameOverDialog.enabled = false;
			alertDialogText.enabled = false;
			OpenScreen(0);

		}
		else
		{
			Destroy(this.gameObject);
		}

		CloseAlertDialog ();

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
			pLobby.enabled = true;
			gameCanvas.enabled = false;
			lobbyCamera.GetComponent<Camera> ().enabled = true;
			break;

			//no lobby menu
		    case 1:
			currentMenu = _current;
			pLobby.enabled = false;
			gameCanvas.enabled = true;
			lobbyCamera.GetComponent<Camera> ().enabled = false;
			break;

		}

	}


	public void ShowGameOverDialog()
	{
		
		alertGameOverText.text = "YOU LOSE!";
		alertGameOverDialog.enabled = true;
	}

	public void CloseGameOverDialog()
	{
		alertGameOverDialog.enabled = false;
        //Destroy(NetworkManager.instance.camRig);
		OpenScreen (0);
	}

	/// <summary>
	/// Shows the alert dialog.
	/// </summary>
	/// <param name="_message">Message.</param>
	public void ShowAlertDialog(string _message)
	{
		alertDialogText.text = _message;
		alertDialogText.enabled = true;
		StartCoroutine (CloseAlertDialog() );//chama corrotina para esperar o player colocar o outro pé no chão
	}

	/// <summary>
	/// Closes the alert dialog.
	/// </summary>

	IEnumerator CloseAlertDialog() 
	{

		yield return new WaitForSeconds(4);
		alertDialogText.text = "";
		alertDialogText.enabled = false;
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
