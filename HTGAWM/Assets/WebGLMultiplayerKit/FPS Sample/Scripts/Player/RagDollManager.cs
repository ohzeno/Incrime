using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSExample{
public class RagDollManager : MonoBehaviour
{
   
   public GameObject hips;

   public Transform cameraToTarget;
  

    void Awake()
    {
        Rigidbody[] rig = GetComponentsInChildren<Rigidbody> ();

		foreach(Rigidbody r in rig)
		{
			r.useGravity = true;
			r.isKinematic= false;
		}
		
		Destroy (gameObject, 7f);
       
    }

   
    public void DestroyRagDoll()
    {
       Destroy(gameObject);

    }
}
}

