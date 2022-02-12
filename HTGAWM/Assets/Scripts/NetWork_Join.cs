using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class NetWork_Join : MonoBehaviour
{
    // 자기 자신을 instance로 선언
	public static NetWork_Join instance;

    // 모든 연결된 클라이언트 저장
	//public Dictionary<string, Client> connectedClients = new Dictionary<string, Client>();
    
    // flag which is determined the player is logged in the arena
	public bool onLogged = false;
    string local_player_id;
    
    //  ':' 로 분리할 것
	static private readonly char[] Delimiter = new char[] {':'};
	
    // 게임 오브젝트
    [Header("Input field  :")]
    // 이름 입력하기
    public InputField JoinName;
    public InputField JoinPassword;
    public InputField JoinMail;

    public GameObject joinSuccessBox;
	public Text text;


    // Start is called before the first frame update
    void Start()
    {
        joinSuccessBox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JoinUser()
	{
        // 키 밸류 데이터 
		Dictionary<string, string> data = new Dictionary<string, string>();
		
		// 플레이어 이름
		data["name"] = "empty";
        data["password"] = "empty";
        data["email"] = "empty";
        //StartScene.instance.Login.text; 이런식은 안됨 왜 안되는지 모르겠지만
        // 또는 
        // GameObject.Find("JoinName").GetComponent<Text>();

		string msg = string.Empty;
		data["callback_name"] = "USERJOIN";
        data["name"] = JoinName.text;
        data["password"] = JoinPassword.text;
        data["email"] = JoinMail.text;

        // JSON으로 묶어서 보냄 
		Application.ExternalCall("socket.emit", data["callback_name"], new JSONObject(data));
		// server.js : JOIN 으로 가셈
	}

    public void JoinSuccess(string data) {
		joinSuccessBox.SetActive(true);
		text.text = data;
    }

    public void ChangeStartScene() {
        joinSuccessBox.SetActive(false);
        SceneManager.LoadScene("StartScene");
    }

}
