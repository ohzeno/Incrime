using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ResultPageScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickExitButton()
    {
        Application.ExternalCall("socket.emit", "END_GAME", Client.room, Client.win, Client.name);
        Client.role = "";
        Client.storyname = "";
        Client.storydesc = "";
        Client.memo = "";
        Client.ready = false;
        Client.meeting_num = 0;
        Client.win = 0;
        SceneManager.LoadScene("Lobby");

    }
}
