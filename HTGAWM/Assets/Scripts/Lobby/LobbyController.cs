using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using User;
using GameInfo;

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
    GameObject passwordInputBoxObject;

    [SerializeField]
    TMP_InputField joinRoomPassword;

    void Start()
    {
        roomUsers = userListParent.GetComponentsInChildren<RoomUser>();
        OnClickCreateTabButton();
        roomInnerUIObject.SetActive(false);
        
        Button tempButton = tempRoomUI.gameObject.GetComponent<Button>();
        tempButton.onClick.AddListener(() => OnClickRoomUI(tempRoomUI));
        //ClearChildsDataInRoomUser();
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
        if (CurrentState == State.Create)
        {
            CreateRoom();
        }
        else
        {
            if(currentPickRoom != null) { 
                
                if (currentPickRoom.isPassword)
                {
                    if (CurrntPasswordState == PasswordState.Close)
                    {
                        CurrntPasswordState = PasswordState.Open;
                        passwordInputBoxObject.SetActive(true);
                    }
                    else
                    {
                        Debug.Log(currentPickRoom.title.text + "입장");
                        CurrntPasswordState = PasswordState.Close;
                        Debug.Log(joinRoomPassword.text + "유니티 비밀번호 입력값");
                        Application.ExternalCall("socket.emit", "JOIN_ROOM", currentPickRoom.roomNo, joinRoomPassword.text);
                        OnClickClosePasswordInput();
                    }
                }
                else
                {
                    Debug.Log(currentPickRoom.title.text + "입장");
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
        OnClickClosePasswordInput();
        Debug.Log("방 리스트 새로고침 호출");
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
        DestoryChildsInRoomList();

        Debug.Log("수신된 방 리스트 json: " + jsonstr);
        RoomsWrapper receiveRoomsWrapper = JsonUtility.FromJson<RoomsWrapper>(jsonstr);

        Debug.Log(receiveRoomsWrapper.rooms);
        if(receiveRoomsWrapper.rooms != null)
        {
            Debug.Log(receiveRoomsWrapper.rooms[0].waitingroom_nm);
            for(int i=0, end=receiveRoomsWrapper.rooms.Length; i<end; i++)
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

    public void ReceiveRoomInfo(string roomjsonstr)
    {
        Debug.Log("수신된 방 정보 json: " + roomjsonstr);
        //수신되는 정보 waitingroom_no, waitingroom_nm, story_no, people_count
        Room receiveRoom = JsonUtility.FromJson<Room>(roomjsonstr);

        if (receiveRoom.waitingroom_no != 0)
        {
            roomInnerUIObject.SetActive(true);
            GameInfo.GameRoomInfo.roomNo = roomInfo.roomNo = receiveRoom.waitingroom_no;
            GameInfo.GameRoomInfo.roomTitle =  roomInfo.roomInfoTitleText.text = receiveRoom.waitingroom_nm;
            //TODO 상용화 및 개선시엔 고쳐야 할 부분.
            GameInfo.GameRoomInfo.roomStory = roomInfo.roomInfoStroyText.text = "이팀장 살인사건";
            roomInfo.roomInfoPeopleCountText.text = receiveRoom.people_count + "/6 인";
        }
    }

    public void ReceiveRoomUserInfo(string usersjsonstr)
    {
        ClearChildsDataInRoomUser();
        
        Debug.Log("수신된 유저 리스트 json: " + usersjsonstr);
        UsersWrapper receiveUsersWrapper = JsonUtility.FromJson<UsersWrapper>(usersjsonstr);

        Debug.Log(receiveUsersWrapper.users);
        if (receiveUsersWrapper.users != null)
        {
            Debug.Log(receiveUsersWrapper.users[0].user_name);
            for (int i = 0, end = receiveUsersWrapper.users.Length; i < end && i < 6; i++)
            {
                roomUsers[i].userId = receiveUsersWrapper.users[i].user_id;
                roomUsers[i].userNameTextMesh.text = receiveUsersWrapper.users[i].user_id;
                //해당 행 보이기
                SetActiveRecursively(roomUsers[i].transform, true, true);
            }
        }
    }


    public void SetActiveRecursively(Transform trans, bool flag, bool first_flag)
    {
        if(!first_flag)
            trans.gameObject.SetActive(flag);
        foreach (Transform child in trans)
        {
            SetActiveRecursively(child, flag, false);
        }
    }

    //내용을 제거하는 것이 아닌 재귀적으로 Active 상태를 해제함.
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
        if(currentPickRoom != null)
        {
            currentPickRoom.outline.color = ORIGIN_COLOR;
        }
        OnClickClosePasswordInput();
        currentPickRoom = roomData;
        currentPickRoom.outline.color = Color.white;
    }

    public void OnClickLeaveRoomButton()
    {
        if (roomInfo.roomNo != 0)
        {
            Application.ExternalCall("socket.emit", "LEAVE_ROOM", roomInfo.roomNo);

        }
        roomInnerUIObject.SetActive(false);
        OnClickRefreshButton();
        //TODO 추가적으로 나갈 때의 제어 넣어야 함.
    }
}

