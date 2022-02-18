using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace FPSExample{
public class  CustonGun {

	public int  id;

	public string name;

	public enum TypeOfWeapon { GUN,M1,M4,RPG}; // use burst for single shot weapons like pistols / sniper rifles
	
	public TypeOfWeapon type;

	//total ammo in Weapon Pent
	public int ammoInPent;

	// current ammo in inventory
	public int currentAmmo;

	// max ammunition for Pents
	public int ammoPerPents;

	// max ammo in inventory
	public int maxAmmo;
	
	public bool onInventory;

	public bool isDropped;
	
	public string weaponTAG;

	public void SetTypeOfWeapon(int _type)
	{
		switch (_type)
        {
			case 0:
			type = TypeOfWeapon.GUN;
			weaponTAG = "GUN";
		
			break;
			case 1:
			type = TypeOfWeapon.M1;
			weaponTAG = "M1";
			break;
			case 2:
			type = TypeOfWeapon.M4;
			weaponTAG = "M4";
			break;
			case 3:
			type = TypeOfWeapon.RPG;
			weaponTAG = "RPG";
			break;
		
	}

}
   public string GetTypeOfWeapon()
	{
		switch (type)
        {
			case TypeOfWeapon.GUN:
			return "GUN";
			break;
			case TypeOfWeapon.M1:
			return"M1";
			break;
			case TypeOfWeapon.M4:
			return "M4";
			break;
			case TypeOfWeapon.RPG:
			return"RPG";
			break;
                 
	}
	return string.Empty;
		
	}
}
}
