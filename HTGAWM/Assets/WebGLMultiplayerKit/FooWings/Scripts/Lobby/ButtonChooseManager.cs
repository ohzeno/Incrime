using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;


namespace MultiplayerShooter
{
public class ButtonChooseManager : MonoBehaviour {


	public static  ButtonChooseManager  instance;

	
	[Header("Max Characters")]
	public int maxCharacters = 5;
	

    [Header("Slide Buttons")]
	[SerializeField] private Canvas nextButton, prevButton;

	public GameObject[] wings; //ships available for choice

  
	
	public int currentWings = 0;

	// Use this for initialization
	void Awake () {
	
		// if don't exist an instance of this class
		if (instance == null) {


			// define the class as a static variable
			instance = this;

			currentWings = 0;

			//configures and displays the first avatar as available
			SetWings(currentWings);

			//configure the slider buttons
			CheckButtonStatus();

		}
	}

    
	

	/// <summary>
	/// method for controlling the avatars choice buttons
	/// </summary>
	private void CheckButtonStatus()
	{
	
		if (nextButton == null || prevButton == null)
			return;
		
		if (currentWings == 0) 
		{
			prevButton.enabled = false;
			nextButton.enabled = true;
		} else if (currentWings >= maxCharacters-1) 
		{
			prevButton.enabled = true;
			nextButton.enabled = false;
		} else 
		{
			prevButton.enabled = true;
			nextButton.enabled = true;
		}
		
	}


	/// <summary>
	/// method called by the BtnNext button that selects the next avatar
	/// </summary>
	public void NextAvatar()
	{
	  if(currentWings+1< maxCharacters)
	  {	   
		currentWings++;
		
		//configures the current character for display to the user
		SetWings(currentWings);
		
		if(currentWings>=maxCharacters)
		{
			currentWings = maxCharacters - 1;
		}

		CheckButtonStatus();
	
	  }
	}
	
	/// <summary>
	/// method called by the BtnPrev button that selects the previous avatar
	/// </summary>
	public void PrevAvatar()
	{
	  if(currentWings-1 >= 0)
	   {
				    
		  currentWings--;

		
		  //configures the current character for display to the user
		  SetWings(currentWings);
		
		 		
		   if(currentWings<0)
		   {
			  currentWings =0;
		   }

		   CheckButtonStatus();
		  
		}
	}

    

    
	
	
	/// <summary>
	///   helper method to set HUD avatar 
	/// </summary>
	/// <param name="index">current character index</param>
    /**
	 * method to defines the helicopter to be displayed according to the player's choice
	 *
     */
	void SetWings(int index)
	{
	 
	  for(int i =0;i< wings.Length;i++)
	  {
	    if(i.Equals(index))
		{
		   wings[index].SetActive(true);  //displays the helicopter chosen by the player
		}
		else
		{
		   wings[i].SetActive(false);  //hides the other helicopters
		}
	  }
	}

}//END_CLASS
}//END_NAMESPACE
