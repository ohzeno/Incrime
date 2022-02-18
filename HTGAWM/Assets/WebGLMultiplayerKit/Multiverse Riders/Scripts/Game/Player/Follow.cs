using UnityEngine;
using System.Collections;

namespace MultiverseRiders
{
public class Follow : MonoBehaviour {

	public Transform target;
	
	public Vector3 offset;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (target) {
			transform.position = new Vector3 (target.position.x + - target.GetComponent<BoxCollider2D> ().size.x + offset.x, target.position.y + offset.y *  target.GetComponent<BoxCollider2D> ().size.y, transform.position.z);
	
		}
	}

	public void SetTarget(Transform _target)
	{
		target = _target;
	}




}
}
