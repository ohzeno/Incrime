
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.Text;
using System.Runtime.InteropServices;
using agora_gaming_rtc;


public class MoveTo : MonoBehaviour
{
    static TestHelloUnityVideo app;


    void Start()
    {
    }

    public void ToTeamLeaderLeeOffice()
    {
        app = new TestHelloUnityVideo();
        app.loadEngine("b16baf20b1fc49e99bd375ad30d5e340");
        Debug.Log("[system] 팀장실로 입장합니다.");
        app.join( Client.room + "TeamLeaderLeeOffice", true);
        
        SceneManager.LoadScene("TeamLeaderLeeOffice");
    }

    public void ToSecurityRoom()
    {
        app = new TestHelloUnityVideo(); //
        app.loadEngine("b16baf20b1fc49e99bd375ad30d5e340");
        Debug.Log("[system] 보안실로 입장합니다.");
        app.join( Client.room + "SecurityRoom", true);

        SceneManager.LoadScene("SecurityRoom");
    }

    public void ToReceptionDesk()
    {
        app = new TestHelloUnityVideo(); // 
        app.loadEngine("b16baf20b1fc49e99bd375ad30d5e340"); 
        Debug.Log("[system] 비서실로 입장합니다.");
        app.join( Client.room + "ReceptionDesk", true);

        SceneManager.LoadScene("ReceptionDesk");
    }

    public void ToOfficeCubicles()
    {
        app = new TestHelloUnityVideo(); 
        app.loadEngine("b16baf20b1fc49e99bd375ad30d5e340"); 
        Debug.Log("[system] 사무실로 입장합니다.");
        app.join( Client.room +  "OfficeCubicles", true);

        SceneManager.LoadScene("OfficeCubicles");
    }

    public void ToMeetingRoom()
    {
        app = new TestHelloUnityVideo();
        app.loadEngine("b16baf20b1fc49e99bd375ad30d5e340"); 
        Debug.Log("[system] 회의실로 입장합니다.");
        app.join( Client.room +  "MeetingRoom", true);

        SceneManager.LoadScene("MeetingRoom");
    }

    public void ToRestRoom()
    {
        app = new TestHelloUnityVideo(); 
        app.loadEngine("b16baf20b1fc49e99bd375ad30d5e340");
        Debug.Log("[system] 화장실로 입장합니다.");
        app.join( Client.room + "RestRoom", true);

        SceneManager.LoadScene("RestRoom");
    }

    public void ToDirectorMaOffice()
    {  
        app = new TestHelloUnityVideo(); 
        app.loadEngine("b16baf20b1fc49e99bd375ad30d5e340"); 
        Debug.Log("[system] 마이사 방으로 입장합니다.");
        app.join( Client.room + "DirectorMaOffice", true);

        SceneManager.LoadScene("DirectorMaOffice");
    }

    public void ToBreakRoom()
    {
        app = new TestHelloUnityVideo();
        app.loadEngine("b16baf20b1fc49e99bd375ad30d5e340"); 
        Debug.Log("[system] 탕비실로 입장합니다.");
        app.join( Client.room + "BreakRoom", true);

        SceneManager.LoadScene("BreakRoom");
    }

    public void ToHallway()
    {
        app = new TestHelloUnityVideo(); 
        app.loadEngine("b16baf20b1fc49e99bd375ad30d5e340"); 
        Debug.Log("[system] 복도로 입장합니다.");
        app.join( Client.room + "Hallway", true);

        SceneManager.LoadScene("Hallway");
    }
}
