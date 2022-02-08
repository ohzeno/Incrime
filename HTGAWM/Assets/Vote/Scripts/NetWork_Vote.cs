using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

namespace Project
{
    public class NetWork_Vote : MonoBehaviour
    {
        public static NetWork_Vote instance;
        static private readonly char[] Delimiter = new char[] {':'};
        // 들어오는 데이터를 : 구문으로 자르는 역할
        // minute : second

        public Text timer;
        // Start is called before the first frame update
        void Start()
        {
            // 싱글 톤 코드
        if (instance == null) {
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

        // 투표 씬 이동

        public void emitVote(){
            Debug.Log("[System] Client : 첫 투표 하러 가자");
            Application.ExternalCall("socket.emit", "setvote");
        }

        public void onVote(){
            Debug.Log("[System] Client : 첫 투표 하러 갑니다 ");
            SceneManager.LoadScene("VoteScene");
        }

        // 투표 시간
        public void VoteTimer(string data){
            var pack = data.Split(Delimiter);
            // Debug.Log(pack[1] + " : " + pack[2]);

            Client.minute = pack[1]; 
            Client.second = pack[2];

            var timetxt = "";
            timetxt = pack[1] + ":" + pack[2];
            timer.text = timetxt;
        }
    }

}
