using UnityEngine;
using System.Collections;
namespace MultiplayerShooter
{
public class Spin : MonoBehaviour {

	public Vector3 spinXYZ;

	public bool offSpin;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
       
			transform.Rotate(spinXYZ*Time.deltaTime);

		
	}


}
}
