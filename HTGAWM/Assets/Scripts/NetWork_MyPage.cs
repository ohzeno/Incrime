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
    public Text MyGames;
    public Text MyWinGames;
    // Start is called before the first frame update

    public Image update_image;
	public Text text;
	public Button update_button1;
	public Button update_button2;

    public Image delete_image;

    void Start()
    {
        Application.ExternalCall("socket.emit", "USERINFOPAGE");
        update_image.gameObject.SetActive(false);
        delete_image.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckUserInfo(string data) {
        var pack = data.Split (Delimiter);

        MyName.text = pack[0];
        MyMail.text = pack[2];
        MyGames.text = pack[3].ToString();
        MyWinGames.text = pack[4].ToString();
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
        delete_image.gameObject.SetActive(false);

        SceneManager.LoadScene("StartScene");
    }

    public void CheckDelete() {
        update_image.gameObject.SetActive(false);
        delete_image.gameObject.SetActive(true);
    }

    public void DeleteCancel() {
        delete_image.gameObject.SetActive(false);
    }

    public void DeleteUser()
	{
        // JSON으로 묶어서 보냄 
		Application.ExternalCall("socket.emit", "USERDELETE");
		// server.js : JOIN 으로 가셈
	}

    public void UpdateSuccessMSG(string data) {
        update_image.gameObject.SetActive(true);
        delete_image.gameObject.SetActive(false);

        text.text = data;

        update_button1.gameObject.SetActive(true);
		update_button2.gameObject.SetActive(false);
    }

    public void UpdateSuccess() {
        update_image.gameObject.SetActive(false);

        // Application.ExternalCall("socket.emit", "USERINFOPAGE");
    }

    public void UpdateFailMSG(string data) {
        update_image.gameObject.SetActive(true);
        delete_image.gameObject.SetActive(false);

        text.text = data;

        update_button1.gameObject.SetActive(false);
		update_button2.gameObject.SetActive(true);
    }

    public void UpdateFail() {
        update_image.gameObject.SetActive(false);
    }

}
