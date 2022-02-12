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

// 사용하는 변수들
var clients = []; // to storage clients
var clientLookup = {}; // clients search engine
var sockets = {}; //// to storage sockets

// 게임단계 변수
var gamephase = 0;
// 나중에 복합적으로 관리하게 되면 phase를 리스트로 변경해야할듯.
// gamephase == 2 : 역할 대기화면
// gamephase == 3 : 자기소개시간
// gamephase == 4 : 탐색
// gamephase == 5 : 1차 회의
// gamephase == 6 : 투표시간
// gamephase == 7 : 게임 종료

// 시간관련 변수
var time;
var minute;
var second;
// 타임 업데이트 함수
let timerId;
// 미팅을 나가고 싶어하는 사람
var exitMeeting_people = 0;
// 회의 가고싶어하는 사람
var conference_number = 0;

// 처음 투표 변수
var first_vote_number = 0; // 인원 체크
var votes = [0, 0, 0, 0, 0, 0]; // 투표 결과

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
			} else {
				// clients list에 추가
				clients.push(currentUser);
				// add client in search engine
				clientLookup[currentUser.id] = currentUser;

				console.log("[system] 지금 참가자 수 : " + clients.length);
				//
				socket.emit(
					"JOIN_SUCCESS",
					currentUser.id,
					currentUser.name,
					clients.length
				);
				// Client.js 의 JOIN_SUCCESS 로 가셈
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
			lobbyFunc.leaveRoom(socket, currentUser, connection);
			//  socket.broadcast.emit('USER_DISCONNECTED', currentUser.id);

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
	socket.on("setrole", function () {
		console.log("[system] 역할 배정 합니다.");
		if (clients.length == 6) {
			// phase : 역할 배정
			gamephase = 2;
			console.log("[system] 플레이어가 모두 모였습니다. 역할을 배정합니다. ");
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
			clients.forEach(function (i) {
				var rolenumber = role[roleindex];
				var myrole;
				var SQL =
					"SELECT s.story_nm, s.story_description, r.role_name " +
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

					console.log("[system] 역할 -" + myrole);
					io.to(i.id).emit(
						"SET_ROLE",
						i.id,
						i.name,
						myrole,
						mystoryname,
						mystorydesc
					);
				});

				roleindex += 1;
			}); //end_forEach

			// 10 분 : 600000
			// 시간 초과 시 // 10분
			setTimeout(function () {
				if (gamephase == 2) {
					clearInterval(timerId);
					console.log(
						"[system] 역할 확인 시간이 끝났습니다. 회의실로 갑니다. "
					);
					// 자기소개 : 3
					gamephase = 3;
					clients.forEach(function (i) {
						clearInterval(timerId);
						// 자기소개시간 : 10분
						Timeset(10, gamephase);
						io.to(i.id).emit("GO_MEETING");
					}); //end_forEach
				}
			}, 600000);

			// 시간 보내주기
			Timeset(10, gamephase);
		} else {
			console.log("[system] We can't play crime scene");
		}
	});

	// 화상회의 씬
	socket.on("go_firstconference", function () {
		console.log("[system] 미팅실로 이동합니다.");
		conference_number += 1;
		// 모든 플레이어가 준비 됐을 때.
		if (conference_number == clients.length) {
			conference_number = 0;

			gamephase = 3;
			console.log("[system] 모든 플레이어가 준비 상태 입니다. ");

			conference = true;
			// 회의 실로 보내버리기
			clients.forEach(function (i) {
				io.to(i.id).emit("GO_MEETING");
			}); //end_forEach

			// 시간 제한 - 시간이 지나면 맵 화면으로 바꾸기
			// 600000 - 10분
			setTimeout(function () {
				if (gamephase == 3) {
					clearInterval(timerId);
					console.log("[system] 미팅 시간이 끝났습니다. ");
					// 이동시키기
					gamephase = 4;
					clients.forEach(function (i) {
						// 각자 게임 화면으로 보내버리면서 시간 처리
						clearInterval(timerId);
						// 탐색시간 10분
<<<<<<< Updated upstream
						Timeset(10, gamephase);
						io.to(i.id).emit("GO_MAP");
					}); //end_forEach
				}
			}, 600000);
			// 시간 보내주기
			Timeset(10, gamephase);
=======
						Timeset(1,gamephase);
						io.to(i.id).emit('GO_MAP');
					}); //end_forEach
				}
			}, 60000); 
			// 시간 보내주기 
			Timeset(1,gamephase);

>>>>>>> Stashed changes
		} else {
			console.log("[system] 현재 준비 인원 : " + conference_number);
		}
	});

	socket.on("exit_meeting", function (_data) {
		var data = JSON.parse(_data);
		console.log(
			"[system] 플레이어 " + data.name + " 이 미팅을 나가려고 합니다. "
		);
		exitMeeting_people += 1;
		// 모든 플레이어가 나가고 싶어할 때.
		if (exitMeeting_people == clients.length) {
			exitMeeting_people = 0;

			if (gamephase == 3) {
				// 자기소개 가 끝난 경우
				// phase : 탐색 4
				gamephase = 4;
				console.log(
					"[system] 자기소개를 끝내고 모든 플레이어가 나가고 싶어합니다. "
				);

				// 탐색화면으로 보내버리기
				clients.forEach(function (i) {
					io.to(i.id).emit("GO_MAP");
				}); //end_forEach

				// 탐색 시간 : 10분 = 600000
				setTimeout(function () {
					clearInterval(timerId);
					console.log("[system] 탐색시간이 끝났습니다. 1차회의로 갑니다. ");
					//
					gamephase = 5;
					clients.forEach(function (i) {
						// 탐색시간이 종료되면 미팅으로 보내야한다.
						// 같은 회의실 방을 사용하므로 뒤에 함수를 줘서 바꾸던가해야할듯
						clearInterval(timerId);
						// 탐색 후 회의 시간
						Timeset(15, gamephase);
						io.to(i.id).emit("GO_MEETING2");
					}); //end_forEach

					// 탐색 종료후 1분 뒤
					setTimeout(function () {
						console.log("[system] 새로운 증거를 공개합니다.");
						clients.forEach(function (i) {
							io.to(i.id).emit("VIEW_CLUE_VIDEO");
						}); //end_forEach
<<<<<<< Updated upstream
					}, 60000);
				}, 600000);
=======
					}, 60000); 

					
				}, 60000); 

				// 시간 보내주기 
				Timeset(1,gamephase);

>>>>>>> Stashed changes

				// 시간 보내주기
				Timeset(10, gamephase);
			} else if (gamephase == 5) {
				// 1차회의가 끝난 경우
				// phase 6 : 투표 씬
				gamephase = 6;
				console.log(
					"[system] 모든 플레이어가 1차회의를 마치고 나가고 싶어합니다. "
				);

				// 투표로 보내버리기
				clients.forEach(function (i) {
					// 여기다가 투표로 가는 메소드 주기
					console.log("[system] 투표하러 갑시다.");
					io.to(i.id).emit("GO_VOTE");
				}); //end_forEach

				// 투표 시간 : 1분 = 60000
				setTimeout(function () {
					if (gamephase != 7) {
						clearInterval(timerId);
						console.log("[system] 투표 시간이 끝났습니다. ");
						//
						gamephase = 7;
						clients.forEach(function (i) {
							// 투표가 끝난 경우
							clearInterval(timerId);
							// 결과 창으로 이동
							io.to(i.id).emit("GO_VOTE_RESULT", votes);
						}); //end_forEach
					}
				}, 60000);

				// 시간 보내주기
				Timeset(1, gamephase);
			}
		} else {
			console.log("[system] 회의 나가기 대기 인원 : " + exitMeeting_people);
		}
		// 변환
	});

	// 투표 결과
	socket.on("first_vote", function (data) {
		console.log("[INFO] player가 투표 했습니다.");
		first_vote_number++;

		if (data == "Ma") {
			votes[0]++;
		} else if (data == "Kim") {
			votes[1]++;
		} else if (data == "Chun") {
			votes[2]++;
		} else if (data == "Jang") {
			votes[3]++;
		} else if (data == "Choi") {
			votes[4]++;
		} else if (data == "Yun") {
			votes[5]++;
		}

		if (first_vote_number == 6) {
			console.log("[INFO] player 모두가 투표 했습니다.");
			gamephase = 7;
			clearInterval(timerId);
			clients.forEach(function (i) {
				io.to(i.id).emit("GO_VOTE_RESULT", votes);
			});
		}
	});
}); //END_IO.ON

var proof = require("./share_server");
proof.shareProof(io);

var voteResult = require("./vote_server");
voteResult.voteResult(io);

http.listen(process.env.PORT || 3000, function () {
	console.log("listening on *:3000");
});
console.log("------- server is running -------");

// 시간 세팅 함수
function Timeset(minutes, phase) {
	clearInterval(timerId);
	// 분단위로 받는다.
	// var settime = minutes * 60 * 1000;

	time = 0;
	minute = minutes - 1;
	second = 59;

	// 시간 보내주는 함수
	timerId = setInterval(function () {
		second--;
		// console.log(phase + " --> " + minute + ":" + second );
		if (phase == 2) {
			// 2 : 역할 시간 업데이트
			io.emit("SET_ROLE_TIMER", time, minute, second);
		} else if (phase == 3) {
			// 3. 미팅 시간 업데이트
			io.emit("SET_GAME_TIMER", time, minute, second);
		} else if (phase == 4) {
			// 4. 게임 시간 업데이트
			io.emit("SET_GAME_TIMER", time, minute, second);
		} else if (phase == 5) {
			// 5. 1차 회의 시간 업데이트
			io.emit("SET_GAME_TIMER", time, minute, second);
		} else if (phase == 6) {
			// 6. 투표 시간 업데이트
			io.emit("SET_VOTE_TIMER", time, minute, second);
		} else {
			clearInterval(timerId);
		}

		if (second == 0) {
			minute -= 1;
			second = 60;
		}
		// 나중에 min 0 && second 0 이 되었을 때 게임페이즈에 따라 맵으로 보내줘야함
	}, 1000);

	// 역할 배치 완료.
}
