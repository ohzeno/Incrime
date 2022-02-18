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

	socket.on('setrole', function ()
	{
		console.log("[system] 역할 배정 합니다.");
		if (clients.length == 6 ){
			// phase : 역할 배정 
			gamephase = 2;
			conference = false;
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

			// 
			setTimeout(function () {
				clearInterval(timerId);
				if ( gamephase == 2 ) {
					console.log("[system] 역할 확인 시간이 끝났습니다. 회의실로 갑니다. ");
					// 이동시키기
					conference = true;
					gamephase = 3;
					clients.forEach( function(i) {
						io.to(i.id).emit('go_firstconference');
					}); //end_forEach
				}
			}, 5000 ); 
			// 10 분 : 600000

			var time = 0;
			var minute = 9;
			var second = 58;

			let timerId = setInterval(function () {
				if ( gamephase == 2  ) {
					second--;
					io.emit('SET_ROLE_TIMER', time, minute, second);
					if ( second == 0 ){
						minute -= 1;
						second = 60;
					}
				} else {
					clearInterval(timerId);
				}
			}, 1000);
			// 역할 배치 완료.

			
		} else {
			console.log("We can't play crime scene")
		}

		
	});

	socket.on('go_firstconference', function ()
	{
		console.log("[system] 미팅실로 이동합니다.");
		conference_number += 1;
		// 모든 플레이어가 준비 됐을 때.
		if ( conference_number == clients.length ){
			
			// phase : 회의실
			gamephase = 3;
			console.log("[system] 모든 플레이어가 준비 상태 입니다. ");	
			
			conference = true;
			// 회의 실로 보내버리기
			clients.forEach( function(i) {
				io.to(i.id).emit('go_firstconference');
			});//end_forEach

			// 시간 제한 - 시간이 지나면 게임 화면으로 바꾸기
			setTimeout(function () {
				clearInterval(timerId);
				if ( gamephase == 3 ) {
					console.log("[system] 미팅 시간이 끝났습니다. ");
					// 이동시키기
					gamephase = 4;
					clients.forEach( function(i) {
						// 각자 게임 화면으로 보내버리기.
						io.to(i.id).emit('go_gamescene');
					}); //end_forEach
				}
			}, 600000); 
			// 10 분

			var time = 0;
			var minute = 9;
			var second = 58;
			var brief = false;


			let timerId = setInterval(function () {
				if ( gamephase == 3 ) {
					// console.log(minute + " " +second + " ");
					second--;
					io.emit('SET_MEETING_TIMER', time, minute, second);
					if ( second == 0 ){
						minute -= 1;
						second = 60;
					}
				} else {
					clearInterval(timerId);
				}
			}, 1000);
			// 역할 배치 완료.




		} else {
			console.log("[system] 현재 준비 인원 : " + conference_number );	
		}
	});
		
});//END_IO.ON


http.listen(process.env.PORT ||3000, function(){
	console.log('listening on *:3000');
});
console.log("------- server is running -------");