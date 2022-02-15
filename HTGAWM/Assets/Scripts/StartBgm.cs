using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBgm : MonoBehaviour
{
  // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += CheckScene;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckScene(Scene scene, LoadSceneMode mode)
    {
        if (!(scene.name == "StartScene" || scene.name == "JoinScene")){
            Destroy(this.gameObject);
        }
    }
}
