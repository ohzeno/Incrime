using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using User;
using GameInfo;
using UnityEngine.SceneManagement;

public class LobbyController : MonoBehaviour
{
    public enum State
    {
        Create,
        Join
    }
    public enum PasswordState
    {
        Open,
        Close
    }

    public State CurrentState = State.Create;
    public PasswordState CurrntPasswordState = PasswordState.Close;
    public GameObject PlayCrimeSceneButton;
    public GameObject PlayCrimeSceneButton_cancel;
    public Text ReadyUserText;

    [Serializable]
    public class RoomsWrapper
    {
        public Room[] rooms;
    }
    [Serializable]
    public class Room
    {
        public int waitingroom_no;
        public string waitingroom_nm;
        public string waitingroom_pw;
        public string waitingroom_host_id;
        public string waitingroom_status;
        public int story_no;
        public int people_count;
        public bool is_password;

        public int waitingroom_ready;

        public Room(string waitingroom_nm, string waitingroom_pw, int story_no)
        {
            this.waitingroom_nm = waitingroom_nm;
            this.waitingroom_pw = waitingroom_pw;
            this.story_no = story_no;
        }
    }

    [SerializeField]
    private GameObject createTab;
    [SerializeField]
    private GameObject joinTab;

    [SerializeField]
    private GameObject roomListParent;

    [SerializeField]
    private TMP_InputField createRoomName;

    [SerializeField]
    private TMP_InputField createRoomPassword;

    private List<GameObject> roomObjectList = new List<GameObject>();
    // Start is called before the first frame update

    [SerializeField]
    private Button createTabButton;
    [SerializeField]
    private Button joinTabButton;

    [SerializeField]
    private Button refreshButton;

    [SerializeField]
    private GameObject roomInnerUIObject;

    [SerializeField]
    private GameObject userListParent;

    private RoomUser[] roomUsers;

    public RoomInfo roomInfo;

    private RoomUIWrapper currentPickRoom;

    public RoomUIWrapper tempRoomUI;

    [SerializeField]
    private GameObject passwordInputBoxObject;

    [SerializeField]
    private TMP_InputField joinRoomPassword;

    private AgoraController agoraController;


    [SerializeField]
    private CanvasGroup menuCanvasGroup;

    private bool isFIrst = true;

    void Start()
    {
        roomUsers = userListParent.GetComponentsInChildren<RoomUser>(true);

        if (Client.ready == true)
        {
            PlayCrimeSceneButton.SetActive(false);
            PlayCrimeSceneButton_cancel.SetActive(true);
        }
        else
        {
            PlayCrimeSceneButton.SetActive(true);
            PlayCrimeSceneButton_cancel.SetActive(false);
        }

        OnClickCreateTabButton();
        roomInnerUIObject.SetActive(false);
        PlayCrimeSceneButton_cancel.SetActive(false);

        Button tempButton = tempRoomUI.gameObject.GetComponent<Button>();
        tempButton.onClick.AddListener(() => OnClickRoomUI(tempRoomUI));
        //ClearChildsDataInRoomUser();

        ClearChildsDataInRoomUser();

        agoraController = AgoraController.GetAgoraControllerInstance();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickCreateTabButton()
    {
        CurrentState = State.Create;
        joinTab.SetActive(false);
        createTab.SetActive(true);
        createTabButton.animator.SetTrigger(Animator.StringToHash("Selected"));

        refreshButton.gameObject.SetActive(false);
        OnClickClosePasswordInput();
    }

    public void OnClickJoinTabButton()
    {
        CurrentState = State.Join;
        joinTab.SetActive(true);
        createTab.SetActive(false);

        refreshButton.gameObject.SetActive(true);
        OnClickClosePasswordInput();
        OnClickRefreshButton();
    }

    public void OnClickStartButton()
    {
        OnClickMyPageCloseButton();
        if (CurrentState == State.Create)
        {
            CreateRoom();
        }
        else
        {
            if (currentPickRoom != null)
            {

                if (currentPickRoom.isPassword)
                {
                    if (CurrntPasswordState == PasswordState.Close)
                    {
                        CurrntPasswordState = PasswordState.Open;
                        passwordInputBoxObject.SetActive(true);
                    }
                    else
                    {
                        Debug.Log(currentPickRoom.title.text + "??? ???");
                        CurrntPasswordState = PasswordState.Close;
                        Application.ExternalCall("socket.emit", "JOIN_ROOM", currentPickRoom.roomNo, joinRoomPassword.text);
                        OnClickClosePasswordInput();
                    }
                }
                else
                {
                    Debug.Log(currentPickRoom.title.text + "??? ???");
                    Application.ExternalCall("socket.emit", "JOIN_ROOM", currentPickRoom.roomNo, "");
                }
            }
        }
    }

    public void OnClickClosePasswordInput()
    {
        CurrntPasswordState = PasswordState.Close;
        passwordInputBoxObject.SetActive(false);
        joinRoomPassword.text = "";
    }

    public void CreateRoom()
    {
        Room room = new Room(createRoomName.text, createRoomPassword.text, 1);
        Debug.Log(JsonUtility.ToJson(room));
        Application.ExternalCall("socket.emit", "CREATE_ROOM", JsonUtility.ToJson(room));
    }

    public void OnClickRefreshButton()
    {
        DestoryChildsInRoomList();
        OnClickClosePasswordInput();
        Debug.Log("?????? ?????? ?????? ??????");
        Application.ExternalCall("socket.emit", "REFRESH_ROOM_LIST");
    }

    private void DestoryChildsInRoomList()
    {
        RoomUIWrapper[] roomUIWrappers = roomListParent.GetComponentsInChildren<RoomUIWrapper>();

        for (int i = 0, end = roomUIWrappers.Length; i < end; i++)
        {
            Destroy(roomUIWrappers[i].gameObject);
        }
    }

    public void ReceiveRoomList(string jsonstr)
    {
        Debug.Log("??? ????????? ?????? json: " + jsonstr);
        RoomsWrapper receiveRoomsWrapper = JsonUtility.FromJson<RoomsWrapper>(jsonstr);

        Debug.Log(receiveRoomsWrapper.rooms);
        if (receiveRoomsWrapper.rooms != null)
        {
            Debug.Log(receiveRoomsWrapper.rooms[0].waitingroom_nm);
            for (int i = 0, end = receiveRoomsWrapper.rooms.Length; i < end; i++)
            {
                GameObject tempGameObject = GameObject.Instantiate(Resources.Load("Lobby/RoomUIWrapper"), roomListParent.transform) as GameObject;
                Debug.Log(tempGameObject);
                RoomUIWrapper tempRoomUI = tempGameObject.GetComponent<RoomUIWrapper>();
                Button tempButton = tempGameObject.GetComponent<Button>();
                tempButton.onClick.AddListener(() => OnClickRoomUI(tempRoomUI));
                tempRoomUI.roomNo = receiveRoomsWrapper.rooms[i].waitingroom_no;
                tempRoomUI.title.text = receiveRoomsWrapper.rooms[i].waitingroom_nm;
                tempRoomUI.hostId.text = receiveRoomsWrapper.rooms[i].waitingroom_host_id;
                tempRoomUI.story.text = receiveRoomsWrapper.rooms[i].story_no.ToString();
                tempRoomUI.peopleCount.text = receiveRoomsWrapper.rooms[i].people_count.ToString();
                tempRoomUI.isPassword = receiveRoomsWrapper.rooms[i].is_password;

                roomObjectList.Add(tempGameObject);
            }
        }
    }

    private void MenuShow()
    {
        menuCanvasGroup.alpha = 1;
        menuCanvasGroup.interactable = true;
        menuCanvasGroup.blocksRaycasts = true;
    }
    private void MenuHide()
    {
        menuCanvasGroup.alpha = 0;
        menuCanvasGroup.interactable = false;
        menuCanvasGroup.blocksRaycasts = false;
    }

    //???????????? ????????? ??? ??? ?????? ????????? ??????
    public void ReceiveRoomInfo(string roomjsonstr)
    {
        Debug.Log("????????? ??? ?????? json: " + roomjsonstr);
        //?????? ????????? ?????? waitingroom_no, waitingroom_nm, story_no, people_count
        Room receiveRoom = JsonUtility.FromJson<Room>(roomjsonstr);

        if (receiveRoom.waitingroom_no != 0)
        {
            MenuHide();
            GameRoomInfo.roomNo = roomInfo.roomNo = receiveRoom.waitingroom_no;
            GameRoomInfo.roomTitle = roomInfo.roomInfoTitleText.text = receiveRoom.waitingroom_nm;
            //TODO ???????????? ????????? ??? ??????.
            GameRoomInfo.roomStory = roomInfo.roomInfoStroyText.text = "????????? ????????????";
            roomInfo.roomInfoPeopleCountText.text = receiveRoom.people_count + "/6 ???";
            Client.room = receiveRoom.waitingroom_no.ToString();
            roomInnerUIObject.SetActive(true);
        }
    }

    public void ReceiveRoomUserInfo(string usersjsonstr)
    {
        ClearChildsDataInRoomUser();

        Debug.Log("????????? ??? ?????? ?????? json: " + usersjsonstr);
        UsersWrapper receiveUsersWrapper = JsonUtility.FromJson<UsersWrapper>(usersjsonstr);

        Debug.Log(receiveUsersWrapper.users);
        if (receiveUsersWrapper.users != null)
        {
            Debug.Log(receiveUsersWrapper.users[0].user_name);
            for (int i = 0, end = receiveUsersWrapper.users.Length; i < end && i < 6; i++)
            {
                roomUsers[i].userId = receiveUsersWrapper.users[i].user_id;
                roomUsers[i].userNameTextMesh.text = receiveUsersWrapper.users[i].user_id;

                roomUsers[i].userNoByRoom = (receiveUsersWrapper.users[i].enter_no % 9999) + 1;

                if (roomUsers[i].userId == Client.name && isFIrst)
                {
                    isFIrst = false;
                    GameRoomInfo.userNoByRoom = roomUsers[i].userNoByRoom;
                    agoraController.JoinAgoraRoom(GameRoomInfo.roomNo + "Lobby", false, GameRoomInfo.userNoByRoom);
                }

                //?????? ???????????? ??????
                SetActiveRecursively(roomUsers[i].transform, true, true);

                if (roomUsers[i].userId == Client.name)
                {
                    roomUsers[i].slider.gameObject.SetActive(false);
                }
            }
        }
    }


    public void SetActiveRecursively(Transform trans, bool flag, bool first_flag)
    {
        if (!first_flag)
            trans.gameObject.SetActive(flag);
        foreach (Transform child in trans)
        {
            SetActiveRecursively(child, flag, false);
        }
    }

    //??? ?????? ?????? User ???????????? ?????????. ???????????? Active false??? ??????.
    private void ClearChildsDataInRoomUser()
    {
        roomUsers[0].userNameTextMesh.text = "";
        for (int i = 1, end = roomUsers.Length; i < end; i++)
        {
            SetActiveRecursively(roomUsers[i].transform, false, true);
        }
    }

    private Color ORIGIN_COLOR = new Color(0.67f, 0.67f, 0.67f);
    public void OnClickRoomUI(RoomUIWrapper roomData)
    {
        Debug.Log(roomData.title.text);
        if (currentPickRoom != null)
        {
            currentPickRoom.outline.color = ORIGIN_COLOR;
        }
        OnClickClosePasswordInput();
        currentPickRoom = roomData;
        currentPickRoom.outline.color = Color.white;
    }

    public void OnClickLeaveRoomButton()
    {
        if (Client.ready == true)
        {
            Client.ready = false;
            // ????????? ?????? ?????????
            Application.ExternalCall("socket.emit", "NOT_READY_CRIMESCENE", Client.room);
        }

        if (roomInfo.roomNo != 0)
        {
            Client.room = "empty";
            Application.ExternalCall("socket.emit", "LEAVE_ROOM", roomInfo.roomNo);
            agoraController.LeaveAgoraRoom();
        }
        MenuShow();
        GameRoomInfo.ClearGameRoomInfo();
        PlayCrimeSceneButton.SetActive(true);
        PlayCrimeSceneButton_cancel.SetActive(false);
        roomInnerUIObject.SetActive(false);
        OnClickRefreshButton();
        isFIrst = true;
        //TODO ??? ??? ????????? ????????? ??????
    }

    public void OnClickPlayCrimeScene()
    {
        Debug.Log("[system] ?????? ?????? ?????? ??????: " + Client.room);

        Debug.Log("[system] ????????? ????????????: " + GameInfo.GameRoomInfo.roomReadyPlayer);

        if (GameInfo.GameRoomInfo.roomReadyPlayer == 5)
        {
            Debug.Log("[system] ??????????????? ?????? ????????? : " + Client.room);
            Application.ExternalCall("socket.emit", "PLAY_CRIMESCENE", Client.room);

        }
        else
        {
            Client.ready = true;
            PlayCrimeSceneButton.SetActive(false);
            PlayCrimeSceneButton_cancel.SetActive(true);
            Application.ExternalCall("socket.emit", "READY_CRIMESCENE", Client.room);
        }
    }

    public void OnClickPlayCrimeScene_Cancel()
    {
        Client.ready = false;
        PlayCrimeSceneButton.SetActive(true);
        PlayCrimeSceneButton_cancel.SetActive(false);
        Application.ExternalCall("socket.emit", "NOT_READY_CRIMESCENE", Client.room);

    }

    public void onRefreshReadyPlayer(int readyPlayer)
    {
        Debug.Log("[system] ????????? ???????????? : " + readyPlayer);
        GameInfo.GameRoomInfo.roomReadyPlayer = readyPlayer;
        ReadyUserText.text = readyPlayer.ToString();

    }

    public void playCrimeScene()
    {
        Client.ready = false;
        GameInfo.GameRoomInfo.roomReadyPlayer = 0;
        Debug.Log("[system] ?????? ?????? ???????????? : " + Client.room);
        agoraController.LoadNewAgoraInstance();
        SceneManager.LoadScene("WaitScene");
    }

    [SerializeField]
    private GameObject myPageObject;
    [SerializeField]
    private GameObject updateImage;
    [SerializeField]
    private GameObject deleteImage;
    public void OnClickMyPageButton()
    {
        myPageObject.SetActive(true);
    }

    public void OnClickMyPageCloseButton()
    {
        myPageObject.SetActive(false);
        updateImage.SetActive(false);
        deleteImage.SetActive(false);
    }
}

