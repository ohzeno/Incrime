using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSound : MonoBehaviour
{
    private static ButtonSound buttonSound = null;
    // public AudioClip buttonSoundSource;
    public AudioSource soundPlayer;
    // Start is called before the first frame update
    void Awake()
    {
        if (buttonSound == null)
        {
            buttonSound = this;
            DontDestroyOnLoad(this.gameObject);
        } 
        else 
        {
            Destroy(this.gameObject);
        }
    }
    
    public void PlayButtonSound()
    {
        soundPlayer.Play();
    }

    public static ButtonSound GetButtonSoundInstance()
    {
        return buttonSound;
    }
}
