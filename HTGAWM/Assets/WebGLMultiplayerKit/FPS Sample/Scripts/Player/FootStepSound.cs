using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FPSExample{
public class FootStepSound : MonoBehaviour {

	public AudioSource footAudioSource ;

	public AudioClip[] woodSteps;
	public AudioClip[] metalSteps ;
	public AudioClip[] concreteSteps;
	public  AudioClip[] sandSteps ;


	private Transform myTransform;

	public   LayerMask hitLayer;
	private  string cTag;
	public bool step;
	public bool inicialDelay;
	public bool isMoving;
	float audioStepDelayWalk = 0.45f;
	float audioStepDelayRun = 0.25f;
	public bool onHit;
	public float offset;

	// Use this for initialization
	void Start () {
		
		myTransform = transform;
		step = true;
	}
	
	// Update is called once per frame
	void Update () {

			// read inputs
		isMoving = false;
		
		float v = 0;
		float h = 0;
		//up button or joystick
		if (Input.GetKey("up"))
		{
		  v =1;
		  isMoving = true;
		}
		if (Input.GetKey("down"))//down button or joystick
		{
			v=-1;
			isMoving = true;
			
		}
		//up button or joystick
		if (Input.GetKey("left"))
		{
		  h =-1;
		  isMoving = true;
		 
		}
		if (Input.GetKey("right"))//down button or joystick
		{
			h=1;
			isMoving = true;
			
		}

		
		OnFootStrike ();
		
	}

	public void OnFootStrike () 
	{
		//if player not moving
		if (!isMoving) 
		{
			inicialDelay = false;//clear the variable inicialDelay
		}

		  //  GetComponent<AudioSource>().volume = Mathf.Clamp01(0.1f + myRigidbody.velocity.magnitude * 0.3f);
		if (isMoving && step)
		{
			//if the player was  began to run, we have to give a delay also for begin  of the movement 
			//se o player estava para e comessou a correr, temos que dar um delay tambem para o inicio do movimento
			//se ainda não deu um delay inicial para o player comessar a correr
			if (!inicialDelay) 
			{
				StartCoroutine (WaitForFootSteps (audioStepDelayRun));//call coroutine 
				inicialDelay = true;
			}
			else
			{
			     footAudioSource.PlayOneShot (GetAudio (), GetComponent<AudioSource> ().volume);
				StartCoroutine (WaitForFootSteps (audioStepDelayRun));////call coroutine for wait other foot
			}
		}
	}


	public AudioClip GetAudio()
	{
		RaycastHit hit;
       
		onHit = Physics.Raycast(myTransform.position + new Vector3(0, offset, 0), -Vector3.up,out hit, Mathf.Infinity, hitLayer);
		if(Physics.Raycast(myTransform.position + new Vector3(0, offset, 0), -Vector3.up,out hit, Mathf.Infinity, hitLayer))
		{
			cTag = hit.collider.tag.ToLower(); //tag do chão de onde o player se localiza
		}
       
		if(cTag == "wood")
		{
			return woodSteps[Random.Range(0, woodSteps.Length)];
		}
		else if(cTag == "metal")
		{
			return metalSteps[Random.Range(0, metalSteps.Length)];
		}
		else if(cTag == "floor")
		{
			//GetComponent<AudioSource>().volume = 0.8f;
			return concreteSteps[Random.Range(0, concreteSteps.Length)];
		}
		else if(cTag == "dirt")
		{
		//	GetComponent<AudioSource>().volume = 1.0f;
			return sandSteps[Random.Range(0, sandSteps.Length)];
		}
		else if(cTag == "sand")
		{
			//GetComponent<AudioSource>().volume = 1.0f;
			return sandSteps[Random.Range(0, sandSteps.Length)];
		}
		else
		{
			//GetComponent<AudioSource>().volume = 1.0f;
			return concreteSteps[Random.Range(0, concreteSteps.Length)];
		}
	}

	//  is necessary to wait  the other foot of the player reaches the ground 
	IEnumerator WaitForFootSteps(float _stepDelay) 
	{
		step = false;
		yield return new WaitForSeconds(_stepDelay);
		step = true;
	} 
}
}