/** @format */

//room.js
window.addEventListener("load", function () {
	// 탐색 후 1차 회의 시간
	socket.on("CREATE_ROOM_SUCCESS", function (_roomNumber) {
		if (window.unityInstance != null) {
			console.log("생성 성공 수신");
			console.log("방생성정보" + _roomNumber);
			socket.emit("REFRESH_INFO_IN_ROOM");
			socket.emit("REFRESH_USERS_IN_ROOM");

			// 준비 유저 업데이트
			socket.emit("REFRESH_READY_USER", _roomNumber);

			//window.unityInstance.SendMessage("Player", "ReceiveSharedProof", _data);
		}
	}); //END_SOCKET.ON

	socket.on("REFRESH_ROOM_LIST_SUCCESS", function (_data) {
		if (window.unityInstance != null) {
			console.log("새로고침 정보 수신");
			console.log("방목록정보" + _data);
			window.unityInstance.SendMessage(
				"LobbyController",
				"ReceiveRoomList",
				_data
			);
		}
	}); //END_SOCKET.ON

	socket.on("JOIN_ROOM_SUCCESS", function (_roomNumber) {
		socket.emit("REFRESH_INFO_IN_ROOM");
		socket.emit("REFRESH_USERS_IN_ROOM");
		// server.js 의 변수에다가 저장 하기
		socket.emit("UPDATE_ROOMINFO_IN_GAMES", _roomNumber);

		// 준비 유저 업데이트
		socket.emit("REFRESH_READY_USER", _roomNumber);
	});

	socket.on("LEAVE_ROOM_SUCCESS", function () {
		console.log("방에서 나왔습니다.");
	});

	socket.on("REFRESH_INFO_IN_ROOM_SUCCESS", function (_jsondata) {
		console.log("입장한 방의 정보 수신");
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage(
				"LobbyController",
				"ReceiveRoomInfo",
				_jsondata
			);
		}
	});

	socket.on("REFRESH_USERS_IN_ROOM_SUCCESS", function (_jsondata) {
		console.log("방 내 유저 목록 수신");
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage(
				"LobbyController",
				"ReceiveRoomUserInfo",
				_jsondata
			);
		}
	});

	socket.on("SOMEONE_JOIN_ROOM", function () {
		console.log("client.js 새로고침 요청");
		socket.emit("REFRESH_INFO_IN_ROOM");
		socket.emit("REFRESH_USERS_IN_ROOM");
	});

	socket.on("SOMEONE_LEAVE_ROOM", function () {
		console.log("client.js 새로고침 요청");
		socket.emit("REFRESH_INFO_IN_ROOM");
		socket.emit("REFRESH_USERS_IN_ROOM");
	});

	socket.on("LOBBY_ERROR", function (_strdata) {
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage(
				"ErrorMessageSystem",
				"OnRecieveErrorMessage",
				_strdata
			);
		}
	});

	socket.on("REFRESH_READY_USER_SUCCESS", function (_data) {
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage(
				"LobbyController",
				"onRefreshReadyPlayer",
				_data
			);
		}
	});

	socket.on("DELETE_GAME_ROOM", function (_data) {
		socket.emit("DELETE_GAME_ROOM_IN_SERVER", _data);
	});
}); //END_window_addEventListener
