using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class typingeffect : MonoBehaviour
{
    public Text tx;
    private string m_text = "";
    private AudioSource musicPlayer;
    public AudioClip keyboard;
    static private readonly char[] Delimiter = new char[] { ':' };
    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // multi vote
    IEnumerator _multi_typing(string m_text, string[] data)
    {
        yield return new WaitForSeconds(1f);
        PlaySound(keyboard, musicPlayer);
        for (int i = 0; i <= m_text.Length; i++)
        {
            tx.text = m_text.Substring(0, i);

            yield return new WaitForSeconds(0.15f);
        }
        yield return new WaitForSeconds(2f);
        MoveVote(data);
    }

    // single vote
    IEnumerator _typing(string m_text)
    {
        yield return new WaitForSeconds(1f);
        PlaySound(keyboard, musicPlayer);
        for (int i = 0; i <= m_text.Length; i++)
        {
            tx.text = m_text.Substring(0, i);

            yield return new WaitForSeconds(0.15f);
        }
        yield return new WaitForSeconds(2f);
        MoveResultStory();
    }

    public void VoteTextResult(string data)
    {
        Debug.Log("수신"+data);
        m_text = data + "는 범인으로 지목되었습니다.";
        StartCoroutine(_typing(m_text));
    }

    public void MultiVoteTextResult(string data)
    {
        Debug.Log("멀티수신" + data);
        var pack = data.Split(Delimiter);
        for (int i = 0; i < pack.Length; i++)
        {
            m_text += pack[i] + " ";
        }
        m_text += "동표로 재투표 합니다.";
        StartCoroutine(_multi_typing(m_text, pack));
    }

    public void MoveResultStory()
    {
        SceneManager.LoadScene("ResultPage");
    }

    public void MoveVote(string[] data)
    {
        Debug.Log("MoveVote" + data[0] + data[1]);
        Application.ExternalCall("socket.emit", "SECOND_VOTE", data);
        SceneManager.LoadScene("VoteScene");
    }

    public void PlaySound(AudioClip clip, AudioSource audioPlayer)
    {
        audioPlayer.Stop();
        audioPlayer.clip = clip;
        audioPlayer.loop = false;
        audioPlayer.time = 0;
        audioPlayer.Play();
    }

    public void StopSound(AudioClip clip, AudioSource audioPlayer)
    {
        audioPlayer.Stop();
    }
}
