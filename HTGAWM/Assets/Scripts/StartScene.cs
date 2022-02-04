
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;

namespace Project
{

public class StartScene : MonoBehaviour
{
    //useful for any script access this class without the need of object instance  or declare .
    public static StartScene instance;


    [Header("Input field  :")]
    // 이름 입력하기
    public InputField JoinName;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
        
    }

    public void  SetUpProfile(string _current)
	{   
        Debug.Log(_current);
        Debug.Log("여기는 StartScene ===> " + _current );
    }

    
}
}