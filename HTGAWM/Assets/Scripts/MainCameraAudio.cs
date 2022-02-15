using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraAudio : MonoBehaviour
{
    private static MainCameraAudio mainCamera = null;
    // Start is called before the first frame update
    void Start()
    {
        if (mainCamera == null){
            mainCamera = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
