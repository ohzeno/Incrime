using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AimDownSights : MonoBehaviour
{
    public static  AimDownSights instance;

	public float defaultFOV = 60.0F;

	public float aimedFOV = 45.0F;

	public float smoothFOV = 10.0F;
	
	public Vector3 hipPosition;//default postion of the gun

	public Transform aimTransform;//aim trasnform point

	public float smoothAim = 12.5F;

	 void Start()
	{
		
		if (instance == null) {

		   instance = this;

		   hipPosition = transform.localPosition;//set up default position of the gun
		}
		else
		{
			Destroy(this.gameObject);
		}

	}
	
	public void EnableAim()
	{
	  //changes position of the gun in screen
	  //transform.localPosition = Vector3.Lerp (transform.localPosition, aimTransform.localPosition, Time.deltaTime * smoothAim);
	  Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, aimedFOV, Time.deltaTime * smoothFOV);// approximates the camera
			
	}
	
	public void DisableAim()
	{
	   //sets the weapon for the original position
	   //transform.localPosition = Vector3.Lerp(transform.localPosition, hipPosition, Time.deltaTime * smoothAim);
	   Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, defaultFOV, Time.deltaTime * smoothFOV);//return original zoom
	}
	

}