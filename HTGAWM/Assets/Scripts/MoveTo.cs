
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
    void Start()
    {

    }

    public void ToTeamLeaderLeeOffice()
    {
        Debug.Log("[system] 팀장실로 입장합니다.");
        
        SceneManager.LoadScene("TeamLeaderLeeOffice");
    }

    public void ToSecurityRoom()
    {
        Debug.Log("[system] 보안실로 입장합니다.");

        SceneManager.LoadScene("SecurityRoom");
    }

    public void ToReceptionDesk()
    {
        Debug.Log("[system] 비서실로 입장합니다.");

        SceneManager.LoadScene("ReceptionDesk");
    }

    public void ToOfficeCubicles()
    {
        Debug.Log("[system] 사무실로 입장합니다.");
        SceneManager.LoadScene("OfficeCubicles");
    }

    public void ToMeetingRoom()
    {
        Debug.Log("[system] 회의실로 입장합니다.");

        SceneManager.LoadScene("MeetingRoom");
    }

    public void ToRestRoom()
    {
        Debug.Log("[system] 화장실로 입장합니다.");

        SceneManager.LoadScene("RestRoom");
    }

    public void ToDirectorMaOffice()
    {
        Debug.Log("[system] 마이사 방으로 입장합니다.");

        SceneManager.LoadScene("DirectorMaOffice");
    }

    public void ToBreakRoom()
    {
        Debug.Log("[system] 탕비실로 입장합니다.");

        SceneManager.LoadScene("BreakRoom");
    }

    public void ToHallway()
    {
        Debug.Log("[system] 복도로 입장합니다.");

        SceneManager.LoadScene("Hallway");
    }
}
