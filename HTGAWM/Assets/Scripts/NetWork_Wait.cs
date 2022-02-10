
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
        // 싱글 톤 코드
        if (instance == null) {
		    // 
            // 만약 다른 씬이 있으면 오브젝트를 파괴하지 않음
            // it doesn't destroy the object, if other scene be loaded
		    DontDestroyOnLoad (this.gameObject);
			instance = this;// define the class as a static variable        
            StartButton.SetActive(false);
            emitTotalplayer();

            total = Tuto.transform.childCount;

            index = 0;
            for(int i = 0; i < total; i++ ){
                Tuto.transform.GetChild(i).gameObject.SetActive(false);
            }
            Tuto.transform.GetChild(0).gameObject.SetActive(true);
        }
		else
		{
			//it destroys the class if already other class exists
			Destroy(this.gameObject);
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void emitTotalplayer(){
        Debug.Log("대기 페이지로 들어왔다.");
        Application.ExternalCall("socket.emit", "totalplayer");
    }

    void test(){
        Debug.Log("테스트용");
    }
    
    void onTotalplayer(string data)
    {
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
        Debug.Log("역할 배정하기");
        Application.ExternalCall("socket.emit", "setrole");
    }

    void OnSetRole(string data){
        var pack = data.Split (Delimiter);

        string myrole = pack[2]; // myrole
        // string storyname = pack[3];
        // string storydesc = pack[4];
        
		Client.role = myrole;
        Client.storyname = pack[3];
        Client.storydesc = pack[4];

        Debug.Log("역할을 배정 받았습니다. 역할 설명화면으로 갑니다. ");
        // Debug.Log(myrole);   
        // Debug.Log(storyname);   
        // Debug.Log(storydesc);   

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
}
}
