using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MultiplayerShooter
{
public class ShipSoundController : MonoBehaviour
{
    public AudioSource audioSource;
    public float minPitch = 0.05f;
    private float pitchFromShip;

    public PlayerManager playerManager;
     
    // Start is called before the first frame update
    void Start()
    {
        audioSource.pitch = minPitch;
    }
 
    // Update is called once per frame
    void Update()
    {
       
        if(!playerManager.GetComponent<PlayerHealth>().isDead )
        {
            pitchFromShip = playerManager.acceleration;
           
        }
        else
        {
            audioSource.Stop();
        }

     
    }
}
}
