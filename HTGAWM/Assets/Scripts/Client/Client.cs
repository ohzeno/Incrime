using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Client
{
  // 유저 별로 저장
  public static string id;
  public static string name;
  public static string role;
  public static string alibi;
  
  // 스토리 저장
  public static string storyname;
  public static string storydesc;

  // 시간
  public static string minute;
  public static string second;

  // 메모
  public static string memo = "";

  // 대기실 관련
  public static string room = "temproom";
  public static bool ready = false;

  // 기타 변수
  public static int meeting_num = 0;
  public static int win = 0;
  
  public static bool isLocalPlayer;

}
