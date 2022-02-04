
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

    Text minute;
    Text second;

    [Header("오브젝트 :")]
	int page;
    public GameObject Description1;
    public GameObject Description2;

    public GameObject btn_Conference;
    
    // 비디오용
    static TestHelloUnityVideo app = null;
        

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

            page = 1;
            Description1.SetActive(true);
            Description2.SetActive(false);
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
        Debug.Log("[System] Client : 첫 회의 하러 가자");
        btn_Conference.SetActive(false);
        Application.ExternalCall("socket.emit", "go_firstconference");

    }

    public void onConference(){
        Debug.Log("[System] Client : 첫 회의 하러 갑니다 ");
        
         // create app if nonexistent
        if (ReferenceEquals(app, null))
        {
            app = new TestHelloUnityVideo(); // create app
            app.loadEngine("b16baf20b1fc49e99bd375ad30d5e340"); // load engine
        }

        // // join channel and jump to next scene
        app.join("TEST", true);
        // SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
        SceneManager.LoadScene("SceneHelloVideo", LoadSceneMode.Single);


        // SceneManager.LoadScene("RTCScene");
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

        // create app if nonexistent
        if (ReferenceEquals(app, null))
        {
            app = new TestHelloUnityVideo(); // create app
            app.loadEngine("b16baf20b1fc49e99bd375ad30d5e340"); // load engine
        }

        // // join channel and jump to next scene
        app.join("TEST", true);
        // SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
        SceneManager.LoadScene("SceneHelloVideo", LoadSceneMode.Single);

        // //LoadSceneMode.single -> 현재 씬의 오브젝트를 날리고 감.
    }

    public void onJoinTest()
    {
        Debug.Log("테스트2");

        // create app if nonexistent
        if (ReferenceEquals(app, null))
        {
            app = new TestHelloUnityVideo(); // create app
            app.loadEngine("b16baf20b1fc49e99bd375ad30d5e340"); // load engine
        }

        // // join channel and jump to next scene
        app.join("TEST", true);
        // SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
        SceneManager.LoadScene("SceneHelloVideo", LoadSceneMode.Single);

        // //LoadSceneMode.single -> 현재 씬의 오브젝트를 날리고 감.
    }


}
}
