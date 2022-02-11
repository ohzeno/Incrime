using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetWork_MyPage : MonoBehaviour
{
    Text Username;

    static private readonly char[] Delimiter = new char[] {':'};

    // 게임 오브젝트
    [Header("Input field  :")]
    // 이름 입력하기
    public InputField MyName;
    public InputField MyPassword;
    public InputField MyMail;
    // Start is called before the first frame update
    void Start()
    {
        Application.ExternalCall("socket.emit", "USERINFOPAGE");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckUserInfo(string data) {
        var pack = data.Split (Delimiter);

        MyName.text = pack[0];
        MyPassword.text = pack[1];
        MyMail.text = pack[2];
    }

    public void UpdateUser()
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
		data["callback_name"] = "USERUPDATE";
        data["name"] = MyName.text;
        data["password"] = MyPassword.text;
        data["email"] = MyMail.text;

        Debug.Log("info : " + MyPassword.text + " " + MyMail.text);

        // JSON으로 묶어서 보냄 
		Application.ExternalCall("socket.emit", data["callback_name"], new JSONObject(data));
		// server.js : JOIN 으로 가셈

        // Application.ExternalCall("socket.emit", "USERINFOPAGE");
	}

    public void ChangeStartScene() {
        SceneManager.LoadScene("WaitScene");
    }

    public void DeleteUser()
	{
        // JSON으로 묶어서 보냄 
		Application.ExternalCall("socket.emit", "USERDELETE");
		// server.js : JOIN 으로 가셈
	}
}
