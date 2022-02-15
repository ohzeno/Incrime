
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

namespace Project
{
public class NetWork_Wait : MonoBehaviour
{
    // 자기 자신을 instance로 선언
	public static NetWork_Wait instance;
    //  ':' 로 분리할 것
	static private readonly char[] Delimiter = new char[] {':'};
    int index;
    int total;

    [Header("오브젝트 :")]
	public GameObject StartButton;  
    public GameObject Tuto;

    public Text Message;

    // Start is called before the first frame update
    void Start()
    { 
            
            // emitTotalplayer();

            // Debug.Log("테스트로그");
            // Debug.Log(Client.name);
            // Debug.Log(Client.room);

            total = Tuto.transform.childCount;

            index = 0;
            for(int i = 0; i < total; i++ ){
                Tuto.transform.GetChild(i).gameObject.SetActive(false);
            }
            Tuto.transform.GetChild(0).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void emitTotalplayer(){
        Debug.Log("[system] wait player update ");

        // Application.ExternalCall("socket.emit", "totalplayer");
    }

    void test(){
        Debug.Log("테스트용");
    }
    
    void onTotalplayer(string data)
    {
            //Debug.Log("[system] 준비된 플레이어 : " + readyPlayer);
            //GameInfo.GameRoomInfo.roomReadyPlayer = readyPlayer;
            //ReadyUserText.text = readyPlayer.ToString();

            var pack = data.Split (Delimiter);
        Debug.Log("총 유저 수를 유저를 업데이트 한다. >> " + pack[0] + " -- " + pack[1] );
        // 전체 인원이 6이고 내가 6번째인원인 경우만 가능하다.
        if ( pack[0] == "6" && pack[1] == "6" ) {  
            StartButton.SetActive(true);
        }
        Text txt_player = GameObject.Find("txt_player").GetComponent<Text>();
		txt_player.text = pack[0];
    }

    public void emitSetRole(){
        Debug.Log("[system] 역할 배정하기");
        StartButton.SetActive(false);

        if ( GameInfo.GameRoomInfo.roomReadyPlayer == 5 ){
                Debug.Log("[system] 플레이어가 모두 준비됨 : " + Client.room);
                Application.ExternalCall("socket.emit", "SET_ROLE", Client.room);

        } else {
                Client.ready = true;
                Application.ExternalCall("socket.emit", "READY_CRIMESCENE", Client.room);
        }
    }

    public void onRefreshReadyPlayer(int readyPlayer)
    {
        Debug.Log("[system] 준비된 플레이어 : " + readyPlayer);
        GameInfo.GameRoomInfo.roomReadyPlayer = readyPlayer;
        Text txt_player = GameObject.Find("txt_player").GetComponent<Text>();
        txt_player.text = readyPlayer.ToString();
    }

    void OnSetRole(string data){
        var pack = data.Split (Delimiter);

	    Client.role = pack[0];
        Client.storyname = pack[1];
        Client.storydesc = pack[2];
        Client.alibi = pack[3];
        Debug.Log("[system] 알리바이" + pack[3]);

        Client.ready = false;
        GameInfo.GameRoomInfo.roomReadyPlayer = 0;

        Debug.Log("[system] 역할을 배정 받았습니다. 역할 설명화면으로 갑니다. ");
        SceneManager.LoadScene("RoleScene");
    }

    void showTutorial(int index){
        Tuto.transform.GetChild(index).gameObject.SetActive(true);        
    }

    public void nextTuto(){
        Tuto.transform.GetChild(index).gameObject.SetActive(false);  
        index += 1;
        if ( index == total ) index = 0 ;
        Tuto.transform.GetChild(index).gameObject.SetActive(true); 
    
    }
    
    public void beforeTuto(){
        Tuto.transform.GetChild(index).gameObject.SetActive(false);  
        index -= 1;
        if ( index == -1) index = total-1 ;
        Tuto.transform.GetChild(index).gameObject.SetActive(true); 
    }

    public void ChangeMyPageScene() {
        SceneManager.LoadScene("MyPageScene");
    }
}
}
