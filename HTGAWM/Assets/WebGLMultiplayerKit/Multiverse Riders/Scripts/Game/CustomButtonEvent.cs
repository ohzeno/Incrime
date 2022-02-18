using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MultiverseRiders
{
public class CustomButtonEvent : MonoBehaviour {

	public delegate void OnActionPress( GameObject unit, bool state );
	
	public event OnActionPress onPress;
	EventTrigger eventTrigger;
	
	
	


	void Start () {

		eventTrigger = this.gameObject.GetComponent<EventTrigger>();
		AddEventTrgger( OnPointDown, EventTriggerType.PointerDown);
		AddEventTrgger(OnPointUp, EventTriggerType.PointerUp);
		
	}


	void AddEventTrgger( UnityAction action, EventTriggerType triggerType ){

		EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
		trigger.AddListener( (eventData) => action());

		EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };
		eventTrigger.triggers.Add(entry);

	}


	void OnPointDown(){

   
		  ShooterNetworkManager.instance.myPlayer.GetComponent<Player2DManager>().EnableKey (gameObject.name);
	
		if( onPress != null  ){
			//Debug.Log("OnPointDown");
			onPress(this.gameObject, true);

		}else{
			//Debug.Log("Event null");
		}

	}

	void OnPointUp(){
	
		  ShooterNetworkManager.instance.myPlayer.GetComponent<Player2DManager>().DisableKey (gameObject.name);
		
		if( onPress != null  ){
			//Debug.Log("OnPointUp");
			onPress(this.gameObject, false);
			
		}
	}

}
}
