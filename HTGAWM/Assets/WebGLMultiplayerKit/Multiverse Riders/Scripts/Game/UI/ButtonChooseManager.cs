using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
namespace MultiverseRiders
{
public class ButtonChooseManager : MonoBehaviour {


	public static  ButtonChooseManager  instance;
	
	public Animator charAnim;
	
	public int maxCharacters = 5;
	
	public TextMeshProUGUI txtCharName;

	[SerializeField] private Canvas nextButton, prevButton;

	public int currentAvatar = 0;


	// Use this for initialization
	void Start () {
	
		// if don't exist an instance of this class
		if (instance == null) {


			// define the class as a static variable
			instance = this;
			
			currentAvatar = 0;
			SetAvatarTextName(currentAvatar);
			CheckButtonStatus();

		}
	}
	

	//method for controlling the avatars choice buttons
	private void CheckButtonStatus()
	{
	
		if (nextButton == null || prevButton == null)
			return;
		
		if (currentAvatar == 0) 
		{
			prevButton.enabled = false;
			nextButton.enabled = true;
		} else if (currentAvatar >= maxCharacters-1) 
		{
			prevButton.enabled = true;
			nextButton.enabled = false;
		} else 
		{
			prevButton.enabled = true;
			nextButton.enabled = true;
		}
		
	}

	//method called by the BtnNext button that selects the next avatar
	public void NextAvatar()
	{
	  if(currentAvatar+1< maxCharacters)
	  {	   
		currentAvatar++;
		
		SetAvatarTextName(currentAvatar);
		
		switch (currentAvatar) { 

		  case 0:
		  	charAnim.SetTrigger ("IsBlack");
		  break;
		  case 1:
		  	charAnim.SetTrigger ("IsBlue");
		  break;
		   case 2:
		  	charAnim.SetTrigger ("IsGreen");
		  break;
		  case 3:
		  	charAnim.SetTrigger ("IsRed");
		  break;
		   case 4:
		  	charAnim.SetTrigger ("IsYellow");
		  break;
		}
		
		if(currentAvatar>=maxCharacters)
		{
			currentAvatar = maxCharacters - 1;
		}

		ShooterNetworkManager.instance.EmitChangeAvatar();

		CheckButtonStatus();
	
	  }
	}
	
	//method called by the BtnPrev button that selects the previous avatar
	public void PrevAvatar()
	{
	  if(currentAvatar-1 >= 0)
	   {
				    
		  currentAvatar--;
		  
		  SetAvatarTextName(currentAvatar);
		
		  switch (currentAvatar) { 

		     case 0:
		  	charAnim.SetTrigger ("IsBlack");
		  break;
		  case 1:
		  	charAnim.SetTrigger ("IsBlue");
		  break;
		   case 2:
		  	charAnim.SetTrigger ("IsGreen");
		  break;
		  case 3:
		  	charAnim.SetTrigger ("IsRed");
		  break;
		   case 4:
		  	charAnim.SetTrigger ("IsYellow");
		  break;
		  }
		 		
		   if(currentAvatar<0)
		   {
			  currentAvatar =0;
		   }

		   ShooterNetworkManager.instance.EmitChangeAvatar();

		   CheckButtonStatus();
		  
		}
	}
	
	
	
	
	 /// <summary>
    /// helper method to set HUD avatar name
    /// </summary>
    /// <returns></returns>
    public void SetAvatarTextName(int index)
    {
        
        if (index == 0)
            txtCharName.text = "Black Rider";
        else if (index == 1)
           txtCharName.text = "Blue Rider";
		else if (index == 2)
           txtCharName.text = "Green Rider";
		else if (index == 3)
           txtCharName.text = "Red Rider";
		else if (index == 4)
           txtCharName.text = "Yellow Rider";
  
       
    }
	
}
}
