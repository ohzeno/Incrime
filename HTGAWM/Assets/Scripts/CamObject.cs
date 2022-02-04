using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;

public class CamObject : MonoBehaviour
{
    [SerializeField]
    private GameObject go_CamsParent;

    private VideoSurface[] surfaces;

    // Start is called before the first frame update
    void Start()
    {
        surfaces = go_CamsParent.GetComponentsInChildren<VideoSurface>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddOtherUser(uint uid)
    {
        for (int i = 0; i < surfaces.Length; i++)
        {
            if (surfaces[i].videoFps == 0)
            {
                
                surfaces[i].transform.Rotate(0f, 0.0f, 180.0f);
                // surfaces[i].videoFps = 30;
                surfaces[i].SetForUser(uid);
                surfaces[i].SetEnable(true);
                return;
            }
        }
    }
}
