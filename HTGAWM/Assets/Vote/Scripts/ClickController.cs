using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Project {
    public class ClickController : MonoBehaviour
    {
        [SerializeField]
        private GameObject KimCheck;
        [SerializeField]
        private GameObject MaCheck;
        [SerializeField]
        private GameObject ChunCheck;
        [SerializeField]
        private GameObject JangCheck;
        [SerializeField]
        private GameObject YunCheck;
        [SerializeField]
        private GameObject ChoiCheck;
        [SerializeField]
        private GameObject ResultVote;
        [SerializeField]
        private GameObject MaHand;
        [SerializeField]
        private GameObject KimHand;
        [SerializeField]
        private GameObject ChunHand;
        [SerializeField]
        private GameObject JangHand;
        [SerializeField]
        private GameObject ChoiHand;
        [SerializeField]
        private GameObject YunHand;
        [SerializeField]
        private Text VoteText;
        [SerializeField]
        private Button VoteBtn;

        private string name;

        public void nameSet(string name){
            this.name = name;
        }

        public string nameGet(){
            return name;
        }

        static private readonly char[] Delimiter = new char[] {':'};

        void Start(){
            KimCheck.SetActive(false);
            MaCheck.SetActive(false);
            ChunCheck.SetActive(false);
            JangCheck.SetActive(false);
            YunCheck.SetActive(false);
            ChoiCheck.SetActive(false);
            ResultVote.SetActive(false);

        }

        void HideCheck(){
            KimCheck.SetActive(false);
            MaCheck.SetActive(false);
            ChunCheck.SetActive(false);
            JangCheck.SetActive(false);
            YunCheck.SetActive(false);
            ChoiCheck.SetActive(false);
        }

        public void ChunClick(){
            name = "Chun";
            nameSet(name);
            HideCheck();
            ChunCheck.SetActive(true);
        }

        public void KimClick(){
            name = "Kim";
            nameSet(name);
            HideCheck();
            KimCheck.SetActive(true);
        }

        public void JangClick(){
            name = "Jang";
            nameSet(name);
            HideCheck();
            JangCheck.SetActive(true);
        }

        public void MaClick(){
            name = "Ma";
            nameSet(name);
            HideCheck();
            MaCheck.SetActive(true);
        }

        public void YunClick(){
            name = "Yun";
            nameSet(name);
            HideCheck();
            YunCheck.SetActive(true);
        }

        public void ChoiClick(){
            name = "Choi";
            nameSet(name);
            HideCheck();
            ChoiCheck.SetActive(true);
        }

        public void VoteClick(){
            // emitVote 서버에 보낼 클라이언트 투표 데이터
            string data = nameGet();
            Debug.Log("[System] Client : 캐릭터 투표 server.js로 보내기 " + data);
            VoteText.text = "다른 플레이어를 기다려 주세요";
            VoteBtn.interactable = false;
            Application.ExternalCall("socket.emit", "first_vote", data);
        }

        public void MoveResultStory()
        {
            SceneManager.LoadScene("ResultPage");
        }

        public void onVote(string data){
            ResultVote.SetActive(true);
            MaHand.SetActive(false);
            KimHand.SetActive(false);
            ChunHand.SetActive(false);
            JangHand.SetActive(false);
            ChoiHand.SetActive(false);
            YunHand.SetActive(false);

            var vote = data.Split (Delimiter);
            Debug.Log(vote[0] + " " + vote[1] + " " + vote[2] + " " + vote[3] + " " + vote[4] + " " + vote[5]);

            for(int i = 0; i < int.Parse(vote[0]); i++){
                if(i == 0){
                    MaHand.SetActive(true);
                }
                else{
                    Debug.Log("ma " + i);
                    GameObject ins = Instantiate(MaHand, MaHand.transform.parent.transform) as GameObject;
                }
            }
            for(int i = 0; i < int.Parse(vote[1]); i++){
                if(i == 0){
                    KimHand.SetActive(true);
                }else{
                    Debug.Log("kim " + i);
                    GameObject ins = Instantiate(KimHand, KimHand.transform.parent.transform) as GameObject;
                }
            }
            for(int i = 0; i < int.Parse(vote[2]); i++){
                if(i == 0){
                    ChunHand.SetActive(true);
                }else {
                    Debug.Log("Chun " + i);
                    GameObject ins = Instantiate(ChunHand, ChunHand.transform.parent.transform) as GameObject;
                }
            }
            for(int i = 0; i < int.Parse(vote[3]); i++){
                if(i == 0){
                    JangHand.SetActive(true);
                } else {
                    Debug.Log("Jang " + i);
                    GameObject ins = Instantiate(JangHand, JangHand.transform.parent.transform) as GameObject;
                }
            }
            for(int i = 0; i < int.Parse(vote[4]); i++){
                if(i == 0){
                    ChoiHand.SetActive(true);
                }else {
                    Debug.Log("Choi " + i);
                    GameObject ins = Instantiate(ChoiHand, ChoiHand.transform.parent.transform) as GameObject;
                }
            }
            for(int i = 0; i < int.Parse(vote[5]); i++){
                if(i == 0){
                    YunHand.SetActive(true);
                }else {
                    Debug.Log("Yun " + i);
                    GameObject ins = Instantiate(YunHand, YunHand.transform.parent.transform) as GameObject;
                }
            }

            // 15초 뒤에 스토리 결과 출력
            Invoke("MoveResultStory", 15); 
        }
    }
}

