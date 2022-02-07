using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    private void Awake()
    {
        var obj = FindObjectsOfType<PlayerObject>(); 
        if (obj.Length == 1) { 
            DontDestroyOnLoad(gameObject); 
        } 
        else { 
            Destroy(gameObject); 
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
