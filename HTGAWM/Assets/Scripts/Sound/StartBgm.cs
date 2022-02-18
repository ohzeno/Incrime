using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBgm : MonoBehaviour
{
    public static GameObject startBgm = null;
    // Start is called before the first frame update
    void Start()
    {
        if (startBgm == null)
        {
            startBgm = this.gameObject;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
        SceneManager.sceneLoaded += CheckScene;
    }
    
    private void CheckScene(Scene scene, LoadSceneMode mode)
    {
        if (!(scene.name == "StartScene" || scene.name == "JoinScene")){
            SceneManager.sceneLoaded -= CheckScene;
            Destroy(this.gameObject);
        }
    }
}
