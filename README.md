<h1 align="center">인크라임</h1>

### 목차

---

1. [프로젝트 소개](#프로젝트 소개)
2. [사용스택](#사용스택)
3. [Project Period](#Project Period)
4. [Contributors](#Contributors)
5. [프로젝트세팅](#프로젝트 세팅)
5. [ERD](#ERD)
5. [주요 기능](#주요 기능)
5. [주요 기술](#주요 기술)
5. [개발기록](#개발기록)





## 📖 프로젝트 소개

`인크라임`은 웹RTC를 활용한 추리게임입니다.

commit log: 싸피 외부 깃헙/깃랩 주소

🏠 https://ssafycrimescene.herokuapp.com/

📷 프로젝트 영상 : 영상주소

노션: https://puzzled-carpenter-bec.notion.site/bf36726088f74c7c862dd2ab69b00141



- 기획배경
  - 코로나19로 인해 오프라인상 친목이 힘들어짐. 
  - 방탈출 게임이나 추리 게임을 좋아하는 사람들도 바깥에서 관련 게임을 즐길 수가 없어짐.
  - 온라인에서 화상으로 서로 상호작용하며 즐길 수 있는 게임을 만들고자 함.







### 사용스택

| 용도        | 스택                                                         | 버전                  |
| ----------- | ------------------------------------------------------------ | --------------------- |
| JS 편집     | <img src="https://img.shields.io/badge/Visual Studio Code-007ACC?style=plastic&logo=Visual Studio Code&logoColor=white"> | v1.64                 |
| C# 편집     | <img src="https://img.shields.io/badge/Visual Studio-5C2D91?style=plastic&logo=Visual Studio&logoColor=white"> | 2019, community       |
| 배포서버    | <img src="https://img.shields.io/badge/Heroku-430098?style=plastic&logo=Heroku&logoColor=white"> |                       |
| DB          | <img src="https://img.shields.io/badge/MySQL-4169E1?style=plastic&logo=MySQL&logoColor=white"> | 5.7.x / 5.7.35        |
| 서버 설계   | <img src="https://img.shields.io/badge/node.js-339933?style=plastic&logo=Node.js&logoColor=white"> | node-v16.13.1-x64.msi |
| 인게임 개발 | <img src="https://img.shields.io/badge/Unity-색상?style=plastic&logo=Unity&logoColor=white"> | 2020.3.25f1 (LTS)     |
| 웹빌드      | <img src="https://img.shields.io/badge/WebGL-990000?style=plastic&logo=WebGL&logoColor=white"> |                       |
| 화상통화    | <img src="https://img.shields.io/badge/WebRTC-333333?style=plastic&logo=WebRTC&logoColor=white"> |                       |
| 형상관리    | <img src="https://img.shields.io/badge/git-F05032?style=plastic&logo=git&logoColor=white"> |                       |
| 협업        | <img src="https://img.shields.io/badge/Jira Software-0052CC?style=plastic&logo=Jira Software&logoColor=white"><img src="https://img.shields.io/badge/Notion-000000?style=plastic&logo=Notion&logoColor=white"> |                       |





## **Project Period**

2022.01.10 - 2022.02.18 (6주)





## **Contributors**

| 팀원   | 역할                 | 비고          | 깃헙                           |
| ------ | -------------------- | ------------- | ------------------------------ |
| 김재욱 | 백엔드               |               | https://github.com/blackvill   |
| 류기탁 | 팀장(신), 백엔드     |               | https://github.com/alwaysryu   |
| 양재빈 | 팀장(구), 프론트엔드 | 일러스트 제공 | https://github.com/jaebin-yang |
| 오제노 | 프론트엔드           |               | https://github.com/ohzeno      |
| 우윤식 | 백엔드               |               | https://github.com/Y1sik       |
| 장예찬 | 프론트엔드           | 유니티마스터  | https://github.com/redniche    |





## 프로젝트 세팅

### 유니티 빌드 세팅 상세

---

  - Unity Build

    - UnityBuild_1

      <img src="exec/img/UnityBuild_1.PNG">

    - file -> Build Settings -> WebGL -> Player Settings

    - UnityBuild_2

      <img src="exec/img/UnityBuild_2.PNG">

    - Player -> WebGL -> Resolution and Presentation -> WebGL Template -> ProjectTemplate

    - UnityBuild_3

      <img src="exec/img/UnityBuild_3.PNG">

    - Server 폴더 안에 있는 public 선택 후 빌드

  - NodeJS

    ```bash
    $ npm install
    $ node server.js #로컬 호스트 진행
    ```





### 배포 시 특이사항 기재

----

  - Heroku
    - 30분 마다 트래픽이 존재하지 않을 경우 sleep
    - https://puzzled-carpenter-bec.notion.site/Heroku-a935d2419c674bfc8aaaee51ed9010da





### DB 접속 프로퍼티 파일 목록

---

exec/DB/

- DBProperty.PNG

  ![DB프로퍼티](exec/DB/DBProperty.PNG)

- [Property_URL](exec/DB/Property_URL.md)

- DB_DUMP.zip

- DB_ERD.mwb



## ERD

![ERD](README_Sources/unknown.png)





## 사이트 상세

- 회원
  - 회원가입

    ![](README_Sources/회원가입창.png)

  - 로그인

    ![](README_Sources/로그인화면.png)

    - 오류창
  
      ![](README_Sources/오류창.png)
  
  - 마이페이지
    - 계정 별 승, 패 횟수 기록
  
      ![](README_Sources/마이페이지.png)
  
    - 정보 수정
  
      ![](README_Sources/회원정보수정.png)

    - 회원 탈퇴
  
      ![](README_Sources/회원탈퇴.png)
  
- 대기실 기능

  - 방구분

    ![](README_Sources/로비화면.png)

  - 비공개 방 생성 가능

    ![](README_Sources/비공개방1.png)

    ![](README_Sources/비공개2.png)

  - 방 접속

    - 다른 사람 볼륨 조절 가능

      ![](README_Sources/로비방.png)

- 게임설명

  ![](README_Sources/게임설명.png)

- 역할설명

  ![](README_Sources/역할설명.png)

- 실시간 화상회의

  ![](README_Sources/화상회의.gif)

  - 다른 사람 볼륨 조절 가능

- 역할 팝업(스포일러 방지 위해 내용 가림)

  ![](README_Sources/역할팝업.png)

- 메모장

  ![](README_Sources/메모장.png)

- 실시간 상호작용

  - 증거 공유

    ![](README_Sources/증거공유.png)

- 맵 이동

  ![](README_Sources/맵이동.png)

- 오브젝트와 상호작용

  - 화장실 문 등

    ![](README_Sources/문놀이.gif)

  - 증거 수집

    ![](README_Sources/proof.gif)

    - 중복수집 불가 알림창

  - 이스터에그

    ![](README_Sources/fish22.gif)


- 추가 증거 영상

  ![](README_Sources/추가증거영상.png)

- 투표

  ![](README_Sources/투표.png)

- 투표결과

  ![](README_Sources/투표결과.gif)

- 사건의 전말(스포일러 방지 위해 생략)





## 주요 기술

- Agora.io
  - WebRTC
- WebSocket
- Unity
  - Single-ton
  - MultiPlay
- WebGL





## 개발기록


### 01.10
### 1. 사용 프로그램 설치 및 환경 설정
### 2. 프로젝트 기획
- 프로젝트 미팅
    - kotlin server
    - 음성 서버 채팅
        - 필요하다면 채팅을 버리겠다
    - 환경을 만들 때 힘들다..
        - 문화유산 asset(무료 사용 가능)

    - 프로젝트 사이즈를 많이 줄여라
        - 유니티 -> 예찬님 -> 유니티 팀을 데려가라
        - 유니티를 웹에 어떻게 띄우는지 -> 멀티가 되는 환경을 어떻게 구현할 것인지 생각해야 한다.
            - 멀티로 소통을 하는 방식이 아닌 증거를 찾는 시간은 개인, 서로 조사 상황을 토론 하는 시간에만 멀티로 구현 하면 되지 않을까
    - 서버를 어떻게 해야할지 -> 무료 서버를 찾아서 세팅해라
    - 음성 / 대화 여러 유저 활동 프로토타입을 만들어서 올려라
        - 유저가 가입 접속 들어가서 방에 6명이든 2명이든 들어가서 활동 상호활동이 가능한지 확인
    - 스토리 2개 작은 거(우리 거) + 큰거 (차용)
        - 시간이 부족할 경우를 대비해 스토리를 작은 축소판으로 구현할 생각을 해야한다.
    - 상호작용 눌렀을 때 쪽지 형태로 나온다. (양피지st 이렇게 줄수 있는 거 / 이런 식으로 크기를 줄일 수 있다.)
    - 인벤토리 -> 이력 관리 (급하진 않음)
    - 게임이 돌아갈 수 있는 환경 갖추기
    - UNITY/SERVER/스토리 팀이 필요 (그만큼 최대한 빠른 진행을 해야한다.)

## 01.11
### 1. git flow 공부
- 깃이 동작하는 과정을 알아보았다.
### 2. 프로젝트 화면 구성을 위한 Asset 조사 및 구매
- 프론트의 화면을 담당할 Unity에서 사용할 Asset을 조사
    - 프로젝트에 알맞은 asset을 구매
### 3. DB 설계
- DB를 구성하기 위해 속성을 정리
### 4. 유니티를 통한 WebRTC 구현 해보기
- WebRTC의 구현에는 실패

### 01.12
### 1. Agora.io SDK, WebGL Plugin(Open Source)를 사용해 Unity WebGL 영상 및 음성 스트리밍
- 패치 버전을 사용하면 안됨
- 여러명의 영상과 음성 동시 송출 가능
### 2. DB 설계
- DB ERD 작성
- DB 구성 뼈대 작성 
    - Unity Game 에서 내에서 사용되는 데이터의 정보와 DB에서 사용되는 정보 정확하게 구분
### 3. 게임 구성
- 게임에서 관리하는 데이터를 정확하게 구분하기
- 게임에서 데이터와 웹 서버에서 통신 방식 자료조사



## 1.13

### 1.  노션 페이지 생성
- 명세서 작성

  - DB ERD 업로드
  - 알고리즘 디자인 설계 작성 및 업로드
    - 회원 가입
    - 로그인
    - 로비 - 대기실 검색 기능
    - 로비 - 대기실 방 만들기
    - 로비 - 대기실 입장
    - 대기실 - 대기실에서 게임 시작
    - 게임 시작 후 역할 배정
    - 초반 브리핑
    - 1차 탐색
    - 중간 회의 및 중간 투표
    - 2차탐색
    - 최종 회의 및 최종 투표
    - 게임 종료
  - 화면 설계 작성, 이미지 업로드

- 규칙 작성

  - commit 규칙 업로드
  - JIRA특강 정리

### 2.  JIRA
-  컴포넌트 생성
  - FE
  - BE
  - 개발과 관련 없는 부분

- 노션 명세서 따라 에픽, 스토리 생성

## 1.14
### 1. Git & Jira
- Story 정리 및 할당

### 2. Game Story 확정

## 1.17
### 1. Multi WebSocket
- 팀원들이 유니티 프로젝트를 각자 빌드해서 한 서버에 모임
- WebGL Multiplay 실행 확인

### 2. Lobby Server
- Unity Lobby 구동 확인
- 배포 확인

## 1.18
### 1. Unity - Firebase 연동
- 로그인 / 회원가입 / 구글 연동 로그인 / 페이스북 연동 로그인
- 팀프로젝트 데이터베이스 생성

## 1.19
### 1. Unity In-Game 개발
- 이팀장살인사건 1화, 2화 방별 증거 및 인물 정보 추출 및 노션 정리
- 크라임씬 -> Unity 구현방식 설계 및 방향성 회의
- Unity 씬 구현 각자 조사

### 2. Unity Server
- Node.js 에서 socket.io 사용하는 방법 조사 및 MultiPlayer 에셋 코드 분석
- 유니티를 웹으로 빌드해서 서버에서 유니티 asset들을 조작하고 다른 클라이언트에 업데이트하는 방식 조사
- heroku를 사용하여 서버 빌드 테스트

## 1.20
### 1. Unity
- 인게임 내에서 사용할 씬 구성
  - 팀장실, 회의실, 탕비실, 이사실 완료
  - 사무실 진행중

### 2. Server
- node.js와 socket.io를 사용해 멀티 채팅서버 구현 및 코드분석
- 인게임에 맞춰 서버에 접속 인원 체크 테스트
  - 역할 분배 진행중

## 1.21
### 1. Unity
- 인게임 내에서 사용할 씬 구성
  - 보안실, 비서자리, 사무실, 화장실, 복도
  - blender를 이용한 유니티 asset 수정 진행중

### 2. Server
- node.js와 socket.io를 사용해 멀티 채팅서버 구현 및 코드분석
- 인게임에 맞춰 서버에 접속 인원 체크
  - 역할 분배 완료
  - heroku server 분배 시 mysql 연동 접속 진행중
- 화상채팅 시 방 이동 테스트

## 1.24
### 1. Unity
- 인게임 내에서 동작 및 상호작용 개발 시작
  - 유니티 내 UI 구현 시작 (게임 시작화면, 게임 내 화면 등)
### 2. Server
- 게임 시작 화면 구현 (Backend)
- 게임 내 타이머 구현
- Heroku와 MySQL 연동 완료 및 각종 빌드 테스트

## 1.25
### 1. Unity
- Main Map 기능 구현
- 분리된 화장실 씬 결합
- Unity 물체 외곽선 자료 조사
- 사물 Orbit carmera 자료 조사 및 구현
  - 진행중 (50%)
- 게임 대기 화면 구현 완료 
  - 대기 중 인원 모든 클라이언트에게 보여주기 
  - 마지막으로 들어온 사람만 게임실행 버튼이 보이기
  - 클라이언트 별로 전역변수로 사용자의 정보를 저장하기
### 2. Server
- DB 테이블 재구성
- DB에 들어갈 데이터 재정리
- Join시 DB에 저장 기능 구현
- Disconnetion시 DB에 저장된 회원 삭제기능 구현
- 게임 대기 화면 DB 연동


## 1.26
1. 증거가 될 GameObject 번호, 이름, 설명 명세
2. Map Scene 구현
3. 역할 별 스토리 정리 완료
4. 역할 배정 화면 Scene 구현
5. 복도 Scene 수정 
6. Map 설계도 작성


## 1.27
1. Map 기능 모든 씬에 적용
2. 안쓰는 오브젝트 제거
3. 플레이어 맵 벗어나는 버그 해결방법 연구
4. 컨설턴트님 피드백 받아서 ppt 개선
5. Project에 Agora.io SDK 적용
6. 투표기능 구현중

## 1.28
1. 투표기능 구현 완료
2. 현재 진행 상황 발표
3. Agora.io SDK 활용 테스트 진행 중
4. Unity에 증거품 증거 입력 진행 중

## 2.03
1. Agora Cam 오브젝트 구성
2. 투표서버 구현
3. 투표결과 기능 진행중
4. 저장된 증거 확인 기능
5. 중간에 증거로 보여줄 CCTV 영상 편집
6. 프리팹 공유 연구
7. 이미지 파일 공유 연구

## 2.04
1. 로그인 구현
2. 회원가입 구현
3. 회원탈퇴 구현
4. 투표 기능 구현 완료
5. 진행된 프로젝트 병합
6. 데이터 통신 연구

## 2.07
1. 회원 정보 조회 구현
2. 회원 정보 수정 구현
3. 결과화면 초안 구현
4. PlayerObject UI 수정 (메모,맵)
5. PlayerObject를 인스턴스 하나만 생성하도록 수정 (싱글톤)
6. 전체적인 오류 수정
7. 현재 개발단계까지 서버에서 실행 확인

## 2.08
1. 결과화면 개선
2. 팀장실, 보안실 천장, 조명 추가
3. 팀장실 어항 버그 수정
4. 로비 구현방법 연구
5. 영상 증거공유 구현
6. 투표기능 병합
7. 스토리 결과 출력화면 연구
8. 증거 공유기능 구현

## 2.09
1. 로비 UI 틀 구성
2. 천장, 조명 추가
3. 기능 액션 추가
4. 대기 씬 UI 수정
5. Agora.io Prefab 구성

## 2.10
1. Agora.io preafab 구성
2. 투표 동표일 때 제어 구현
3. 역할 씬 UI 리뉴얼
4. 로비 생성, 목록 조회 구현
5. 결과창에서 사운드, 타이핑 효과 추가
6. 이팀장, 스마트폰, 화장실 문 상호작용 수정

## 2.11
1. 유저 관련 기능 병합 (로그인, 마이페이지, 유저 정보 수정, 삭제 등)
2. 1차 중복투표 시 2차 투표 진행 구현중
3. 방 별 Agroa.io 연결 완료 (로비 구현 후 프로젝트에 맞게 수정 필요)
4. 게임 내 로비 구현 마무리 단계
5. 증거 prefab 업데이트 (네이밍 통일, 누락된 prefab 등록 등)
6. 로비 별 게임 연결 진행중

## 2.14
1. 로그인이 되어 있을 때, 로그인이 되지 않도록 구현
2. 증거물 이름 변경, 설명 추가
3. 증거 설명 연출, 효과음 적용
4. 역할 상세 정보 DB에 추가 및 프로젝트에 적용
5. 결과화면 배경음악 적용
6. 서버 리뉴얼
7. 인게임 조명 밝기 조절
8. 마우스 커서가 게임화면 밖으로 나가지 않도록 적용
9. 서버 배포
10. 플레이어가 증거들을 보고 추리를 잘 할 수 있도록 증거의 설명을 추가
11. 추가 증거 제시할 때 효과음 적용

## 2.15
1. UCC 제작
2. 로비 볼륨조절 구현
3. 효과음 씬 이동해도 안 사라지는 방법 연구
4. 서버 재구성 완료
5. 투표 버그 수정
6. 버튼 사운드 관리 구현

## 2.16
1. UCC 제작
2. 마이페이지 리뉴얼
3. 역할 출력
4. 유니티 각종 버그 수정
5. 게임 결과 정보 저장
6. 아고라 캠별 볼륨 조절 기능
7. 기타 인게임 퀄리티 상향
8. 버튼 효과음 적용 완료



## 2.17
1. UCC 마무리
2. PPT 제작
3. 발표준비
4. 포팅 매뉴얼 작성
5. 발표 및 PPT에 쓸 영상 촬영
6. 프로젝트 전체 README.md 정리
