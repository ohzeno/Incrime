using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameInfo
{
    public static class GameRoomInfo
    {
        public static int roomNo = 0;
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

            agoraVideoScene.Add("MeetingScene");
            //���� �ư��󿡼� ������ �ʴ� ���� ������ ���þ����� �̾���.
            agoraKeepScene.Add("VoteScene");
            agoraKeepScene.Add("VoteSecondScene");
            agoraKeepScene.Add("ResultPage");
            agoraKeepScene.Add("ResultPage");
        }

        public static void ClearGameRoomInfo()
        {
            roomNo = 0;
            string roomTitle = "";
            string roomStory = "";
        }
}
}
