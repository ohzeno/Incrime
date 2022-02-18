using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

/**
 * Created by Rio 3D Studios.
 *
 * class to manage the choice of helicopters in the lobby room
 *
 */
public class CharacterChooseManager : MonoBehaviour {


	public static  CharacterChooseManager  instance;  //useful for any gameObject to access this class without the need of instances her or you declare her
	
	public Material[] materials;
	
	public GameObject avatar;
	
	public int maxCharacters = 4;
	
	[SerializeField] private Canvas nextButton, prevButton;

	public int currentAvatar = 0;


	// Use this for initialization
	void Start () {
	

		// if don't exist an instance of this class
		if (instance == null) {


			// define the class as a static variable
			instance = this;
			
			currentAvatar = 0;
			SetAvatar3DText(currentAvatar);
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
		
		SetAvatar3DText(currentAvatar);
		SkinnedMeshRenderer[] skinRends = avatar.GetComponentsInChildren<SkinnedMeshRenderer> ();
		
		foreach(SkinnedMeshRenderer smr in skinRends)
		{
		  smr.material = materials[currentAvatar];
		}
					
		if(currentAvatar>=maxCharacters)
		{
			currentAvatar = maxCharacters - 1;
		}
		CheckButtonStatus();
	
	  }
	}
	
	//method called by the BtnPrev button that selects the previous avatar
	public void PrevAvatar()
	{
	  if(currentAvatar-1 >= 0)
	   {
				    
		  currentAvatar--;
		  SetAvatar3DText(currentAvatar);
		  SkinnedMeshRenderer[] skinRends = avatar.GetComponentsInChildren<SkinnedMeshRenderer> ();
		
		  foreach(SkinnedMeshRenderer smr in skinRends)
		  {
		    smr.material = materials[currentAvatar];
		  }
		 		
		   if(currentAvatar<0)
		   {
			  currentAvatar =0;
		   }
		   CheckButtonStatus();
		  
		}
	}
	
	//method used in the NetworkManager class to define the skin of the character chosen by the localPlayer
	public void SetUpCharacter(PlayerManager player)
	{
	   SkinnedMeshRenderer[] skinRends = player.model.GetComponents<SkinnedMeshRenderer> ();
		
		  foreach(SkinnedMeshRenderer smr in skinRends)
		  {
		    smr.material = materials[currentAvatar];
		  }
		 		
	}
	
	//method used in the NetworkManager class to define the skin of the character chosen by NetworkPlayer (the other online players)
	public void SetUpNetworkCharacter(PlayerManager player, int index)
	{
	   SkinnedMeshRenderer[] skinRends = player.model.GetComponents<SkinnedMeshRenderer> ();
		
		  foreach(SkinnedMeshRenderer smr in skinRends)
		  {
		    smr.material = materials[index];
		  }
		 		
	}
	
	 /// <summary>
    /// helper method to set HUD avatar name
    /// </summary>
    /// <returns></returns>
    public void SetAvatar3DText(int index)
    {
        
        if (index == 0)
            avatar.GetComponentInChildren<TextMesh> ().text = "Red Samurai";
        else if (index == 1)
            avatar.GetComponentInChildren<TextMesh> ().text = "Green Samurai";
        else if (index == 2)
             avatar.GetComponentInChildren<TextMesh> ().text = "Yellow Samurai";
        else if (index == 3)
             avatar.GetComponentInChildren<TextMesh> ().text = "Pink Samurai";
       
    }
	
	public void Reset()
	{
	    currentAvatar = 0;
		SetAvatar3DText(currentAvatar);
		CheckButtonStatus();
		SkinnedMeshRenderer[] skinRends = avatar.GetComponentsInChildren<SkinnedMeshRenderer> ();
		
		  foreach(SkinnedMeshRenderer smr in skinRends)
		  {
		    smr.material = materials[currentAvatar];
		  }
	}
	
}
