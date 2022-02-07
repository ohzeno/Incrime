using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapButton : MonoBehaviour
{
    
    bool map_view;
    public GameObject mapimage;

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
            SceneManager.LoadScene("Map");
        } else {
            if ( map_view == false ){
                // 메모 키기
                map_view = true;
                mapimage.gameObject.SetActive(true);

            } else {
                // 메모 끄기
                map_view = false;        
                mapimage.gameObject.SetActive(false);
            }

        }

    }
}
