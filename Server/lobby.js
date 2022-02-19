/** @format */

var self = (module.exports = {
	//공실인지 확인
	checkEmpty: function (roomId, connection, socket) {
		console.log("방 공실 체크 및 삭제");
		var checkEmptySQL = `select count(user_id) as count from waiting_user where waitingroom_no = ?`;
		let params = [roomId];

		connection.query(checkEmptySQL, params, function (error, rows, fields) {
			if (error) {
				console.log("방 공실 체크 중 에러");
				console.log(error);
				return -1;
			} else {
				if (rows[0].count == 0) {
					socket.emit("DELETE_GAME_ROOM", roomId);
					var removeRoomSQL = `delete from waitingroom where waitingroom_no = ?`;
					let params = [roomId];
					connection.query(
						removeRoomSQL,
						params,
						function (error, rows, fields) {
							if (error) {
								console.log(error);
								console.log("공실 삭제 중 에러");
								return -1;
							} else {
								return true;
							}
						}
					);
				} else {
					return false;
				}
			}
		});
	},

	//방을 나가면 DB와 socket에서 정보가 사라지도록 하는 함수
	leaveRoom: function (socket, currentUser, connection) {
		console.log("방을 떠나는 함수 요청");
		if (currentUser.joinedRoomId == 0) return;
		var leaveRoomSQL = `delete from waiting_user where user_id = (?)`;
		var params = [currentUser.name];
		connection.query(leaveRoomSQL, params, function (error, rows, fields) {
			if (error) {
				console.log("방을 떠나는 함수 중 에러");
				socket.emit("LOBBY_ERROR", "방을 떠나는 중 에러 발생");
				console.log(error);
			} else {
				self.checkEmpty(currentUser.joinedRoomId, connection, socket);
				socket.to("room" + currentUser.joinedRoomId).emit("SOMEONE_LEAVE_ROOM");
				socket.leave("room" + currentUser.joinedRoomId); /////////////////////////////////
				currentUser.joinedRoomId = 0;
				socket.emit("LEAVE_ROOM_SUCCESS");
			}
		});
	},

	roomSocketEvent: function (socket, currentUser, connection) {
		//소켓에서 방 생성이 호출될 때
		//START_SOCKET_ON
		socket.on("CREATE_ROOM", function (_data) {
			const data = JSON.parse(_data);
			console.log("방 생성 호출 수신");
			var createRoomSQL = `
insert into waitingroom (waitingroom_nm, waitingroom_pw, waitingroom_host_id, waitingroom_status, story_no) values (?,?,?,?,?);
insert into waiting_user (waitingroom_no, user_id) values (last_insert_id(), ?);
`;
			var params = [
				data.waitingroom_nm,
				data.waitingroom_pw,
				currentUser.name,
				"0",
				data.story_no,
				currentUser.name,
			];

			connection.query(createRoomSQL, params, function (error, rows) {
				if (error) {
					socket.emit("LOBBY_ERROR", "이미 방을 생성하셨습니다.");
				} else {
					currentUser.joinedRoomId = rows[0].insertId;
					socket.join("room" + rows[0].insertId);

					socket.emit("CREATE_ROOM_SUCCESS", rows[0].insertId);
				}
			});
		}); //END_SOCKET_ON

		//소켓에서 방 새로고침이 호출될 때
		//START_SOCKET_ON
		socket.on("REFRESH_ROOM_LIST", function () {
			console.log("방 목록 새로고침 요청 수신");
			var selectRoomSQL = `select w.waitingroom_no, waitingroom_nm, waitingroom_host_id, waitingroom_status, story_no, count(*) as people_count, IF(LENGTH(waitingroom_pw)>0,'true','false') as is_password
      from waitingroom w, waiting_user u
      where w.waitingroom_no=u.waitingroom_no
      group by w.waitingroom_no;`;

			connection.query(selectRoomSQL, function (error, rows, fields) {
				if (error) {
					console.log("방 목록 새로고침 중 에러");
					console.log(error);
				} else {
					var roomsWrapper = { rooms: rows };
					socket.emit(
						"REFRESH_ROOM_LIST_SUCCESS",
						JSON.stringify(roomsWrapper)
					);
				}
			});
		}); //END_SOCKET_ON

		//연결된 소켓과 currentUser 정보를 이용해 JOIN_ROOM 이벤트가 호출되면
		//방에 연결되도록 소켓 이벤트를 등록하는 함수
		socket.on("JOIN_ROOM", function (_roomNumber, _password) {
			console.log("방 입장 요청 수신");
			var getRoomPasswordSQL = `select waitingroom_pw from waitingroom where waitingroom_no = ?`;

			connection.query(
				getRoomPasswordSQL,
				_roomNumber,
				function (error, rows, fields) {
					if (error) {
						console.log(error);
						socket.emit("LOBBY_ERROR", "방 입장 중 에러");
					} else {
						// 비밀번호가 같으면 입장
						if (_password == rows[0].waitingroom_pw) {
							var joinRoomSQL = `insert into waiting_user (waitingroom_no, user_id)
							values (?, ?)`;
							let params = [_roomNumber, currentUser.name];

							connection.query(
								joinRoomSQL,
								params,
								function (error, rows, fields) {
									if (error) {
										console.log("비밀번호 검사 중 에러");
										socket.emit("LOBBY_ERROR", "비밀번호 검사 중 에러");
									} else {
										//동시성 문제가 있을 수 있는 부분. 접속할 때 한 번 유저가 6명 이상인지 확인은 하지만 쿼리가 너무 잦아 개선이 필요한 부분.
										var userCountSql = `select count(waitingroom_no) as people_count from waiting_user where waitingroom_no = ?`;
										connection.query(
											userCountSql,
											_roomNumber,
											function (error, rows, fields) {
												if (error) {
													console.log("풀방 확인 중 에러");
													console.log(error);
													return;
												} else {
													if (rows[0].people_count > 6) {
														console.log("이미 풀방입니다.");
														var deleteUserRoomSql = `delete from waiting_user where user_id = ?`;
														connection.query(
															deleteUserRoomSql,
															currentUser.name,
															function (error, rows, fields) {
																if (error) {
																	console.log("풀방 나가는 중 에러");
																	console.log(error);
																}
																socket.emit(
																	"LOBBY_ERROR",
																	"이미 방이 가득 찼습니다"
																);
															}
														);
														return;
													}
													currentUser.joinedRoomId = _roomNumber;
													socket.join("room" + _roomNumber);
													//TODO 상용화 시 최적화가 필요한 부분. 단 한 번만 쿼리를 작동하고 서버에서 유지하는 식으로 가능.
													socket
														.to("room" + _roomNumber)
														.emit("SOMEONE_JOIN_ROOM");
													socket.emit("JOIN_ROOM_SUCCESS", _roomNumber);
												}
											}
										);
									}
								}
							);
						}
					}
				}
			);
		});

		//정상적으로 LEAVE_ROOM을 호출할 때 발생하는 동작
		socket.on("LEAVE_ROOM", function () {
			console.log("정상적인 방 나가기 요청 수신");

			self.leaveRoom(socket, currentUser, connection);
		});

		//방 정보 업데이트
		socket.on("REFRESH_INFO_IN_ROOM", function () {
			console.log("들어간 방의 정보 업데이트");

			self.getRoomInfo(currentUser.joinedRoomId, socket, connection);
		});

		//방 내 유저 목록 업데이트
		socket.on("REFRESH_USERS_IN_ROOM", function () {
			console.log("방 내의 유저 목록 새로고침");

			self.getUsersInRoom(currentUser.joinedRoomId, socket, connection);
		});
	},

	getRoomInfo: function (roomId, socket, connection) {
		console.log(roomId + "번 방의 정보 요청");

		var getRoomInfoSQL = `select w.waitingroom_no, waitingroom_nm, story_no, count(u.waitingroom_no) as people_count
		from waitingroom w, waiting_user u
		where w.waitingroom_no = u.waitingroom_no and w.waitingroom_no = ?
		group by w.waitingroom_no`;

		let params = [roomId];
		connection.query(getRoomInfoSQL, params, function (error, rows, fields) {
			if (error) {
				console.log("정보 요청 처리 실패");
				console.log(error);
				socket.emit("LOBBY_ERROR", "방 정보 수신 실패");
			} else {
				//Room 형태의 JSON객체 전달
				socket.emit("REFRESH_INFO_IN_ROOM_SUCCESS", JSON.stringify(rows[0]));
			}
		});
	},

	//호출시
	//방에 있는 유저들을 자바스크립트 객체 형식으로 반환해주는 함수
	getUsersInRoom: function (roomId, socket, connection) {
		console.log(roomId + "번 방의 유저목록 송신 함수 요청");
		var getUsersSQL = `select user_id, enter_no
		from waiting_user u
		where u.waitingroom_no = ?
		order by (enter_no)`;

		let params = [roomId];
		connection.query(getUsersSQL, params, function (error, rows, fields) {
			if (error) {
				console.log("유저 목록 송신 실패");
				console.log(error);
				socket.emit("LOBBY_ERROR", "방 정보 수신 실패");
			} else {
				var usersWrapper = { users: rows };
				//RoomsWrapper 형태의 JSON객체 전달
				socket.emit(
					"REFRESH_USERS_IN_ROOM_SUCCESS",
					JSON.stringify(usersWrapper)
				);
			}
		});
	},
});
