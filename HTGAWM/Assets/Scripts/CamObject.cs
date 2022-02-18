using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;

using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CamObject : MonoBehaviour
{
    [SerializeField]
    private GameObject go_CamsParent;

    private VideoSurface[] surfaces;
    public Dictionary<string, int> surfaceIndexDict = new Dictionary<string, int>();

    private void Awake()
    {
        SceneManager.sceneUnloaded += ClearSurfaces;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void ClearSurfaces(Scene scene)
    {
        surfaces = go_CamsParent.GetComponentsInChildren<VideoSurface>(true);
        Debug.Log(surfaces.Length);
        surfaceIndexDict.Clear();
        foreach (VideoSurface vid in surfaces)
        {
            vid.gameObject.SetActive(false);
        }
    }
    public void AddOtherUser(uint uid)
    {
        string targetString = uid.ToString();
        for (int i = 0; i < surfaces.Length; i++)
        {
            if (!surfaces[i].gameObject.activeSelf && !surfaceIndexDict.ContainsKey(targetString))
            {
                OtherCam tempCam = surfaces[i].gameObject.GetComponent<OtherCam>();
                tempCam.userNo = (int)uid;
                surfaces[i].videoFps = 30;
                surfaces[i].SetForUser(uid);
                surfaces[i].SetEnable(true);
                surfaces[i].gameObject.SetActive(true);
                surfaceIndexDict.Add(uid.ToString(), i);
                Debug.Log("Agora: 다른 유저의 입장이 성공 했습니다.");
                return;
            }
            else
            {
                Debug.Log("Agora: 이미 존재하는 유저입니다");
            }
        }
    }



    public void DeleteOtherUser(uint uid)
    {
        string targetString = uid.ToString();

        Debug.Log("Agora: " + targetString + "유저가 나가려고 시도합니다.");
        if (surfaceIndexDict.ContainsKey(targetString)) {
            int targetIndex = surfaceIndexDict[targetString];
            surfaces[targetIndex].SetEnable(false);
            surfaces[targetIndex].gameObject.SetActive(false);
            surfaceIndexDict.Remove(targetString);
            Debug.Log("Agora: 나간 유저의 삭제에 성공했습니다.");
            return;
        }
        Debug.Log("Agora: 나간 유저의 삭제가 실패 했습니다.");
    }
}
