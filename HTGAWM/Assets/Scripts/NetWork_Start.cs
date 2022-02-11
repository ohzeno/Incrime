
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

public class NetWork_Start : MonoBehaviour
{
    // 자기 자신을 instance로 선언
	public static NetWork_Start instance;

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
            Debug.Log(" ------- WELCOME ------");
		}
		else
		{
			//it destroys the class if already other class exists
			// Destroy(this.gameObject);
		}
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	/// <summary>
    //    method triggered by the BtnLogin button
	/// EmitToServers the player's name to server.
    /// 로그인 버튼 누르면 나오는거
	/// </summary>
	public void EmitJoinRoom()
	{
        // 키 밸류 데이터 
		Dictionary<string, string> data = new Dictionary<string, string>();
		
		// 플레이어 이름
		data["name"] = "empty";
		data["password"] = "empty";
        //StartScene.instance.Login.text; 이런식은 안됨 왜 안되는지 모르겠지만
        // 또는 
        // GameObject.Find("JoinName").GetComponent<Text>();

		string msg = string.Empty;
		data["callback_name"] = "JOIN";
        data["name"] = JoinName.text;
		data["password"] = JoinPassword.text;

        // JSON으로 묶어서 보냄 
		Application.ExternalCall("socket.emit", data["callback_name"], new JSONObject(data));
		// server.js : JOIN 으로 가셈
	}


    /// <summary>
	/// 플레이어가 조인이 성공했을 때 
	/// </summary>
	/// <remarks>
    /// </remarks>
	/// <param name="_data">Data.</param>
	public void OnJoinGame(string data)
	{
		
		/*
		 * data.pack[0] = id (local player id)
		 * data.pack[1]= username (local player name)
		 * data.pack[2] = " local user avatar index"
		*/

        var pack = data.Split (Delimiter);
		
        Debug.Log(" 성공적으로 들어왔습니다.");
        // the local player now is logged
		onLogged = true;

		// 류기탁 사용
        Variables.totalplayer = pack[2];
		Client.id = pack[0];
		Client.name = pack[1];
		Client.isLocalPlayer = true;

		string tempname = Client.name;

		StartScene.instance.SetUpProfile(tempname);
        
		Debug.Log(Client.name + " 은 대기화면으로 갑니다. 총 인원 수 : " + Variables.totalplayer);
        SceneManager.LoadScene("WaitScene");
		
	}

	public void ChangeJoinScene() {
		SceneManager.LoadScene("JoinScene");
	}

}
}