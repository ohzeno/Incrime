using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Player health.
/// </summary>
namespace FPSExample{
public class PlayerHealth : MonoBehaviour {

	

	public Image damageImage;

	public float health = 100;

	public float maxHealth = 100;

	public float damageValue = 10;

	bool damaged;

    public AudioClip damageAudioClip;

	public AudioClip deathSound;

	private AudioSource damageSoundSource;
	
	CapsuleCollider capsuleCollider;
	
	public bool isDead;

	[Header("Third Person Game Object")]
	public GameObject thirdPerson;

	[Header("RagDoll Prefab")]
	public GameObject thirdPersonRagDollPref;

	[HideInInspector] public GameObject thirdPersonRagDoll;
    

	void Awake()
	{
		isDead = false;

		capsuleCollider = GetComponent <CapsuleCollider> ();

	}

	

	public void TakeDamage ()
	{
		if (GetComponent<PlayerManager> ().isLocalPlayer) {
			damaged = true;
			CameraShake.Shake (0.25f, 0.1f);
			PlayDamageSound();

		
		}
	
	}
	
	 public void Death ()
    {
	   StartCoroutine ("DeathCutScene"); 
		
    }

	IEnumerator DeathCutScene()
	{ 
		isDead = true;
	    GetComponent<FPSController> ().PlayDeathCam();
	    DoRagDoll();
		yield return new WaitForSeconds(3f);

		NetworkManager.instance.GameOver ();
	}

	

	
	//---------------AUDIO METHODS--------
	public void PlayDeathSound()
	{
	   if (!GetComponent<AudioSource> ().isPlaying)
	    {
			
		  GetComponent<AudioSource>().PlayOneShot(deathSound);

		}
		

	}
		
	public void PlayDamageSound()
	{

	   if (!GetComponent<AudioSource> ().isPlaying )
		{
		
		  GetComponent<AudioSource>().PlayOneShot(damageAudioClip);

		}


	}

	public void DoRagDoll()
	{
	    Debug.Log("do ragdoll");
		if( thirdPersonRagDoll==null)
		{
			thirdPersonRagDoll = GameObject.Instantiate (thirdPersonRagDollPref,
				               transform.position, thirdPersonRagDollPref.transform.rotation) as GameObject;

			
		}
        	

	}

	

		
}
}
