using UnityEngine;
using UnityEngine.UI;

using agora_gaming_rtc;
using agora_utilities;


// this is an example of using Agora Unity SDK
// It demonstrates:
// How to enable video
// How to join/leave channel
// 




public class TestHelloUnityVideo
{

    // instance of agora engine
    public static IRtcEngine mRtcEngine;
    private Text MessageText;
    // 
    public CamObject camObject;

    // load agora engine
    public void loadEngine(string appId)
    {
        // start sdk
        Debug.Log("해당 앱 키로 아고라 로드 합니다.");

        if (mRtcEngine != null)
        {
            Debug.Log("Engine exists. Please unload it first!");
            return;
        }

        // init engine
        mRtcEngine = IRtcEngine.GetEngine(appId);

        // enable log
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
    }

    public void join(string channel, bool enableVideoOrNot)
    {
        Debug.Log("calling join (channel = " + channel + ")");

        if (mRtcEngine == null)
            return;

        // set callbacks (optional)
        mRtcEngine.OnJoinChannelSuccess = onJoinChannelSuccess;
        mRtcEngine.OnUserJoined = onUserJoined;
        mRtcEngine.OnUserOffline = onUserOffline;
        mRtcEngine.OnLeaveChannel += OnLeaveChannelHandler;
        mRtcEngine.OnWarning = (int warn, string msg) =>
        {
            Debug.LogWarningFormat("Warning code:{0} msg:{1}", warn, IRtcEngine.GetErrorDescription(warn));
        };
        mRtcEngine.OnError = HandleError;

        // enable video

        if (enableVideoOrNot)
        {
            mRtcEngine.EnableVideo();
            // allow camera output callback
            mRtcEngine.EnableVideoObserver();
        }

        // mRtcEngine.EnableAudio();

        // Debug.Log("여기 이거 실행 하나?");
        // Debug.Log(mRtcEngine.EnableVideo()); // 0 이 나온것으로 봐선 도긴 되는데 내 화면에 안나옴.
        // mRtcEngine.EnableVideo();
        // mRtcEngine.EnableVideoObserver();

        // join channel
        mRtcEngine.JoinChannel(channel, null, 0);
    }
    
    void OnLeaveChannelHandler(RtcStats stats)
    {
        Debug.Log("OnLeaveChannelSuccess ---- TEST");
    }

    public string getSdkVersion()
    {
        string ver = IRtcEngine.GetSdkVersion();
        return ver;
    }

    public void leave()
    {
        Debug.Log("calling leave");

        if (mRtcEngine == null)
            return;

        // leave channel
        mRtcEngine.LeaveChannel();
        // deregister video frame observers in native-c code
        mRtcEngine.DisableVideoObserver();
    }

    // unload agora engine
    public void unloadEngine()
    {
        Debug.Log("calling unloadEngine");

        // delete
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();  // Place this call in ApplicationQuit
            mRtcEngine = null;
        }
    }

    public void EnableVideo(bool pauseVideo)
    {
        Debug.Log("EnableVideo");
        if (mRtcEngine != null)
        {
            Debug.Log(mRtcEngine);
            if (!pauseVideo)
            {
                mRtcEngine.EnableVideo();
            }
            else
            {
                mRtcEngine.DisableVideo();
            }
        }
    }

    // accessing GameObject in Scnene1
    // set video transform delegate for statically created GameObject
    public void onSceneHelloVideoLoaded()
    {
        // Attach the SDK Script VideoSurface for video rendering
        GameObject quad = GameObject.Find("Quad");
        if (ReferenceEquals(quad, null))
        {
            Debug.Log("failed to find Quad");
            return;
        }
        else
        {
            quad.AddComponent<VideoSurface>();
        }

        GameObject cube = GameObject.Find("Cube");
        if (ReferenceEquals(cube, null))
        {
            Debug.Log("failed to find Cube");
            return;
        }
        else
        {
            cube.AddComponent<VideoSurface>();
        }

        GameObject text = GameObject.Find("MessageText");
        if (!ReferenceEquals(text, null))
        {
            MessageText = text.GetComponent<Text>();
        }
    }

    // implement engine callbacks
    private void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
        GameObject textVersionGameObject = GameObject.Find("VersionText");
        textVersionGameObject.GetComponent<Text>().text = "SDK Version : " + getSdkVersion();
    }

    // When a remote user joined, this delegate will be called. Typically
    // create a GameObject to render video on it
    private void onUserJoined(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);
        // this is called in main thread

        // find a game object to render video stream from 'uid'
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }

        // create a GameObject and assign to this new user
        VideoSurface videoSurface = makeImageSurface(uid.ToString());
        if (!ReferenceEquals(videoSurface, null))
        {
            camObject.AddOtherUser(uid);

            // configure videoSurface
            videoSurface.SetForUser(uid);
            videoSurface.SetEnable(true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
        }
    }

    public VideoSurface makePlaneSurface(string goName)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);

        if (go == null)
        {
            return null;
        }
        go.name = goName;
        // set up transform
        go.transform.Rotate(-90.0f, 0.0f, 0.0f);
        float yPos = Random.Range(3.0f, 5.0f);
        float xPos = Random.Range(-2.0f, 2.0f);
        go.transform.position = new Vector3(xPos, yPos, 0f);
        go.transform.localScale = new Vector3(0.25f, 0.5f, .5f);

        // configure videoSurface
        VideoSurface videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
    }

    private const float Offset = 100;
    //
    static int temp = 1;
    public VideoSurface makeImageSurface(string goName)
    {
        GameObject go = new GameObject();

        if (go == null)
        {
            return null;
        }

        go.name = goName;

        // to be renderered onto
        go.AddComponent<RawImage>();

        // make the object draggable
        go.AddComponent<UIElementDragger>();
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            go.transform.parent = canvas.transform;
        }
         // set up transform
        go.transform.Rotate(0f, 0.0f, 180.0f);
        // 이부분 수정하기
        // float xPos = Random.Range(Offset - Screen.width / 2f, Screen.width / 2f - Offset);
        // float yPos = Random.Range(Offset, Screen.height / 2f - Offset);
        float xPos = -900;
        float yPos = 220;
        go.transform.localPosition = new Vector3(xPos + ( temp * 270 ), yPos, 0f);
        temp += 1;
        if ( temp == 6) temp =1;
        Debug.Log("지금 연결된 수 : " + temp);
        // 화면 사이즈 조절
        // go.transform.localScale = new Vector3(  (2*1.6666f) * (float) 0.8 ,  (2f) * (float) 0.8 , 1f);
        go.transform.localScale = new Vector3(  (1.5f*1.6666f)  ,  (1.5f)  , 1f);

        // configure videoSurface
        VideoSurface videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
    }
    // When remote user is offline, this delegate will be called. Typically
    // delete the GameObject for this user
    private void onUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // remove video stream
        Debug.Log("onUserOffline: uid = " + uid + " reason = " + reason);
        // this is called in main thread
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            Object.Destroy(go);
        }
    }

    #region Error Handling
    private int LastError { get; set; }
    private void HandleError(int error, string msg)
    {
        if (error == LastError)
        {
            return;
        }

        msg = string.Format("Error code:{0} msg:{1}", error, IRtcEngine.GetErrorDescription(error));

        switch (error)
        {
            case 101:
                msg += "\nPlease make sure your AppId is valid and it does not require a certificate for this demo.";
                break;
        }

        Debug.LogError(msg);
        if (MessageText != null)
        {
            if (MessageText.text.Length > 0)
            {
                msg = "\n" + msg;
            }
            MessageText.text += msg;
        }

        LastError = error;
    }

    #endregion
}
