using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class handcuffs : MonoBehaviour
{
    public GameObject MaHand;
    void Start()
    {
        for(int i = 1; i < 6; i++){
                Debug.Log(i);
                GameObject ins = Instantiate(MaHand, transform) as GameObject;
            
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
