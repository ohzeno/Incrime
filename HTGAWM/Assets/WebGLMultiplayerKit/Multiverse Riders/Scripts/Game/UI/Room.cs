using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Text.RegularExpressions;
namespace MultiverseRiders
{
public class Room : MonoBehaviour
{
 
 public string id;

 public Text txtRoomName;

 public Text txtPlayers;

    
	public delegate void OnActionPress( GameObject unit, bool state );
	public event OnActionPress onPress;
	EventTrigger eventTrigger;


	void Start () {

		eventTrigger = this.gameObject.GetComponent<EventTrigger>();
		AddEventTrgger( OnPointDown, EventTriggerType.PointerDown);
		AddEventTrgger(OnPointUp, EventTriggerType.PointerUp);
		//AddEventTrgger(onClick, EventTriggerType.PointerClick);
	}


	void AddEventTrgger( UnityAction action, EventTriggerType triggerType ){

		EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
		trigger.AddListener( (eventData) => action());

		EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };
		eventTrigger.triggers.Add(entry);

	}


	void OnPointDown(){

		Debug.Log("user down:");

        ShooterNetworkManager.instance.EmitJoinGame(id);

		
		if( onPress != null  ){
			
			onPress(this.gameObject, true);

		}else{
			Debug.Log("Event null");
		}

	}

	void OnPointUp(){
		Debug.Log("user Up:");
		if( onPress != null  ){
			Debug.Log("OnPointUp");
			onPress(this.gameObject, false);
			
		}
	}

	

}
}