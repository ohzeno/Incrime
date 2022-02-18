using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLoader : MonoBehaviour
{

    private ButtonSound buttonSound = null;
    // Start is called before the first frame update
    void Start()
    {
        buttonSound = ButtonSound.GetButtonSoundInstance();
    }

    public void OnClickButtonUI()
    {
        buttonSound.PlayButtonSound();
    }
}
