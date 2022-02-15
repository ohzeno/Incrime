/** @format */

var socket = io() || {};
socket.isReady = false;

// chatroom client .js
window.addEventListener("load", function () {
	var execInUnity = function (method) {
		if (!socket.isReady) return;

		var args = Array.prototype.slice.call(arguments, 1);

		f(window.unityInstance != null);
		{
			//fit formats the message to send to the Unity client game, take a look in NetworkManager.cs in Unity
			window.unityInstance.SendMessage("NetWork_Start", method, args.join(":"));
		}
	}; //END_exe_In_Unity

	socket.on("CHECK_ID_PW", function (msg) {
		console.log("CHECK_ID_PW : 아이디나 비밀번호를 확인해야할 때 들어오는 곳");
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage(
				"ErrorMessageSystem",
				"OnRecieveErrorMessage",
				msg
			);
		}
	});

	socket.on("JOINERROR", function (msg) {
		console.log("JOINERROR : 회원가입 문제 발생시 들어오는 곳");
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage(
				"ErrorMessageSystem",
				"OnRecieveErrorMessage",
				msg
			);
		}
	});

	socket.on("ERROR_OCCUR", function (msg) {
		console.log("Error 발생 : " + msg);
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage(
				"ErrorMessageSystem",
				"OnRecieveErrorMessage",
				msg
			);
		}
	});

	socket.on("JOINSUCCESS", function (msg) {
		console.log("JOINSUCCESS : 회원가입 성공 시 들어오는 곳");
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage("NetWork_Join", "JoinSuccess", msg);
		}
	});

	socket.on("UPDATEERROR", function (msg) {
		console.log("UPDATEERROR : 유저 정보 수정 문제 발생시 들어오는 곳");
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage("NetWork_MyPage", "UpdateFailMSG", msg);
		}
	});

	socket.on("UPDATESUCCESS", function (msg) {
		console.log("UPDATESUCCESS : 유저 정보 수정 성공 시 들어오는 곳");
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage(
				"NetWork_MyPage",
				"UpdateSuccessMSG",
				msg
			);
		}
	});

	socket.on("DELETESUCCESS", function () {
		console.log("DELETESUCCESS : 유저 정보 삭제 성공 시 들어오는 곳");
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage("NetWork_MyPage", "ChangeStartScene");
		}
	});

	// 조인이 성공했을 때
	// 콜백  JOIN SUCESS()
	socket.on("JOIN_SUCCESS", function (id, name, totalplayer) {
		var currentUserAtr = id + ":" + name + ":" + totalplayer;
		if (window.unityInstance != null) {
			console.log(
				"id : " + id + " 이름 : " + name + " 현재 이원 : " + totalplayer
			);
			window.unityInstance.SendMessage(
				"NetWork_Start",
				"OnJoinGame",
				currentUserAtr
			);
		}
	}); //END_SOCKET.ON

	socket.on("USERINFO", function (name, password, mail) {
		console.log("USERINFO");

		var currentUserAtr = name + ":" + password + ":" + mail;
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage(
				"NetWork_MyPage",
				"CheckUserInfo",
				currentUserAtr
			);
		}
	});

	socket.on("CHANGE_STARTSSCENE", function () {
		console.log("CHANGE_STARTSSCENE");
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage("NetWork_Join", "ChangeStartScene");
		}
	});

	// 끊겼을 때 아직 안함
	// socket.on('USER_DISCONNECTED', function(id) {
	//      var currentUserAtr = id;
	// 	if(window.unityInstance!=null)
	// 	{
	// 	 window.unityInstance.SendMessage ('', 'OnUserDisconnected', currentUserAtr);
	// 	}
	// });//END_SOCKET.ON

	socket.on("SET_TOTALPLAYER", function (totalplayer, mynum) {
		var data = totalplayer + ":" + mynum;
		console.log("SET_TOTALPLAYER : " + data);
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage("NetWork_Wait", "onTotalplayer", data);
		}
	}); // end

	// 역할 배정
	socket.on("ON_SET_ROLE", function (role, storyname, storydesc) {
		var _data = role + ":" + storyname + ":" + storydesc;

		console.log(_data);
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage("NetWork_Wait", "OnSetRole", _data);
		}
	}); //END_SOCKET.ON

	// 자기소개 시간
	socket.on("GO_MEETING", function () {
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage("NetWork_Role", "onConference");
		}
	}); //END_SOCKET.ON

	// 타이머
	socket.on("SET_ROLE_TIMER", function (time, minute, second) {
		var timer = time + ":" + minute + ":" + second;
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage("NetWork_Role", "RoleTimer", timer);
		}
	});

	// 미팅 내 타이머
	socket.on("SET_MEETING_TIMER", function (time, minute, second) {
		// console.log("미팅내타이머");
		var timer = time + ":" + minute + ":" + second;
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage(
				"NetWork_Meeting",
				"MeetingTimer",
				timer
			);
		}
	});

	// 게임 내 타이머
	socket.on("SET_GAME_TIMER", function (time, minute, second) {
		var timer = time + ":" + minute + ":" + second;
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage("PlayerObject", "InGameTimer", timer);
		}
	});

	// 맵으로 이동
	socket.on("GO_MAP", function () {
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage("NetWork_Meeting", "onExitMeeting");
		}
	}); //END_SOCKET.ON

	// 탐색 후 1차 회의 시간
	socket.on("GO_MEETING2", function () {
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage("PlayerObject", "onMeeting");
		}
	}); //END_SOCKET.ON

	// 추가 증거 비디오 띄워주기
	socket.on("VIEW_CLUE_VIDEO", function (phase) {
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage("NetWork_Meeting", "OnPlayVideo", phase);
		}
	}); //END_SOCKET.ON

	// 게임 시작하기
	socket.on("ON_PLAY_CRIMESCENE", function () {
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage("LobbyController", "playCrimeScene");
		}
	}); //END_SOCKET.ON

	// 준비된 인원 업데이트
	socket.on("REFRESH_READY_USER_SUCCESS", function (_data, _phase) {
		var SceneName;
		if (_phase == 0) {
			// 로비 씬
			SceneName = "LobbyController";
		} else if (_phase == 1) {
			// 게임 방법 숙지화면 : WaitScene
			SceneName = "NetWork_Wait";
		} else if (_phase == 2) {
			// 역할 숙지화면 : RoleScene
			SceneName = "NetWork_Role";
		} else if (_phase == 3 || _phase == 5 ) {
			// 자기소개 화면 : Meeting Scene
			SceneName = "NetWork_Meeting";
		}

		if (window.unityInstance != null) {
			window.unityInstance.SendMessage(
				SceneName,
				"onRefreshReadyPlayer",
				_data
			);
		}
	});
}); //END_window_addEventListener
