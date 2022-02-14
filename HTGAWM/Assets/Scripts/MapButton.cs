using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapButton : MonoBehaviour
{
    
    bool map_view;
    public GameObject mapimage;
    public ProofController proofController;
    
    // 비디오용
    static TestHelloUnityVideo app;

    // Start is called before the first frame update
    void Start()
    {
        map_view = false;
        mapimage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveToMap()
    {   
        

        if (SceneManager.GetActiveScene().name != "MeetingScene" ){
            
            // 맵버튼을 눌렀을 경우 해야할 일
            // 1. 아고라 맵 나가기
            app.leave(); // leave channel
            app.unloadEngine(); // delete engine
            app = null; // delete app
            

            proofController.CloseProofUI();
            SceneManager.LoadScene("Map");
        } else {
            if ( map_view == false ){
                // 맵 키기
                map_view = true;
                mapimage.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            } else {
                // 맵 끄기
                map_view = false;        
                mapimage.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }

        }

    }
}
