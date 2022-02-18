using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Player health.
/// </summary>
public class PlayerHealth : MonoBehaviour {

	
    [Header("Current Health")]
	public float health = 100;
    
	[Header("Max Health")]
	public float maxHealth = 100;
    
	[Header("Damage Value")]
	public float damageValue = 10;

	[HideInInspector]
	public Image damageImage;

	public float flashSpeed = 5f;

	Color defaultColour;

    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

	ParticleSystem hitParticles;

	bool damaged;

	[Header("Damage Audio Clip")]
    public AudioClip damageAudioClip;

	public AudioClip deathSound;
	
	CapsuleCollider capsuleCollider;
	
	public bool isDead;
    

	void Awake()
	{
		isDead = false;



		capsuleCollider = GetComponent <CapsuleCollider> ();

		hitParticles = GetComponentInChildren <ParticleSystem> ();

		if (GetComponent<PlayerManager> ().isLocalPlayer && GameObject.Find ("DamageImage"))
		{
			damageImage = GameObject.Find ("DamageImage").GetComponent<Image> () as Image;
		}

	}

	// Update is called once per frame
	void Update () {


	/*	if (GetComponent<PlayerManager> ().isLocalPlayer) {
			if (damaged) {
			
				damageImage.color = flashColour;

			} else {
				
				damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);

			}

			damaged = false;
		}
		*/
		

	}

	public void TakeDamage ()
	{
		if (GetComponent<PlayerManager> ().isLocalPlayer) {
			damaged = true;

			if (health - damageValue > 0) {
				health = health - damageValue;

				GetComponent<PlayerManager> ().UpdateAnimator ("IsDamage");

				hitParticles.transform.position = transform.position + capsuleCollider.center;
				hitParticles.Play ();
				PlayDamageSound();


			}
		}
		else
		{
			
			hitParticles.transform.position = transform.position + capsuleCollider.center;
			hitParticles.Play ();
			GetComponent<PlayerManager> ().UpdateAnimator ("IsDamage");

		
		}




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
		
}
