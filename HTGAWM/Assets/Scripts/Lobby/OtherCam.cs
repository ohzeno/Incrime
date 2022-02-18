using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OtherCam : MonoBehaviour
{
    public int userNo;

    [SerializeField]
    private GameObject SliderObject;

    private AgoraController agoraController;

    // Start is called before the first frame update
    void Start()
    {
        agoraController = AgoraController.GetAgoraControllerInstance();
    }


    public void OnMouseEnterCam(BaseEventData data)
    {
        SliderObject.SetActive(true);
    }

    public void OnMouseExitCam(BaseEventData data)
    {
        SliderObject.SetActive(false);
    }


    public void OnChangedSliderValue(float value)
    {
        if(!agoraController.isAppNull())
            agoraController.SetVolumeByUserId(userNo, value);
    }
}
