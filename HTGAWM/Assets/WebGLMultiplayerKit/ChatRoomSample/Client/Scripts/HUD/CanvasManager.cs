// ----------------------------------------------------------------------------------------------------------------------
// <copyright company="Rio 3D Studios">UDPNet Pro - Copyright (C) 2020 Rio 3D Studios</copyright>
// <author>Sebastiao Lucio - sebastiao@ice.ufjf.br</author>
// ----------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;

/// <summary>
/// class to manage all the game's UI's and HUDs
/// </summary>
namespace ChatSample
{
 public class CanvasManager : MonoBehaviour
 {
	//useful for any script access this class without the need of object instance  or declare .
    public static  CanvasManager instance;

	[Header("Canvas HUD's :")]
	
	public Canvas hudSignIn; // set in inspector. stores the SignIn Panel

	public Canvas hudUserList; // set in inspector. stores the  User List Panel
	
	public Canvas chatRoom; // set in inspector. stores the  chat Room Panel
	
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

	public GameObject myMessagePrefab; // set in inspector. stores the user Prefab message game object
	
	public GameObject networkMessagePrefab;  // set in inspector. stores the network user message game object

    [Header("Contents :")]

	public GameObject contentUsers; // set in inspector. stores the contentUser game object
	
	public GameObject contentMessages; // set in inspector. stores the content messages game object
	
	
    [HideInInspector]
	public int countMessages; //variable for controlling the number of messages on the screen
	
	[HideInInspector]
	public int maxDeleteMessage; //variable for controlling the number of messages on the screen
	
    [HideInInspector]
	public int currentSkin; // flag to mark the chosen avatar
	
    [HideInInspector]
	public string currentHUD; // flag to mark the current screen
	
	ArrayList users; // users list
	
	ArrayList messages; // list to store all messages
	

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) {

			DontDestroyOnLoad (this.gameObject);

			instance = this;
			
			users = new ArrayList ();
			
			messages = new ArrayList ();
			
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
	        chatRoom.enabled = false;
			break;
			case "room":
			currentHUD = _current;
			hudSignIn.enabled = false;
			hudUserList.enabled = true;
	        chatRoom.enabled = true;
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
	/// set the user profile.
	/// </summary>
	/// <param name="_name">User Name.</param>
	/// <param name="_avatar_index">User avatar.</param>
	public void SetUpProfile( string _name,int _avatar_index)
	{
	  
	  profileName.text = _name;
	
      profileImg.sprite = profileSpritesPref[_avatar_index].GetComponent<SpriteRenderer>().sprite;;
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

	  newUser.GetComponent<User>().id = _id;
	  newUser.GetComponent<User>().name.text = _name;
	  newUser.GetComponent<User>().profileImg.sprite = profileSpritesPref[_avatar_index].GetComponent<SpriteRenderer>().sprite;		
	  newUser.transform.parent = contentUsers.transform;
	  newUser.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
	  users.Add (newUser);
	}
	
	public void SpawnNetworkMessage( string _message, int _avatar_index)
	{
	  countMessages +=1;
	  
	  GameObject newMessage = Instantiate (networkMessagePrefab) as GameObject;
	  newMessage.name = countMessages.ToString();
	  newMessage.GetComponent<Message>().id = countMessages;
	  newMessage.GetComponent<Message>().txtMsg.text = _message;
	  newMessage.GetComponent<Message>().userImg.sprite = profileSpritesPref[_avatar_index].GetComponent<SpriteRenderer>().sprite;	
      newMessage.transform.parent = contentMessages.transform;
	  newMessage.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);	  
	  messages.Add (newMessage);
	  
	  if (messages.Count > 7)
		{
			int j = 0;

			foreach(Message msg in messages )
			{
				if (j == 0) 
				{

					Destroy (GameObject.Find(msg.id.ToString()));
					messages.Remove (msg);

					break;
				}
				j += 1;

			}
		}  
	}
	
	public void SpawnMyMessage( string _message)
	{
	
	 
	  countMessages +=1;
	  
	  GameObject newMessage = Instantiate (myMessagePrefab) as GameObject;
	  newMessage.name = countMessages.ToString();
	  newMessage.GetComponent<Message>().txtMsg.text = _message;
	  newMessage.GetComponent<Message>().userImg.sprite = profileSpritesPref[currentSkin].GetComponent<SpriteRenderer>().sprite;
      newMessage.transform.parent = contentMessages.transform;
	  newMessage.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);	  	  
	  messages.Add (newMessage);
	  Debug.Log("messages.Count : "+messages.Count);
	   if (messages.Count > 7)
		{
		     ArrayList deleteMessages = new ArrayList();

			int j = 0;

			foreach(GameObject msg in messages )
			{
				if (j <= maxDeleteMessage) 
				{
                    deleteMessages.Add(msg);
				}
				j += 1;

			}
			
			foreach(GameObject msg in deleteMessages)
            {
			  Destroy (msg);
              messages.Remove(msg);
             }

		}
    
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

 
}//END_OF_CLASS
}//END_OF_NAMESPACE