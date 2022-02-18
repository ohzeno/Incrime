using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerShooter
{
public class GameCanvas : MonoBehaviour
{

    public static GameCanvas  instance;

	
	public Text txtLocalPlayerHealth;

	int currentMenu;


	public bool gameOver;
	

	public Canvas  alertGameOverDialog;

	public Text  alertGameOverText;

	public AudioSource music;
	
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

			instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}

	}
	


	//reset the game for the losing player
	public void GameOver()
	{
	   gameOver = true;
	  ShowGameOverPanel("YOU LOSE!!!");
	
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
	



	public void ShowGameOverPanel(string _message)
	{
	    
		StopMusic ();
	
		alertGameOverDialog.enabled = true;
		alertGameOverText.text = _message;
		

	}

	public void CloseGameOver()
	{

	  alertGameOverDialog.enabled = false;
	
	}

	
	public void StopMusic()
	{
	
		music.Stop();
	}

	public void PlayAudio(AudioClip _audioclip)
	{
		if (!GetComponent<AudioSource> ().isPlaying) {
			GetComponent<AudioSource> ().PlayOneShot (_audioclip);
		}
	}

}
}