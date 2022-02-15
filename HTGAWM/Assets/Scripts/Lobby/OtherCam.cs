using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    public void OnMouseEnter()
    {
        SliderObject.SetActive(true);
    }

    public void OnMouseExit()
    {
        SliderObject.SetActive(false);
    }


    public void OnChangedSliderValue(float value)
    {
        agoraController.SetVolumeByUserId(userNo, value);
    }
}
