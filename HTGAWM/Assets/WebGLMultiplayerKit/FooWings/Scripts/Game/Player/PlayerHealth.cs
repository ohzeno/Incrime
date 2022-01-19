using UnityEngine;
using System.Collections;
using UnityEngine.UI;


/// <summary>
/// Player health.
/// </summary>
namespace MultiplayerShooter
{
public class PlayerHealth : MonoBehaviour {

	
	public Image damageImage;

	[Header("Health Slider")]
	public Slider healthSlider;

	[Header("Public References")]
    public int startingHealth = 100;

	public float health = 100;

	public float maxHealth = 100;

	public bool onDamageAnim;

    public float damageAnimTime = 3;

    public AudioClip damageSound;

	public AudioClip deathSound;

	public bool isDead;

	public AudioClip colectedSound;
	
   public GameObject explosionPref;  // set in inspector.


	void Awake()
	{
		isDead = false;
		healthSlider.GetComponent<Canvas>().enabled = false;
		healthSlider.maxValue = startingHealth;
	    healthSlider.value = startingHealth;


	}

	 public void TakeDamage (float _health)
    {
        if(isDead)
            return;

		healthSlider.value = _health;

		StartCoroutine ("DamageAnimation");

			
    }


	/// <summary>
	/// Tries the atack.
	/// </summary>
	private IEnumerator DamageAnimation()
	{
		

		if (onDamageAnim)
		{
			yield break; // if already atack
		}
	
		healthSlider.GetComponent<Canvas>().enabled = true;
   
        onDamageAnim = true;

		yield return new WaitForSeconds(damageAnimTime);

		healthSlider.GetComponent<Canvas>().enabled = false;

		onDamageAnim = false;
	
	}
	
	 public void Death (string _shooterID)
    {
		health = 0;

	    isDead = true;

		Instantiate (explosionPref, transform.position, transform.rotation);
		   
    }
	
		
}//END_CLASS
}//END_NAMESPACE
