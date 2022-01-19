using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///Manage Network player if isLocalPlayer variable is false
/// or Local player if isLocalPlayer variable is true.
/// </summary>
namespace ChatBox
{
public class PlayerManager : MonoBehaviour {

	public string	id;

	public string name;

	public int avatar;
	
	public bool isOnline;

	public bool isLocalPlayer;

	public Transform cameraToTarget;

	public float verticalSpeed = 3.0f;

	public float rotateSpeed = 150f;

	float h ;
	
	float v;
	

	// Use this for initialization
	public void Set3DName(string name)
	{
		GetComponentInChildren<TextMesh> ().text = name;

	}



	// Update is called once per frame
	void FixedUpdate () {
		

		if (isLocalPlayer)
		{
			Move();
		}
	

	}

	void Move( )
	{

       
		 // Store the input axes.
        h = Input.GetAxisRaw("Horizontal");
              
		v = Input.GetAxisRaw("Vertical");

		var x = h* Time.deltaTime *  verticalSpeed;
		var y = h * Time.deltaTime * rotateSpeed;
		var z = v * Time.deltaTime * verticalSpeed;

		transform.Rotate (0, y, 0);

		transform.Translate (0, 0, z);

		if (h != 0 || v != 0  ) {
		
			UpdateStatusToServer ();
		}
	

	}



	void UpdateStatusToServer ()
	{


		//hash table <key, value>
		Dictionary<string, string> data = new Dictionary<string, string>();

		data["local_player_id"] = id;

		data["position"] = transform.position.x+":"+transform.position.y+":"+transform.position.z;

		data["rotation"] = transform.rotation.y.ToString();

		NetworkManager.instance.EmitMoveAndRotate(data);



	}



	public void UpdatePosition(Vector3 position)
	{
	
		transform.position = new Vector3 (position.x, position.y, position.z);
		
	}

	public void UpdateRotation(Quaternion _rotation)
	{
		transform.rotation = _rotation;

	}


}
}