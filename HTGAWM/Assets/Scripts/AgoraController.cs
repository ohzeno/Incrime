using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameInfo;

public class AgoraController : MonoBehaviour
{
    private static AgoraController agoraController = null; // 싱글톤

    public CamObject camObject;
    static TestHelloUnityVideo app;      // 비디오용

    private CanvasGroup camCanvasGroup;

    void Awake()
    {
        // 싱글톤
        if (agoraController == null)
        {
            agoraController = this;
            
            DontDestroyOnLoad(this.gameObject);
            if (ReferenceEquals(app, null) && !ReferenceEquals(camObject, null))
            {
                Debug.Log("캠 인스턴스 생성");
                app = new TestHelloUnityVideo(camObject); // create app
                app.loadEngine("b16baf20b1fc49e99bd375ad30d5e340");
            }
            // 미팅 씬이란 채널로 들어오기
            app.join(Client.room + "MeetingScene", true);
            camCanvasGroup = camObject.gameObject.GetComponent<CanvasGroup>();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static AgoraController GetAgoraControllerInstance()
    {
        return agoraController;
    }

    // Start is called before the first frame update
    void Start()
    { 
        
    }

    void CamShow()
    {
        camCanvasGroup.alpha = 1;
        camCanvasGroup.interactable = true;
        camCanvasGroup.blocksRaycasts = true;
    }
    void CamHide()
    {
        camCanvasGroup.alpha = 0;
        camCanvasGroup.interactable = false;
        camCanvasGroup.blocksRaycasts = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name + "아고라로 접속합니다.");
        if(app != null)
        {
            if (GameRoomInfo.agoraVideoScene.Contains(scene.name))
            {
                app.leave();
                JoinAgoraRoom(GameRoomInfo.roomNo + scene.name, true);
                CamShow();
            }
            else if (GameRoomInfo.agoraKeepScene.Contains(scene.name))
            {
                //빈블록: 그냥 아고라의 이전 상태를 유지하기 위함임
            }
            else
            {
                app.leave();
                CamHide();
            }
        }
        
    }

    public void JoinAgoraRoom(string joinRoomID, bool audioFlag)
    {
        if (app != null)
            app.join(joinRoomID, audioFlag);
    }

    //아고라 나가는 메서드
    public void LeaveAgoraRoom()
    {
        // 아고라 나가기 테스트
        if(app != null)
            app.leave(); // leave channel
    }

    
}
