
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using agora_gaming_rtc;


namespace Project 
{
public class PlayerObject : MonoBehaviour
{
    public static PlayerObject instance;
    static private readonly char[] Delimiter = new char[] {':'};
    bool memo_view;

    [Header("Object :")]
    public Text Timer;
    public InputField MemoInput;
    public GameObject aimdot;

    private void Awake()
    {
        if (instance == null) { 
            // PlayerObject = (GameObject)Instantiate(Resources.Load("Prefab/block")); 
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        } 
        else { 
            Destroy(this.gameObject); 
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        memo_view = false;
        MemoInput.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 메모
    public void View_Memo(){
        if ( memo_view == false ){
            // 메모 키기
            memo_view = true;
            MemoInput.gameObject.SetActive(true);
            MemoInput.text = Client.memo;
        } else {
            // 메모 끄기
            memo_view = false;
            MemoInput.gameObject.SetActive(false);
            Client.memo =  MemoInput.text;
            // 저장
        }
    }

    // 인게임 타이머 
    public void InGameTimer(string data){
        /*
         * data.pack[0] = time
         * data.pack[1]=  minute
         * data.pack[2] = second
        */
        var pack = data.Split (Delimiter);

        // Debug.Log(pack);
        
        Client.minute = pack[1]; //set client name
        Client.second = pack[2];  //set client role
        
        var timetxt = "";
        timetxt = pack[1] + ":" + pack[2];
        Timer.text = timetxt;

    }

    // 탐색 종료 미팅씬으로 돌아가기
    public void onMeeting(){
        Debug.Log("[System] Client : 탐색이 끝났습니다. 회의실로 갑니다. ");

        // 탐색 종료 후
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MeetingScene", LoadSceneMode.Single);
    }

    // 투표씬으로 가기
    public void onVote()
    {
        Debug.Log("[System] Client : 첫 투표 하러 갑니다 ");
        SceneManager.LoadScene("VoteScene");
    }


    }
}