# 오늘 진행 내용


## 01.10
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
