using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Proof : MonoBehaviour
{
    public string no;

    public string proofName;

    [TextArea]
    public string proofDescription;

    public Texture2D proofTexture;

    private string objectName;
    private string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        objectName = name;
        sceneName = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public string GetSceneName()
    {
        return sceneName;
    }

    public string GetObjectName()
    {
        return objectName;
    }
}

