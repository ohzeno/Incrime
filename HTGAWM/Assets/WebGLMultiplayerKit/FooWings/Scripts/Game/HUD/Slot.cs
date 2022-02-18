using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerShooter
{
public class Slot : MonoBehaviour {

	public int currentGun;
	public bool isFree;
	public Image weaponsImage;
	public Text txtAmmo;
	public Slider bulletSlider;
	public GameObject emptySpritePref;

	/// <summary>
	/// Sets the weapon.
	/// </summary>
	/// <param name="_weapon">Weapon.</param>
	public void SetWeapon(int _currentGun)
	{
		currentGun = _currentGun;
		NetworkManager NetworkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>() as NetworkManager;
	

		isFree = false;
	
		GetComponent<Button> ().interactable = true;
		weaponsImage.sprite = NetworkManager.localPlayer.
		GetComponentInChildren<Gun>().spriteWeapons[_currentGun].GetComponent<SpriteRenderer> ().sprite; 
	
	
			bulletSlider.GetComponent<Canvas> ().enabled = true;
			bulletSlider.maxValue = NetworkManager.localPlayer.GetComponentInChildren<Gun>().guns[_currentGun].ammoPerPents;
			bulletSlider.value = NetworkManager.localPlayer.GetComponentInChildren<Gun>().guns[_currentGun].ammoInPaint;
			int currentAmmoMinusAmmoInPaint = 0;
			currentAmmoMinusAmmoInPaint =  NetworkManager.localPlayer.GetComponentInChildren<Gun>().guns[_currentGun].
			currentAmmo - NetworkManager.localPlayer.GetComponentInChildren<Gun>().guns[_currentGun].ammoInPaint;
			txtAmmo.text = NetworkManager.localPlayer.GetComponentInChildren<Gun>().guns[_currentGun].ammoInPaint.ToString()+" / "+
				currentAmmoMinusAmmoInPaint.ToString();


	}


	/// <summary>
	/// Sets the empty weapon.
	/// </summary>
	public void SetEmptyWeapon()
	{
		isFree = true;

		GetComponent<Button> ().interactable = false;
		weaponsImage.sprite = emptySpritePref.GetComponent<SpriteRenderer> ().sprite;

		txtAmmo.text = string.Empty;
		bulletSlider.GetComponent<Canvas> ().enabled = false;

	}

	public void SelectThisWeapon()
	{
		NetworkManager NetworkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>() as NetworkManager;
	
		
		int lastGun = NetworkManager.localPlayer.GetComponentInChildren<Gun>().currentGun;
	    HUDWeaponManager.instance.SwitchGun(currentGun);
		HUDWeaponManager.instance.SetUpCurrentGun(currentGun);
		SetWeapon (lastGun);
		
	}

	public void ClearSlot()
	{
		isFree = true;
		GetComponent<Button> ().interactable = false;
		weaponsImage.sprite = emptySpritePref.GetComponent<SpriteRenderer> ().sprite;
		txtAmmo.text = string.Empty;
		bulletSlider.GetComponent<Canvas> ().enabled = false;
	}
}
}
