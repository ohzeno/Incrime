using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerShooter
{
public class HUDWeaponManager : MonoBehaviour {


	public static  HUDWeaponManager instance;

	public GameObject weaponsContainer;

    [Header("Slot Prefab")]
	public GameObject slotPref;

	public Image currentWeaponImage;

	public Text txtCurrentAmmoInPent;

	public Slider currentBulletSlider;


	public int currentGun;

	public ArrayList inventorySlots = new ArrayList();



	// Use this for initialization
	void Start () {

		if (instance == null) {

			DontDestroyOnLoad (this.gameObject);
			instance = this;

		}
		else
		{
			Destroy(this.gameObject);
		}

	}

	/// <summary>
	/// Inicializes the weapon inventory.
	/// </summary>
	public void InicializeInventory ()
	{

	
		foreach (KeyValuePair<int, CustonGun> entry in NetworkManager.instance.localPlayer.GetComponentInChildren<Gun>().guns)
		{
            //  check that it is not the player's current primary weapon
			if (!NetworkManager.instance.localPlayer.GetComponentInChildren<Gun>().
			guns[currentGun].Equals (entry.Value)) {
				
				//spawn slot weapon game object
				GameObject slotInstance = Instantiate (slotPref) as GameObject;
			    slotInstance.transform.parent = weaponsContainer.transform;
				slotInstance.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);

				Slot new_slot = slotInstance.GetComponent<Slot> ();

                //checks if the player has the weapon in the guns inventory
				if (!NetworkManager.instance.localPlayer.GetComponentInChildren<Gun>().guns[currentGun].Equals (entry.Value)) {
                    
					//add gun on slot
					new_slot.SetWeapon(entry.Value.id);

				} else {
					//set with empty sprite 
					new_slot.SetEmptyWeapon ();
				}

				inventorySlots.Add (slotInstance.GetComponent<Slot> ());

			}
		
			}

			
	}





	/// <summary>
	/// Switchs the weapon.
	/// </summary>
	/// <param name="_weapon">Weapon.</param>
	public void SwitchGun(int _currentGun)
	{
	    currentGun = _currentGun;
		NetworkManager NetworkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>() as NetworkManager;
		NetworkManager.localPlayer.GetComponentInChildren<Gun>().currentGun = _currentGun;

		SetUpCurrentGun(_currentGun);
	}


	

	/// <summary>
	///set the current weapon of the player
	/// </summary>
	/// <param name="_weapon">Weapon.</param>
	public void SetUpCurrentGun(int _currentGun)
	{

		NetworkManager NetworkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>() as NetworkManager;
		
	    currentWeaponImage.sprite = NetworkManager.localPlayer.GetComponentInChildren<Gun>().spriteWeapons[_currentGun].GetComponent<SpriteRenderer> ().sprite;

		currentBulletSlider.GetComponent<Canvas> ().enabled = true;

		currentBulletSlider.maxValue = 	NetworkManager.localPlayer.GetComponentInChildren<Gun>().guns[_currentGun].ammoPerPents;

		currentBulletSlider.value = NetworkManager.localPlayer.GetComponentInChildren<Gun>().guns[_currentGun].ammoInPaint;

		int currentAmmoMinusAmmoInPaint = 0;

		currentAmmoMinusAmmoInPaint = NetworkManager.localPlayer.GetComponentInChildren<Gun>().guns[_currentGun].currentAmmo - 	NetworkManager.localPlayer.GetComponentInChildren<Gun>().guns[_currentGun].ammoInPaint;

		if (currentAmmoMinusAmmoInPaint < 0)
		{
			currentAmmoMinusAmmoInPaint =	NetworkManager.localPlayer.GetComponentInChildren<Gun>().guns[_currentGun].ammoInPaint;
		}

		txtCurrentAmmoInPent.text = NetworkManager.localPlayer.GetComponentInChildren<Gun>().guns[_currentGun].ammoInPaint.ToString()+" / "+
				currentAmmoMinusAmmoInPaint.ToString();
		


		
	}

	

		/// <summary>
	/// Updates the current ammo info.
	/// </summary>
	public void UpdateCurrentAmmoInfo()
	{

        
		NetworkManager NetworkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>() as NetworkManager;
		
	
		currentWeaponImage.sprite = NetworkManager.localPlayer.GetComponentInChildren<Gun>().spriteWeapons[currentGun].GetComponent<SpriteRenderer> ().sprite;

				currentBulletSlider.GetComponent<Canvas> ().enabled = true;

				currentBulletSlider.maxValue = NetworkManager.localPlayer.GetComponentInChildren<Gun>().guns[currentGun].ammoPerPents;

				currentBulletSlider.value =NetworkManager.localPlayer.GetComponentInChildren<Gun>().guns[currentGun].ammoInPaint;

				int currentAmmoMinusAmmoInPaint = 0;

				currentAmmoMinusAmmoInPaint = NetworkManager.localPlayer.GetComponentInChildren<Gun>().guns[currentGun].currentAmmo - NetworkManager.localPlayer.GetComponentInChildren<Gun>().guns[currentGun].ammoInPaint;

				if (currentAmmoMinusAmmoInPaint < 0) {
					currentAmmoMinusAmmoInPaint = NetworkManager.localPlayer.GetComponentInChildren<Gun>().guns[currentGun].ammoInPaint;
				}

				txtCurrentAmmoInPent.text = NetworkManager.localPlayer.GetComponentInChildren<Gun>().guns[currentGun].ammoInPaint.ToString () + " / " +
				currentAmmoMinusAmmoInPaint.ToString ();

	

	}

	public void ClearTxtAmmoInPaint()
	{


		txtCurrentAmmoInPent.text = " ";

	}
	

}
}
