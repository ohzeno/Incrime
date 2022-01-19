
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;

/// <summary>
/// class to manage all the game's UI's and HUDs
/// </summary>
namespace ChatBox
{
 public class CanvasManager : MonoBehaviour
 {
	//useful for any script access this class without the need of object instance  or declare .
    public static  CanvasManager instance;

	[Header("Canvas HUD's :")]
	
	public Canvas hudSignIn; // set in inspector. stores the SignIn Panel

	public Canvas hudUserList; // set in inspector. stores the  User List Panel
	
	public Canvas hudChooseAvatar;  // set in inspector. stores the  Choose Avatar Panel
	
    [Header("Text Variables :")]
	public Text profileName; // set in inspector. stores the user profile name text

	public Text txtLog; // set in inspector. stores the txtLog

	
    [Header("Input field Variables :")]
	public InputField ifLogin; // set in inspector. stores the login InputField

	public InputField inputFieldMessage;	

	[Header("Image field Variables :")]
	public Image profileImg; // set in inspector. stores the profile image container

	[Header("Prefabs :")]

	public GameObject[] profileSpritesPref;  // set in inspector. stores the profile sprites list
	
	public GameObject userPrefab; // set in inspector. stores the user Prefab Game Object


    [Header("Contents :")]

	public GameObject contentUsers; // set in inspector. stores the contentUser game object
	
	public int currentSkin;
  
    [HideInInspector]
	public string currentHUD; // flag to mark the current screen
	
	ArrayList users; // users list
	

	public GameObject chatBox;

	public GameObject contentChatBox;

	//store all players in game
	public Dictionary<string, ChatBox> chatBoxes = new Dictionary<string, ChatBox>();

	
	

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) {

			DontDestroyOnLoad (this.gameObject);

			instance = this;
			
			users = new ArrayList ();
			
			OpenScreen("login");


		}
		else
		{
			Destroy(this.gameObject);
		}
    }

  
	/// <summary>
	/// Opens the  current screen.
	/// </summary>
	/// <param name="_current">Current screen .</param>
	public void  OpenScreen(string _current)
	{
		switch (_current)
		{
		    case "login":
			currentHUD = _current;
			hudSignIn.enabled = true;
			hudUserList.enabled = false;
			break;
			case "room":
			currentHUD = _current;
			hudSignIn.enabled = false;
			hudUserList.enabled = true;
	   
			break;
			case "chooseAvatar":
			 hudChooseAvatar.enabled = true;
			break;
		
		}

	}
	
	/// <summary>
	/// set the avatar chosen by the user.
	/// </summary>
	/// <param name="_current">Current.</param>
	public void SetUpcurrentSkin(int _avatar_index)
	{
	  currentSkin = _avatar_index;
	  
	  hudChooseAvatar.enabled = false;
	}


	
	
	/// <summary>
	/// spawn new user gameObject.
	/// </summary>
	/// <param name="_id">user id.</param>
	/// <param name="_name">username.</param>
	/// <param name="_avatar_index">user avatar.</param>
	public void SpawnUser(string _id, string _name, int _avatar_index)
	{
	  GameObject newUser = Instantiate (userPrefab) as GameObject;

	    Debug.Log("user spawned");

	  newUser.GetComponent<User>().id = _id;

	  newUser.GetComponent<User>().name.text = _name;

	  newUser.GetComponent<User>().profileImg.sprite = profileSpritesPref[_avatar_index].GetComponent<SpriteRenderer>().sprite;	

	  newUser.transform.parent = contentUsers.transform;

	  newUser.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
	  users.Add (newUser);

	}
	

	public void SpawnChatBox( string _id,string _host_id,string _guest_id, string _profileName, int _avatar_index)
	{
	  
	  
	  GameObject newChatBox = Instantiate (chatBox) as GameObject;

	  newChatBox.GetComponent<ChatBox>().id =  _id; 
	  newChatBox.GetComponent<ChatBox>().host_id =  _host_id; 
	  newChatBox.GetComponent<ChatBox>().guest_id=  _guest_id; 
	  newChatBox.GetComponent<ChatBox>().profileName.text = _profileName; 
	  newChatBox.GetComponent<ChatBox>().currentAvatar = _avatar_index;
	  newChatBox.GetComponent<ChatBox>().profileImage.sprite = profileSpritesPref[_avatar_index].GetComponent<SpriteRenderer>().sprite;	
      newChatBox.transform.parent = contentChatBox.transform;
	  newChatBox.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);	
	  chatBoxes [_id]  = newChatBox.GetComponent<ChatBox>();

	}
	

	
	public void DestroyUser(string _id)
	{
	  
		     GameObject deletedUser = null;

			int j = 0;

			foreach(GameObject user in users )
			{
				if (user.GetComponent<User>().id.Equals(_id)) 
				{
                    deletedUser = user;
				}
				

			}
			
			Destroy (deletedUser);
            users.Remove(deletedUser);
			Debug.Log("user destroyed");
			
	}

	public void CloseChatBox(ChatBox _chat_box)
	{
		Destroy (_chat_box.gameObject);
        chatBoxes.Remove(_chat_box.id);
	}

 
}//END_OF_CLASS
}//END_OF_NAMESPACE