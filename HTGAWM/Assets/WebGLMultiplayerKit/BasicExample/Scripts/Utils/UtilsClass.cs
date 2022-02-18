using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

    /*
     * Various assorted utilities functions
     * */
    public static class UtilsClass {
		
		public static float StringToFloat(string target ){

		float value;

		if(target.ToLower().Contains(","))
		 {
		   CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ",";
			
			value = float.Parse (target,NumberStyles.Any,ci);
			
		 }
		 else
		 {
		   CultureInfo ci2 = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci2.NumberFormat.CurrencyDecimalSeparator = ".";
			
			value = float.Parse (target,NumberStyles.Any,ci2);
			
			
		 }

		return value;

	}

	 
	public static Vector3 StringToVector3(string target ){

		Vector3 newVector;
		string[] newString = Regex.Split(target,";");
		if(target.ToLower().Contains(","))
		 {
		   CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ",";
			
		newVector = new Vector3(float.Parse (newString[0],NumberStyles.Any,ci), float.Parse (newString[1],NumberStyles.Any,ci) ,
	     float.Parse (newString[2],NumberStyles.Any,ci));
		 }
		 else
		 {
		   CultureInfo ci2 = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci2.NumberFormat.CurrencyDecimalSeparator = ".";
			
			newVector = new Vector3(float.Parse (newString[0],NumberStyles.Any,ci2), float.Parse (newString[1],NumberStyles.Any,ci2) ,
		float.Parse (newString[2],NumberStyles.Any,ci2));
			
		 }

		return newVector;

	}
	
	public static Vector4 StringToVector4(string target ){

		Vector4 newVector;
		string[] newString = Regex.Split(target,";");
		if(target.ToLower().Contains(","))
		 {
		   CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ",";
			
		newVector = new Vector4( float.Parse(newString[0],NumberStyles.Any,ci), float.Parse(newString[1],NumberStyles.Any,ci)
		,float.Parse(newString[2],NumberStyles.Any,ci),float.Parse(newString[3],NumberStyles.Any,ci));
		
		}
		else
		{
		  CultureInfo ci2 = (CultureInfo)CultureInfo.CurrentCulture.Clone();
             ci2.NumberFormat.CurrencyDecimalSeparator = ".";
			
			newVector = new Vector4( float.Parse(newString[0],NumberStyles.Any,ci2), float.Parse(newString[1],NumberStyles.Any,ci2)
		,float.Parse(newString[2],NumberStyles.Any,ci2),float.Parse(newString[3],NumberStyles.Any,ci2));
		
			
		}

		return newVector;

	}
	
	public static string Vector3ToString(Vector3 vet ){

		return  vet.x+";"+vet.y+";"+vet.z;

	}
	
	public static string Vector4ToString(Vector4 vet ){

		return  vet.x+";"+vet.y+";"+vet.z+";"+vet.w;

	}
	
	public static string generateID()
	{
		return Guid.NewGuid().ToString("N");
	}


	}
