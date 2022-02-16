/**
 * /*
 *
 * @format
 * @autor: Rio 3D Studios
 * @description: java script server that works as master server of the Basic Example of WebGL Multiplayer Kit
 */

var express = require("express"); //import express NodeJS framework module
var app = express(); // create an object of the express module
var http = require("http").Server(app); // create a http web server using the http library
var io = require("socket.io")(http); // import socketio communication module

app.use(
	"/public/TemplateData",
	express.static(__dirname + "/public/TemplateData")
);
app.use("/public/Build", express.static(__dirname + "/public/Build"));
app.use(express.static(__dirname + "/public"));

// 게임 로비별 변수 // 룸이름, 게임페이즈, 레디플레이어
var games = {};
// 인게임
var IngameTimer = {};

// 사용하는 변수들
var clients = []; // to storage clients
var clientLookup = {}; // clients search engine

var loginsUsers = {};
// var sockets = {}; //// to storage sockets

// mySQL 연결 잘됨
var mysql = require("mysql");
const { clearInterval } = require("timers");
//
var connection = mysql.createConnection({
	host: "exbodcemtop76rnz.cbetxkdyhwsb.us-east-1.rds.amazonaws.com",
	user: "kw60r04ib61nxpzk",
	password: "req2v8wchyhantdn",
	database: "nlgr288toijev7z6",
	multipleStatements: true,
});
connection.connect();

var truncateRoomsSQL = `SET FOREIGN_KEY_CHECKS = 0;
truncate table waitingroom;
truncate table waiting_user;
SET FOREIGN_KEY_CHECKS = 1;`;

connection.query(truncateRoomsSQL, function (error, rows, fields) {
	if (error) {
		console.log(error);
	} else {
		console.log("전체 룸 삭제 완료");
	}
});

//로비기능
var lobbyFunc = require("./lobby");

//open a connection with the specific client
io.on("connection", function (socket) {
	
	//print a log in node.js command prompt
	console.log("[system] 클라이언트가 연결되었습니다");

	//to store current client connection
	var currentUser;

	// 콜백  EmitJoin()
	socket.on("JOIN", function (_data) {
		console.log("[system] player가 JOIN을 시도합니다. ");
		// 변환
		var data = JSON.parse(_data);

		currentUser = {
			id: socket.id, // 계속 사용할socket.id
			name: data.name,
			password: data.password,
			socketID: socket.id, // fills out with the id of the socket that was open
			usernumber: clients.length + 1,
			joinedRoomId: 0,
		};

		console.log(currentUser);
		console.log("[system] player " + currentUser.name + ": 이 입장 했습니다.!");
		console.log("로그인 시도 비밀번호 : " + currentUser.password);

		var SQL =
			"select * from user" + " where user_id = '" + currentUser.name + "'";

		connection.query(SQL, function (error, results) {
			if (error || results.length == 0) {
				console.log(error);
				var msg = "아이디 혹은 비밀번호를 확인하세요";
				console.log(msg);
				socket.emit("CHECK_ID_PW", msg);
			} else if (results[0].user_pw != currentUser.password) {
				var msg = "아이디 혹은 비밀번호를 확인하세요";
				console.log("아이디 혹은 비밀번호를 확인하세요");
				socket.emit("CHECK_ID_PW", msg);
			} else if (loginsUsers[results[0].user_id]) {
				var msg = "이미 로그인 되어있는 아이디입니다.";
				console.log(msg);
				socket.emit("CHECK_ID_PW", msg);
			} else {
				// clients list에 추가
				clients.push(currentUser);
				// add client in search engine
				clientLookup[currentUser.id] = currentUser;

				loginsUsers[results[0].user_id] = true;

				console.log("[system] 지금 참가자 수 : " + clients.length);
				//
				socket.emit(
					"JOIN_SUCCESS",
					currentUser.id,
					currentUser.name,
					clients.length
				);
				// lobby.js 의 roomSocketEvent
				lobbyFunc.roomSocketEvent(socket, currentUser, connection);
			}
		});
	}); //END_SOCKET_ON

	// 회원가입
	socket.on("USERJOIN", function (_data) {
		console.log("[INFO] User가 회원가입을 시도합니다.");
		var data = JSON.parse(_data);

		console.log(
			"datainfo : " + data.name + " " + data.password + " " + data.mail
		);

		// fills out with the information emitted by the player in the unity
		var User = {
			name: data.name,
			password: data.password,
			mail: data.email,
		}; //new user  in clients list

		console.log(User);
		console.log(
			"userinfo : " + User.name + " " + User.password + " " + User.mail
		);

		var SQL = `insert into user (user_id, user_pw, user_email) values (?,?,?)`;

		console.log(SQL);

		if (User.name.length < 6 || User.name.length > 16) {
			var msg = "아이디를 확인해주세요.";
			console.log(msg);
			socket.emit("JOINERROR", msg);
		} else if (User.password.length < 8 || User.password.length > 16) {
			var msg = "비밀번호를 확인해주세요.";
			console.log(msg);
			socket.emit("JOINERROR", msg);
		} else {
			let params = [User.name, User.password, User.mail];
			connection.query(SQL, params, function (error, results) {
				if (error) {
					console.log(error);
					var msg = "이미 존재하는 아이디입니다.";
					console.log("이미 존재하는 아이디입니다.");
					socket.emit("JOINERROR", msg);
				} else {
					var msg = "회원가입을 성공하였습니다.";
					console.log("[INFO] player " + User.name + ": join sucsess");
					socket.emit("JOINSUCCESS", msg);
					// socket.emit('CHANGE_STARTSSCENE');
				}
			});
		}
	}); //END_SOCKET_ON

	// 회원정보 수정
	socket.on("USERUPDATE", function (_data) {
		console.log("[INFO] User가 정보 수정을 시도합니다.");
		var data = JSON.parse(_data);

		console.log(
			"datainfo : " + data.name + " " + data.password + " " + data.mail
		);

		// fills out with the information emitted by the player in the unity
		var User = {
			name: data.name,
			password: data.password,
			mail: data.email,
		}; //new user  in clients list

		console.log(User);
		console.log(
			"userinfo : " + User.name + " " + User.password + " " + User.mail
		);

		var SQL = `update user set user_pw = ?, user_email = ? where user_id = ?`;

		if (User.password.length < 8 || User.password.length > 16) {
			var msg = "비밀번호를 확인해주세요.";
			console.log(msg);
			socket.emit("UPDATEERROR", msg);
		} else {
			let params = [User.password, User.mail, User.name];
			connection.query(SQL, params, function (error, results) {
				if (error) {
					console.log(error);
					var msg = "정보 수정에 실패하였습니다.";
					console.log("정보 수정에 실패하였습니다.");
					socket.emit("UPDATEERROR", msg);
				} else {
					console.log("[INFO] player " + User.name + ": update sucsess");
					var msg = "회원 정보 수정에 성공하였습니다.";
					socket.emit("UPDATESUCCESS", msg);
					socket.emit("USERINFO", User.name, User.password, User.mail);
				}
			});
		}
	}); //END_SOCKET_ON

	// 회원 탈퇴
	socket.on("USERDELETE", function (_data) {
		console.log("[INFO] User가 회원탈퇴를 시도합니다.");

		var SQL = `delete from user" + " where user_id =  ?`;

		let params = [currentUser.name];
		connection.query(SQL, params, function (error, results) {
			if (error) {
				console.log(error);
				console.log("삭제를 실패하였습니다..");
			} else {

				if (loginsUsers[currentUser.name]) loginsUsers[currentUser.name] = false;
				console.log("[INFO] player " + currentUser.name + ": delete sucsess");
				socket.emit("DELETESUCCESS");
			}
		});
	}); //END_SOCKET_ON

	// 유저 마이페이지
	socket.on("USERINFOPAGE", function () {
		var SQL = "select * from user" + " where user_id =  ?";

		let params = [currentUser.name];
		connection.query(SQL, params, function (error, results) {
			if (error) {
				console.log(error);
				console.log("호출 실패");
			} else {
				console.log(
					"[INFO] player " + currentUser.name + ": userinfo get sucsess"
				);
				socket.emit(
					"USERINFO",
					results[0].user_id,
					results[0].user_pw,
					results[0].user_email
				);
			}
		});
	});

	// 유저가 끊겼을 때
	socket.on("disconnect", function () {
		if (currentUser) {
			currentUser.isDead = true;
			if (loginsUsers[currentUser.name]) loginsUsers[currentUser.name] = false;
			lobbyFunc.leaveRoom(socket, currentUser, connection);
			//  socket.broadcast.emit('USER_DISCONNECTED', currentUser.id);

			var SQL = `update user set is_login = ? where user_id = ?`;

			let params = [0, currentUser.name];
			connection.query(SQL, params, function (error, results) {
				if (error) {
					console.log(error);
					console.log("정보 수정에 실패하였습니다.");
				} else {
					console.log("[INFO] player " + currentUser.name + ": update sucsess");
				}
			});

			for (var i = 0; i < clients.length; i++) {
				if (
					clients[i].name == currentUser.name &&
					clients[i].id == currentUser.id
				) {
					// console.log("User "+clients[i].name+" has disconnected");
					console.log(
						"[system] " + clients[i].name + "이 연결이 끊어졌습니다."
					);
					clients.splice(i, 1);
				}
			}
		}
		console.log("[system] 현재 연걸된 클라이언트 수 : " + clients.length);
	}); //END_SOCKET_ON

	// 대기중인 인원을 표시하는 함수
	socket.on("totalplayer", function () {
		clients.forEach(function (i) {
			io.to(i.id).emit("SET_TOTALPLAYER", clients.length, i.usernumber);
		}); //end_forEach
	});

	// 역할 배정
	socket.on("SET_ROLE", function ( _roomNumber) {
		
		// io.sockets.adapter.rooms[room];
		// var socketsinroom = io.sockets.clients('room'+_roomNumber ); 

		console.log("[system] 역할 배정을 시작 합니다.");
		// phase 2 : 역할 배정
		games[_roomNumber].GamePhase = 2;
		games[_roomNumber].ReadyUser = 0;
		var role = [0, 0, 0, 0, 0, 0];
		for (let index = 1; index <= 6; index++) {
			while (true) {
				var temp = Math.floor(Math.random() * 6);
				if (role[temp] == 0) {
					// 역할이 없을 경우
					role[temp] = index;
					break;
				}
			}
		}
		// console.log(role);
		// 역할 배치 완료.

		var roleindex = 0;
		// 해당 룸에 연결되어 있는 모든 클라이언트 불러오기 ( SET 형 )
		var socketsinroom = io.sockets.adapter.rooms.get('room'+_roomNumber);
		// console.log(socketsinroom);
		socketsinroom.forEach( function( element ) {
			// console.log(element + " --> index : " + index);
			var rolenumber = role[roleindex];
			var myrole;
			var myrolealibi;
			var SQL =
				"SELECT s.story_nm, s.story_description, r.role_name, r.role_alibi " +
				" FROM story AS s INNER JOIN role AS r " +
				" ON s.story_no = r.story_no " +
				" WHERE s.story_no = 1" +
				" AND r.role_id = " +
				rolenumber;
			connection.query(SQL, function (error, results, fields) {
				if (error) {
					console.log(error);
				}
				// console.log(results);

				myrole = results[0].role_name;
				mystoryname = results[0].story_nm;
				mystorydesc = results[0].story_description;
				myrolealibi = results[0].role_alibi;

				console.log("[system] 역할 -" + myrole);
				io.to( element ).emit(
					"ON_SET_ROLE",
					myrole,
					mystoryname,
					mystorydesc,
					myrolealibi
				);


			});
			roleindex += 1;
		});

		// gamephse 2 : 역할 숙지 시간
		// 시간 : 10분
		// 타이머 기능이 처음이라 바로 생성해버리면 됨.
		IngameTimer[_roomNumber] = New_Timer( 10, _roomNumber );

	});

	// 자기소개 하기
	socket.on("GO_MEETING_SCENE", function ( _roomNumber ) {
		console.log("[system] 미팅실로 이동합니다.");
		// gamephse 3 : 자기 소개 시간
		games[_roomNumber].GamePhase = 3;
		games[_roomNumber].ReadyUser = 0;
		// 미팅씬으로 이동
		io.sockets.in( 'room' + _roomNumber ).emit("GO_MEETING");
		// 시간 제한 - 시간이 지나면 맵 화면으로 바꾸기
		// 15분
		
		clearInterval(IngameTimer[_roomNumber]);
		IngameTimer[_roomNumber] = New_Timer( 15, _roomNumber );
	});

	socket.on("EXIT_MEETING", function ( _roomNumber ) {
		if ( games[_roomNumber].GamePhase == 3) {
			// 자기소개가 끝난 경우
			// phase 4 : 탐색 씬 
			games[_roomNumber].GamePhase = 4;
			games[_roomNumber].ReadyUser = 0;
			console.log("[system] 자기소개를 끝내고 모든 플레이어가 나가고 싶어합니다. ");
			// 탐색화면으로 보내버리기
			io.sockets.in( 'room' + _roomNumber ).emit("GO_MAP");
			// 탐색 시간 15분
			clearInterval(IngameTimer[_roomNumber]);
			// 지금은 게임 테스트를 위해 1분
			IngameTimer[_roomNumber] = New_Timer( 1, _roomNumber );

		} else if ( games[_roomNumber].GamePhase == 5) {
			// 1차회의가 끝난 경우
			// phase 6 : 투표 씬
			games[_roomNumber].GamePhase = 6;
			games[_roomNumber].ReadyUser = 0;
			console.log("[system] 모든 플레이어가 1차회의를 마치고 나가고 싶어합니다. ");

			// 투표로 보내버리기
			io.sockets.in( 'room' + _roomNumber ).emit("GO_VOTE");

			// 투표 시간 3분 
			clearInterval(IngameTimer[_roomNumber]);
			IngameTimer[_roomNumber] = New_Timer( 3, _roomNumber );

		}

	});

	// 투표 결과
	socket.on("PLAY_VOTE", function ( data, _roomNumber, votephase ) {
		console.log("[INFO] player가 투표 했습니다.");
		games[_roomNumber].VotedPlayer += 1;
		if (data == "Ma") {
			games[_roomNumber].VoteResult[0] += 1;
		} else if (data == "Kim") {
			games[_roomNumber].VoteResult[1] += 1;
		} else if (data == "Chun") {
			games[_roomNumber].VoteResult[2] += 1;
		} else if (data == "Jang") {
			games[_roomNumber].VoteResult[3] += 1;
		} else if (data == "Choi") {
			games[_roomNumber].VoteResult[4] += 1;
		} else if (data == "Yun") {
			games[_roomNumber].VoteResult[5] += 1;
		}

		if ( games[_roomNumber].VotedPlayer == 6) {
			console.log("[INFO] player 모두가 투표 했습니다.");
			clearInterval(IngameTimer[_roomNumber]);
			var _data = games[_roomNumber].VoteResult;
			if ( votephase == 1 ){
				io.sockets.in( 'room' + _roomNumber ).emit("GO_VOTE_RESULT", _data );
			} else {
				io.sockets.in( 'room' + _roomNumber ).emit('GO_SECOND_VOTE', _data);
			}
		}

	});

	// 투표 결과
	socket.on("RESULT_VOTE", function (data, _roomNumber, num ) { 
		games[_roomNumber].Count += 1;
		if( games[_roomNumber].Count == 6){
			games[_roomNumber].Count = 0;
			console.log("텍스트 멀티 결과 창 " + data);
			if ( num == 0 ){
				// 투표 끝.
				setTimeout(function () {
					io.sockets.in( 'room' + _roomNumber ).emit('GO_SINGLE_RESULT_VOTE', data);
				}, 2000); 
			} else {
				// 재투표
				setTimeout(function () {
					io.sockets.in( 'room' + _roomNumber ).emit('GO_MULTI_RESULT_VOTE', data);
				}, 3000); 
			}
		}
	});

	// 2차 투표
	socket.on("MOVE_SECOND_VOTE", function(data , _roomNumber ){
		games[_roomNumber].Count += 1;
		console.log("[system] 두번째투표 대기자  " + games[_roomNumber].Count );
		if( games[_roomNumber].Count == 6){
			games[_roomNumber].GamePhase = 7; // 2차 투표
			games[_roomNumber].Count = 0;
			games[_roomNumber].VotedPlayer = 0;
			games[_roomNumber].VoteResult = [0,0,0,0,0,0];
			games[_roomNumber].VoteResultTxt = "";

			console.log("[system] 두번째 투표 진행합니다. " + data);
			setTimeout(function () {
				io.sockets.in( 'room' + _roomNumber ).emit('GO_MOVE_SECOND_VOTE', data);
			}, 3000);

			clearInterval(IngameTimer[_roomNumber]);
			// 두 번째 투표 3분
			IngameTimer[_roomNumber] = New_Timer( 3, _roomNumber );
		}

	}); 

	// 2차 투표 결과
	socket.on("RESULT_SECOND_VOTE", function (data, _roomNumber, num ) { 
		games[_roomNumber].Count += 1;
		if( games[_roomNumber].Count == 6){
			games[_roomNumber].Count = 0;
			console.log("[system] 2차 투표 결과 전송 " + data);
			if ( num == 0 ){
				// 투표 끝.
				setTimeout(function () {
					io.sockets.in( 'room' + _roomNumber ).emit('GO_SINGLE_RESULT_SECOND_VOTE', data);
				}, 2000); 
			} else {
				// 재투표
				var voteresultstr = "";
				data.forEach(function(i) {
					if(i != null){
						voteresultstr += i + ":";
					}
				});
				setTimeout(function () {
					io.sockets.in( 'room' + _roomNumber ).emit('GO_MULTI_RESULT_SECOND_VOTE', data);
				}, 3000); 

			}
		}
	});

	// 대기실 내에서 게임 시작 버튼 누르기
	socket.on("READY_CRIMESCENE", function ( _roomNumber ) {
		console.log("[system] 플레이어가 크라임씬을 준비했습니다.");
		games[_roomNumber].ReadyUser += 1;
		refreshreadyUser(_roomNumber);
	});
	// 준비 취소 함수
	socket.on("NOT_READY_CRIMESCENE", function ( _roomNumber ) {
		console.log("[system] 플레이어가 크라임씬을 준비를 취소 했습니다.");
		games[_roomNumber].ReadyUser -= 1;
		refreshreadyUser(_roomNumber);
	});
	// 준비된 인원 업데이트
	socket.on("REFRESH_READY_USER", function ( _roomNumber ) {
		console.log("[system] 준비 된 인원 업데이트  ( 방 번호 : "+  _roomNumber + " ) " );
		refreshreadyUser(_roomNumber);
	});
	// games에 저장된 룸 삭제하기
	socket.on("DELETE_GAME_ROOM_IN_SERVER", function ( _roomNumber ) {
		delete games[_roomNumber];
		console.log("[system] 대기실을 삭제했습니다.(games) 현재 진행 중인 games : " +  Object.keys(games).length );
	});
	// 대기실에서 크라임씬 시작하기
	socket.on("PLAY_CRIMESCENE", function ( _roomNumber ) {
		games[_roomNumber].GamePhase = 1;
		games[_roomNumber].ReadyUser = 0;
		io.sockets.in( games[_roomNumber].SocketRoomName ).emit("ON_PLAY_CRIMESCENE");
	});
	// 게임 종료
	socket.on("END_GAME", function ( _roomNumber ) {
		if ( games[_roomNumber] != null ){
			console.log("[system] 방을 삭제 합니다. ")
			// 방을 삭제합니다.
			delete games[_roomNumber];
			var removeRoomSQL = `delete from waitingroom where waitingroom_no = ?`;
			let params = [_roomNumber];
			connection.query(
				removeRoomSQL,
				params,
				function (error, rows, fields) {
					if (error) {
						console.log(error);
						console.log("삭제에 에러가 발생했습니다.");
						return -1;
					} else {
						return true;
					}
				}
			);
		}
		
		// console.log(_winint);
		var SQL = `INSERT INTO playresult (playresult_userid, playresult_storyno, playresult_win) VALUES (?,?,?);`;
		// TODO each story
		var params = [ _playerid, 1, _winint ];
		connection.query(SQL, params, function (error, rows) {
			if (error) {
				console.log(error);
				socket.emit("[system] 결과를 기록하는 것에 에러가 발생했습니다. ");
			} else {
				return true;
			}
		});

	});



}); //END_IO.ON

var proof = require("./share_server");
proof.shareProof(io);

http.listen(process.env.PORT || 3000, function () {
	console.log("listening on *:3000");
});
console.log("------- server is running -------");

// 유저 업데이트 함수
function refreshreadyUser(_roomNumber) {

	if ( games[_roomNumber] == null ){
		// 생성된 방이 없는 경우 games에 방을 생성해서 넣기.
		console.log("[system] 새로 생성한 방입니다. " );
		games[_roomNumber] = {
			SocketRoomName : "room" + _roomNumber,
			GamePhase : 0,
			ReadyUser : 0,
			VotedPlayer : 0,
			VoteResult : [0,0,0,0,0,0],
			Count : 0,
			
		}
	}
	console.log(" 준비인원 -->" + games[_roomNumber].ReadyUser );
	// 생성된 방이 있으면 준비된 인원 변수를 리턴한다.
	io.sockets.in( games[_roomNumber].SocketRoomName ).emit("REFRESH_READY_USER_SUCCESS", games[_roomNumber].ReadyUser, games[_roomNumber].GamePhase );
	
}

// 새로운 통합 시간 관련 함수
function New_Timer( minutes, _roomNumber ){
	// gamephase == 0 : 게임시작 전 로비
	// gamephase == 1 : 게임 방법 숙지 화면 - WaitScene
	// gamephase == 2 : 역할 대기화면 - RoleScene
	// gamephase == 3 : 자기소개시간
	// gamephase == 4 : 탐색
	// gamephase == 5 : 1차 회의
	// gamephase == 6 : 투표시간
	// gamephase == 7 : 2차 투표
	// gamephase == 8 : 게임 종료

	var time = 0;
	var minute = minutes - 1;
	var second = 60;
	// 게임 페이즈 받기
	var phase = games[_roomNumber].GamePhase;

	// 시간 보내주는 함수
	var TIME_FUNCTION = setInterval(function () {
		second--;
		// console.log(phase + " --> " + minute + ":" + second );
		if (phase == 2) {
			// 2 : 역할 시간 업데이트
			io.sockets.in( 'room' + _roomNumber ).emit("SET_ROLE_TIMER", time, minute, second);
		} else if (phase == 3) {
			// 3. 미팅 시간 업데이트
			io.sockets.in( 'room' + _roomNumber ).emit("SET_GAME_TIMER", time, minute, second);
		} else if (phase == 4) {
			// 4. 게임 시간 업데이트
			io.sockets.in( 'room' + _roomNumber ).emit("SET_GAME_TIMER", time, minute, second);
		} else if (phase == 5) {
			// 5. 1차 회의 시간 업데이트
			io.sockets.in( 'room' + _roomNumber ).emit("SET_GAME_TIMER", time, minute, second);
		} else if (phase == 6) {
			// 6. 투표 시간 업데이트
			io.sockets.in( 'room' + _roomNumber ).emit("SET_VOTE_TIMER", time, minute, second);
		} else if (phase == 7) {
			// 7. 2차 투표 시간 업데이트
			io.sockets.in( 'room' + _roomNumber ).emit('SET_SECOND_VOTE_TIMER', time, minute, second);
		}

		if ( second == 0 && minute == 0 ){
			console.log("[system] 시간이 끝났습니다. 타이머 종료. ");
			clearInterval(IngameTimer[_roomNumber]);
			// 시간이 종료 되었을 때 실행할 곳.
			if (phase == 2) {
				console.log("[system] 역할 확인시간이 끝났습니다. 자기소개로 갑니다. ");
				// 2 : 역할 시간이 끝났을 경우.
				games[_roomNumber].ReadyUser = 0;
				games[_roomNumber].GamePhase = 3; // 페이즈 3으로. 
				// 자기 소개 시간 10분 
				clearInterval(IngameTimer[_roomNumber]);
				IngameTimer[_roomNumber] = New_Timer( 15, _roomNumber );
				io.sockets.in( 'room' + _roomNumber ).emit("GO_MEETING");

			} else if (phase == 3) {
				// 3. 자기 소개 시간 종료의 경우 
				console.log("[system] 자기 소개 시간이 끝났습니다. 자기소개로 갑니다. ");
				games[_roomNumber].ReadyUser = 0;
				games[_roomNumber].GamePhase = 4; // 페이즈 4 (탐색)으로. 
				// 탐색 시간 15분
				clearInterval(IngameTimer[_roomNumber]);
				IngameTimer[_roomNumber] = New_Timer( 15, _roomNumber );
				io.sockets.in( 'room' + _roomNumber ).emit('GO_MAP');
			
			} else if (phase == 4) {
				// 4. 탐색 시간 종료의 경우
				console.log("[system ]탐색 시간이 끝났습니다. 자기소개로 갑니다. ");
				games[_roomNumber].ReadyUser = 0;
				games[_roomNumber].GamePhase = 5; // 페이즈 5 (1차회의) 으로. 
				// 1차 회의시간 20분
				// 1분 있다가 추가 증거 영상 재생하기.
				clearInterval(IngameTimer[_roomNumber]);
				IngameTimer[_roomNumber] = New_Timer( 20, _roomNumber );
				setTimeout(function(){
					console.log("[system] 새로운 증거를 공개합니다.");
					io.sockets.in( 'room' + _roomNumber ).emit('VIEW_CLUE_VIDEO');
				}, 60000);

				io.sockets.in( 'room' + _roomNumber ).emit('GO_MEETING2');

			} else if (phase == 5) {
				// 5. 1차 회의 시간 종료의 경우	
				// phase 6 : 투표 씬
				games[_roomNumber].ReadyUser = 0;
				games[_roomNumber].GamePhase = 6;
				console.log("[system] 1차회의 시간이 종료되었습니다. 투표를 위해 이동 합니다. ");
				
				// 투표 시간 3분 
				clearInterval(IngameTimer[_roomNumber]);
				IngameTimer[_roomNumber] = New_Timer( 3, _roomNumber );
				io.sockets.in( 'room' + _roomNumber ).emit("GO_VOTE");

			} else if (phase == 6) {
				// 6. 1차 투표 시간 이 종료된 경우.
				console.log("[system] 투표 시간이 끝났습니다. ");
				clearInterval(IngameTimer[_roomNumber]);
				//
				var _data = games[_roomNumber].VoteResult;
				io.sockets.in( 'room' + _roomNumber ).emit("GO_VOTE_RESULT", _data);
				
			} else if ( phase == 7 ) {
				// 7. 2차 투표 시간이 종료된 경우
				console.log("[system] 2차 투표 시간이 끝났습니다. ");
				clearInterval(IngameTimer[_roomNumber]);
				var _data = games[_roomNumber].VoteResult;
				io.sockets.in( 'room' + _roomNumber ).emit('GO_SECOND_VOTE', _data);

			}

		} else if (second == 0 ) {
			minute -= 1;
			second = 60;
		}

	}, 1000);

	return TIME_FUNCTION;


}
