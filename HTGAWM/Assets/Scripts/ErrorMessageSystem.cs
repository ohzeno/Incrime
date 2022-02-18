using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessageSystem : MonoBehaviour
{
    public static ErrorMessageSystem instance;

    [SerializeField]
    GameObject errorBoxObject;

    [SerializeField]
    Text errorMessageText;

    // Start is called before the first frame update
    void Start()
    {
        // 싱글 톤 코드
        if (instance == null)
        {
            // 
            // 만약 다른 씬이 있으면 오브젝트를 파괴하지 않음
            DontDestroyOnLoad(this.gameObject);
            instance = this;// define the class as a static variable
        }
        else
        {
            //it destroys the class if already other class exists
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnRecieveErrorMessage(string data)
    {
        errorMessageText.text = data;
        errorBoxObject.SetActive(true);
    }

    public void OnClickCloseErrorBox()
    {
        errorBoxObject.SetActive(false);
    }
}
