using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameInfo;

public class RoomUser : MonoBehaviour
{
    public int userNoByRoom;
    public string userId;
    public TextMeshProUGUI userNameTextMesh;

    private AgoraController agoraController;

    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        agoraController = AgoraController.GetAgoraControllerInstance();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnChangedVolumeSlider(float volume)
    {
        if (userNoByRoom == GameRoomInfo.userNoByRoom)
        {
            agoraController.SetMyVolume(volume);
        }
        else
        {
            Debug.Log(userId + "의 볼륨이 변함");
            agoraController.SetVolumeByUserId(userNoByRoom, volume);
        }
    }
}
