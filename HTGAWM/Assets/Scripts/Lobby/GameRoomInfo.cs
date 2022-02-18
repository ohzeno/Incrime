using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameInfo
{
    public static class GameRoomInfo
    {
        public static int roomNo = 0;
        public static int userNoByRoom = 0;
        public static string roomTitle = "";
        public static string roomStory = "";

        public static int roomReadyPlayer = 0;


        public static HashSet<string> agoraVideoScene = new HashSet<string>();
        public static HashSet<string> agoraKeepScene = new HashSet<string>();

        static GameRoomInfo()
        {
            agoraVideoScene.Add("BreakRoom");
            agoraVideoScene.Add("DirectorMaOffice");
            agoraVideoScene.Add("Hallway");
            agoraVideoScene.Add("MeetingRoom");
            agoraVideoScene.Add("OfficeCubicles");
            agoraVideoScene.Add("ReceptionDesk");
            agoraVideoScene.Add("RestRoom");
            agoraVideoScene.Add("SecurityRoom");
            agoraVideoScene.Add("TeamLeaderLeeOffice");
            agoraVideoScene.Add("ResultPage");

            agoraVideoScene.Add("MeetingScene");
            //아고라 상태를 유지할 목록
            agoraKeepScene.Add("VoteScene");
            agoraKeepScene.Add("VoteSecondScene");
            agoraKeepScene.Add("VoteTextResult");
        }

        public static void ClearGameRoomInfo()
        {
            roomNo = 0;
            string roomTitle = "";
            string roomStory = "";
        }
}
}
