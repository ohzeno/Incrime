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
    static WebAgoraUnityVideo app;

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
            proofController.CloseProofUI();
            SceneManager.LoadScene("Map");
            PlayerController.currentCurrentLockMode = CursorLockMode.None;
        } else {
            if ( map_view == false ){
                // 맵 키기
                map_view = true;
                mapimage.gameObject.SetActive(true);
            } else {
                // 맵 끄기
                map_view = false;        
                mapimage.gameObject.SetActive(false);
            }
        }
    }
}
