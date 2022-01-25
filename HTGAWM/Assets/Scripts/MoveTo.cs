using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveTo : MonoBehaviour
{
    public Button MoveButton;

    void Start()
    {
        MoveButton.GetComponent<Image>().alphaHitTestMinimumThreshold = 1f;
    }

    public void ToTeamLeaderLeeOffice()
    {
        SceneManager.LoadScene("TeamLeaderLeeOffice");
    }

    public void ToSecurityRoom()
    {
        SceneManager.LoadScene("SecurityRoom");
    }

    public void ToReceptionDesk()
    {
        SceneManager.LoadScene("ReceptionDesk");
    }

    public void ToOfficeCubicles()
    {
        SceneManager.LoadScene("OfficeCubicles");
    }

    public void ToMeetingRoom()
    {
        SceneManager.LoadScene("MeetingRoom");
    }

    public void ToRestRoom()
    {
        SceneManager.LoadScene("RestRoom");
    }

    public void ToDirectorMaOffice()
    {
        SceneManager.LoadScene("DirectorMaOffice");
    }

    public void ToBreakRoom()
    {
        SceneManager.LoadScene("BreakRoom");
    }

    public void ToHallway()
    {
        SceneManager.LoadScene("Hallway");
    }
}
