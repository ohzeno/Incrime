using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Proof : MonoBehaviour
{
    public string no;

    public string proofName;

    [TextArea]
    public string proofDescription;

    public Texture2D proofTexture;

    private string objectName;
    private string sceneName;

    public string GetSceneName()
    {
        return sceneName;
    }

    public string GetObjectName()
    {
        return objectName;
    }

    // Awake
    void Awake()
    {
        objectName = name;
        sceneName = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {

    }

    [Serializable]
    public class ProofJson
    {
        public string no;

        public string proofName;

        public string proofDescription;

        public string objectName;
        public string sceneName;

        public ProofJson()
        {

        }

        public ProofJson(Proof _proof)
        {
            no = _proof.no;
            proofName = _proof.proofName;
            proofDescription = _proof.proofDescription;
            sceneName = _proof.GetSceneName();
            objectName = _proof.GetObjectName();
        }
    }
}

