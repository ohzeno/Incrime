/*
*@autor: Rio 3D Studios
*@description:  java script server that works as master server of the Basic Example of WebGL Multiplayer Kit
*/
var express  = require('express');//import express NodeJS framework module
var app      = express();// create an object of the express module
var http     = require('http').Server(app);// create a http web server using the http library
var io       = require('socket.io')(http);// import socketio communication module


app.use("/public/TemplateData",express.static(__dirname + "/public/TemplateData"));
app.use("/public/Build",express.static(__dirname + "/public/Build"));
app.use(express.static(__dirname+'/public'));


// 사용하는 변수들 
var clients			= [];// to storage clients
var clientLookup = {};// clients search engine
var sockets = {};//// to storage sockets

// 게임단계 변수
var gamephase = 0;
// 나중에 복합적으로 관리하게 되면 phase를 리스트로 변경해야할듯.
// gamephase == 2 : 역할 대기화면
// gamephase == 3 : 회의실
// gamephase == 4 : 탐색

// 시간관련 변수
var time;
var minute;
var second;
// 타임 업데이트 함수
let timerId

// 나가기 사람
var exitMeeting_people = 0;
// 회의 가고싶어하는 사람
var conference_number = 0;
var conference = new Boolean(false);



// mySQL 연결 잘됨
var mysql      = require('mysql');
const { clearInterval } = require('timers');
// 
var connection = mysql.createConnection({
  host     : 'localhost',
  user     : 'root',
  password : '2413',
  database : 'project_db'
});
connection.connect();

//open a connection with the specific client
io.on('connection', function(socket){

   //print a log in node.js command prompt
  console.log('[system] 클라이언트가 연결되었습니다');
  
  //to store current client connection
  var currentUser;

	// 콜백  EmitJoin()
	socket.on('JOIN', function (_data)
	{
	    console.log('[system] player가 JOIN을 시도합니다. ');
		// 변환
		var data = JSON.parse(_data);
		
		currentUser = {
			id : socket.id, // 계속 사용할socket.id
			name : data.name,
			socketID : socket.id, // fills out with the id of the socket that was open
			usernumber : (clients.length +1 ), 
		};
					
		console.log(currentUser)
		console.log('[system] player '+currentUser.name+': 이 입장 했습니다.!');

		// clients list에 추가
		clients.push(currentUser);
		// add client in search engine
		clientLookup[currentUser.id] = currentUser;
		
		console.log('[system] 지금 참가자 수 : ' + clients.length);
		// 
		socket.emit("JOIN_SUCCESS",currentUser.id,currentUser.name, clients.length  );
		// Client.js 의 JOIN_SUCCESS 로 가셈 

	});//END_SOCKET_ON
	
	
    // 유저가 끊겼을 때
	socket.on('disconnect', function ()
	{
	    if(currentUser)
		{
			currentUser.isDead = true;
		
		//  socket.broadcast.emit('USER_DISCONNECTED', currentUser.id);
			
		for (var i = 0; i < clients.length; i++)
		{
			if (clients[i].name == currentUser.name && clients[i].id == currentUser.id) 
			{
				// console.log("User "+clients[i].name+" has disconnected");
				console.log("[system] "+clients[i].name+"이 연결이 끊어졌습니다.");
				clients.splice(i,1);
			};
		};
		}
		console.log("[system] 현재 연걸된 클라이언트 수 : " + clients.length );
    });//END_SOCKET_ON

	// 대기중인 인원을 표시하는 함수
	socket.on('totalplayer', function ()
	{
		clients.forEach( function(i) {
			io.to(i.id).emit('SET_TOTALPLAYER', clients.length , i.usernumber );
		});//end_forEach
	});

	// 역할 배정
	socket.on('setrole', function ()
	{
		console.log("[system] 역할 배정 합니다.");
		if (clients.length == 6 ){
			// phase : 역할 배정 
			gamephase = 2;
			console.log("[system] 플레이어가 모두 모였습니다. 역할을 배정합니다. ");	
			var role = [ 0, 0, 0, 0, 0, 0 ];
			for (let index = 1; index <= 6; index++) {
				while(true){
					var temp = Math.floor(Math.random() * 6);
					if ( role[temp] == 0 ){
						// 역할이 없을 경우
						role[temp] = index;
						break;
					}
				}
			}
			// console.log(role);
			// 역할 배치 완료.

			var roleindex = 0;
			clients.forEach( function(i) {

				var rolenumber = role[roleindex];
				var myrole;
				var SQL = "SELECT s.story_nm, s.story_description, r.role_name "
					+ " FROM story AS s INNER JOIN role AS r "
					+ " ON s.story_no = r.story_no "
					+ " WHERE s.story_no = 1"
					+ " AND r.role_id = " + rolenumber;
				connection.query(SQL, function(error, results, fields) {
					if (error) {
						console.log(error);
					} 
					// console.log(results);
					
					myrole = results[0].role_name;
					mystoryname = results[0].story_nm;
					mystorydesc = results[0].story_description;
										
					console.log( "[system] 역할 -" + myrole);				
					io.to(i.id).emit('SET_ROLE', i.id, i.name, myrole, mystoryname , mystorydesc );
				});

				roleindex += 1;
				
			});//end_forEach

			// 시간 초과 시 // 10분
			setTimeout(function() {
				if ( gamephase == 2 ) {
					clearInterval(timerId);
					console.log("[system] 역할 확인 시간이 끝났습니다. 회의실로 갑니다. ");
					// 이동시키기
					gamephase = 3;
					clients.forEach( function(i) {
						clearInterval(timerId);
						Timeset(10,gamephase);
						io.to(i.id).emit('GO_MEETING');
					}); //end_forEach
				}
			}, 600000 ); 
			// 10 분 : 600000
			// 시간 보내주기 
			Timeset(10,gamephase);

		} else {
			console.log("[system] We can't play crime scene")
		}

		
	});

	socket.on('go_firstconference', function ()
	{
		console.log("[system] 미팅실로 이동합니다.");
		conference_number += 1;
		// 모든 플레이어가 준비 됐을 때.
		if ( conference_number == clients.length ){
			conference_number = 0;
			// phase : 회의실
			gamephase = 3;
			console.log("[system] 모든 플레이어가 준비 상태 입니다. ");	
			
			conference = true;
			// 회의 실로 보내버리기
			clients.forEach( function(i) {
				io.to(i.id).emit('GO_MEETING');
			});//end_forEach

			// 시간 제한 - 시간이 지나면 게임 화면으로 바꾸기
			// 600000 - 10분
			setTimeout(function () {
				if ( gamephase == 3 ) {
					clearInterval(timerId);
					console.log("[system] 미팅 시간이 끝났습니다. ");
					// 이동시키기
					gamephase = 4;
					clients.forEach( function(i) {
						// 각자 게임 화면으로 보내버리면서 시간 처리
						clearInterval(timerId);
						Timeset(10,gamephase);
						io.to(i.id).emit('GO_MAP');
					}); //end_forEach
				}
			}, 60000); 
			// 시간 보내주기 
			Timeset(10,gamephase);

		} else {
			console.log("[system] 현재 준비 인원 : " + conference_number );	
		}
	});

	socket.on('exit_meeting', function (_data)
	{
		var data = JSON.parse(_data);
		console.log('[system] 플레이어 ' + data.name + ' 이 미팅을 나가려고 합니다. ');
		exitMeeting_people += 1;
		// 모든 플레이어가 준비 됐을 때.
		if ( exitMeeting_people == clients.length ){
			exitMeeting_people = 0;
			// phase : 탐색 4
			gamephase = 4;
			console.log("[system] 모든 플레이어가 나가고 싶어합니다. ");	

			// 탐색화면으로 보내버리기
			clients.forEach( function(i) {
				io.to(i.id).emit('GO_MAP');
			});//end_forEach


			// 시간 제한 - 시간이 지나면 게임 화면으로 바꾸기
			// 탐색 시간 : 10분 = 600000
			setTimeout(function () {
				clearInterval(timerId);
				console.log("[system] 탐색시간이 끝났습니다. ");
				// 
				gamephase = 5;
				clients.forEach( function(i) {
					// 탐색시간이 종료되면 미팅으로 보내야한다.
					// 같은 회의실 방을 사용하므로 뒤에 함수를 줘서 바꾸던가해야할듯
					clearInterval(timerId);
					// Timeset(10,gamephase);
					// io.to(i.id).emit('GO_MEETING');
				}); //end_forEach
				
			}, 600000); 
			// 시간 보내주기 
			Timeset(10,gamephase);



		} else {
			console.log("[system] 회의 나가기 대기 인원 : " + exitMeeting_people );	

		}
	    // 변환

	});
		
});//END_IO.ON


http.listen(process.env.PORT ||3000, function(){
	console.log('listening on *:3000');
});
console.log("------- server is running -------");


// 시간 세팅 함수
function Timeset(minutes, phase){

	clearInterval(timerId);
	// 분단위로 받는다.
	// var settime = minutes * 60 * 1000;

	time = 0;
	minute = minutes - 1;
	second = 59;
	
	// 시간 보내주는 함수
	timerId = setInterval( function () {
		second--;
		console.log(phase + " --> " + minute + ":" + second );
		if ( phase == 2  ) {
			// 2 : 역할 시간 업데이트
			io.emit('SET_ROLE_TIMER', time, minute, second);
		} else if ( phase == 3 ) {
			// 3. 미팅 시간 업데이트
			io.emit('SET_MEETING_TIMER', time, minute, second);
		} else if ( phase == 4 ) {
			// 4. 게임 시간 업데이트
			io.emit('SET_GAME_TIMER', time, minute, second);
		} else {
			clearInterval(timerId);
		}
		
		if ( second == 0 ){
			minute -= 1;
			second = 60;
		}
	}, 1000);


	// 역할 배치 완료.
};