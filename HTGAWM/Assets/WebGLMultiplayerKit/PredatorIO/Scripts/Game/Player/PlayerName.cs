using UnityEngine;
using System.Collections;
namespace SavanaIO
{
public class PlayerName : MonoBehaviour {

	// Use this for initialization
	void Start () {}
	
	public void setName(string name)
	{
		GetComponent<TextMesh> ().text = name;
		Debug.Log(""+GetComponent<TextMesh> ().text );
	}
}
}
