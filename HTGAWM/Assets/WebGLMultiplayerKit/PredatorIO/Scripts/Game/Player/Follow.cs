using UnityEngine;
using System.Collections;

namespace SavanaIO
{
public class Follow : MonoBehaviour {

	public Transform target;
	
	public Vector3 offset;

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {
	
		if (target) {

		 transform.position = new Vector3 (target.position.x + - target.GetComponent<CircleCollider2D> ().radius + offset.x, target.position.y + offset.y * target.GetComponent<CircleCollider2D> ().radius, transform.position.z);
	
		}
	}

	public void SetTarget(Transform _target)
	{
		target = _target;
	}

}
}
