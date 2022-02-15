using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameInfo;

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
        else
        {
            Destroy(this.gameObject);
        }
        SceneManager.sceneLoaded += CheckScene;
    }

    private void CheckScene(Scene scene, LoadSceneMode mode)
    {
        if (!(GameRoomInfo.buttonSoundScene.Contains(scene.name)))
        {
            SceneManager.sceneLoaded -= CheckScene;
            Destroy(this.gameObject);
        }
    }
}
