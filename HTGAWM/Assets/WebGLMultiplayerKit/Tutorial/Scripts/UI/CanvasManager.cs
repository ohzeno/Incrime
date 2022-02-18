using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Tutorial{
public class CanvasManager : MonoBehaviour {

	public static  CanvasManager instance;

	public Canvas pLobby;

	public InputField inputLogin;

	public GameObject lobbyCamera;

	public int currentMenu;

	

	// Use this for initialization
	void Start () {

		if (instance == null) {

			DontDestroyOnLoad (this.gameObject);
			instance = this;
			OpenScreen(0);

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
			pLobby.enabled = true;
			lobbyCamera.GetComponent<Camera> ().enabled = true;
			break;

			//no lobby menu
		    case 1:
			currentMenu = _current;
			pLobby.enabled = false;
			lobbyCamera.GetComponent<Camera> ().enabled = false;
			break;

		}

	}

}
}