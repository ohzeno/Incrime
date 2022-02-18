using UnityEngine;
using System.Collections;
namespace MultiplayerShooter
{
public class Explosion : MonoBehaviour {

	private float timeLife;
	public 	float maxLifeTime;

	// Use this for initialization
	void Start () {

		timeLife = 0;
	
	}
	
	// Update is called once per frame
	void Update () {
	

		timeLife += Time.deltaTime;

		if (timeLife >= maxLifeTime) {


			Destroy(gameObject);

		}

	}
}
}
