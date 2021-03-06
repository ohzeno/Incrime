
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
public class NewWork_Role : MonoBehaviour
{

    // 자기 자신을 instance로 선언
	public static NewWork_Role instance;
    //  ':' 로 분리할 것
	static private readonly char[] Delimiter = new char[] {':'};
    
    Text RoleDesc;
    Text StoryName;
    Text StoryDesc;
    public Text RoleAlibi;

    Text minute;
    Text second;

    bool memo_view;

    [Header("오브젝트 :")]
	int page;
    public GameObject Description1;
    public GameObject Description2;
    public GameObject btn_Conference;
    public InputField MemoInput;
    
        

    // Start is called before the first frame update
    void Start()
    {
        // 싱글 톤 코드
        if (instance == null) {
		    // 
            RoleDesc = GameObject.Find("txt_RoleDesc").GetComponent<Text>();
            RoleDesc.text = Client.role;
        
            StoryDesc = GameObject.Find("txt_StroyDesc").GetComponent<Text>();
            StoryDesc.text = Client.storydesc;
        
            StoryName = GameObject.Find("txt_StoryName").GetComponent<Text>();
            StoryName.text = Client.storyname;

            RoleAlibi.text = Client.alibi.Replace("\\n", "\n");

            page = 1;
            Description1.SetActive(true);
            Description2.SetActive(false);

            memo_view = false;
            MemoInput.gameObject.SetActive(false);

            } else {
			//it destroys the class if already other class exists
			Destroy(this.gameObject);
		}

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangePage(){
        if ( page == 1 ){
            page = 2;
            Description1.SetActive(false);
            Description2.SetActive(true);
        } else {
            page = 1;
            Description1.SetActive(true);
            Description2.SetActive(false);
        }
    }

    public void emitConference(){
        btn_Conference.SetActive(false);

        if (GameInfo.GameRoomInfo.roomReadyPlayer == 5 || GameInfo.GameRoomInfo.roomReadyPlayer == 6 )
        {
            Debug.Log("[System] Client : 첫 회의 하러 가자");
            Debug.Log("[system] 플레이어가 모두 준비됨 : " + Client.room);
            Application.ExternalCall("socket.emit", "GO_MEETING_SCENE", Client.room );

        }
        else
        {
            Client.ready = true;
            Application.ExternalCall("socket.emit", "READY_CRIMESCENE", Client.room);
        }

    }


    public void onRefreshReadyPlayer(int readyPlayer)
    {
        Debug.Log("[system] 준비된 플레이어 : " + readyPlayer);    
        GameInfo.GameRoomInfo.roomReadyPlayer = readyPlayer;
        if (GameInfo.GameRoomInfo.roomReadyPlayer >= 6)
        {
            Application.ExternalCall("socket.emit", "GO_MEETING_SCENE", Client.room);
        }
    }


    public void onConference(){
        Client.ready = false;
        GameInfo.GameRoomInfo.roomReadyPlayer = 0;
        Debug.Log("[System] Client : 첫 회의 하러 갑니다 ");
        
        // // create app if nonexistent
        // if (ReferenceEquals(app, null))
        // {
        //     app = new TestHelloUnityVideo(); // create app
        //     app.loadEngine("b16baf20b1fc49e99bd375ad30d5e340"); // load engine
        // }

        // // // join channel and jump to next scene
        // app.join("TEST", true);
        // SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
        SceneManager.LoadScene("MeetingScene", LoadSceneMode.Single);
    }

    // 역할 설정 시간 
    public void RoleTimer(string data){
        /*
         * data.pack[0] = time
         * data.pack[1]=  minute
         * data.pack[2] = second
        */
        var pack = data.Split (Delimiter);
        
        Client.minute = pack[1]; //set client name
        Client.second = pack[2];  //set client role

        // 
        minute = GameObject.Find("txt_min").GetComponent<Text>();
        minute.text = Client.minute;
        // 
        second = GameObject.Find("txt_second").GetComponent<Text>();
        second.text = Client.second;

    }

    public void onJoinButtonClicked()
    {
        Debug.Log("카메라 테스트 용입니다.");

        // SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
        SceneManager.LoadScene("MeetingScene", LoadSceneMode.Single);

        // //LoadSceneMode.single -> 현재 씬의 오브젝트를 날리고 감.
    }

    public void onJoinTest()
    {
        Debug.Log("테스트2");

        // SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
        SceneManager.LoadScene("MeetingScene", LoadSceneMode.Single);

        // //LoadSceneMode.single -> 현재 씬의 오브젝트를 날리고 감.
    }
    public void View_Memo()
    {
        if (memo_view == false)
        {
            // 메모 키기
            memo_view = true;
            MemoInput.gameObject.SetActive(true);
            MemoInput.text = Client.memo;
        }
        else
        {
            // 메모 끄기
            memo_view = false;
            MemoInput.gameObject.SetActive(false);
            Client.memo = MemoInput.text;
            // 저장
        }
    }




    }
}
