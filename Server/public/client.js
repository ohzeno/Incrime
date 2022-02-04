var socket = io() || {};
socket.isReady = false;

// chatroom client .js 
window.addEventListener('load', function() {

	var execInUnity = function(method) {
		if (!socket.isReady) return;
		
		var args = Array.prototype.slice.call(arguments, 1);
		
		f(window.unityInstance!=null)
		{
		  //fit formats the message to send to the Unity client game, take a look in NetworkManager.cs in Unity
		  window.unityInstance.SendMessage("NetWork_Start", method, args.join(':'));
		
		}
		
	};//END_exe_In_Unity 
	
					  
	// 조인이 성공했을 때
	// 콜백  JOIN SUCESS()    
	socket.on('JOIN_SUCCESS', function(id, name, totalplayer ) {
				      		
	  	var currentUserAtr = id+':'+name + ':' + totalplayer  ;
		if(window.unityInstance!=null)
		{	
			console.log("id : " + id + " 이름 : " + name + " 현재 이원 : " + totalplayer );
			window.unityInstance.SendMessage ('NetWork_Start', 'OnJoinGame', currentUserAtr);
		}
	  
	});//END_SOCKET.ON
		        
	// 끊겼을 때 아직 안함
	// socket.on('USER_DISCONNECTED', function(id) {
	//      var currentUserAtr = id;
	// 	if(window.unityInstance!=null)
	// 	{
	// 	 window.unityInstance.SendMessage ('', 'OnUserDisconnected', currentUserAtr);
	// 	}
	// });//END_SOCKET.ON

   socket.on('SET_TOTALPLAYER', function( totalplayer, mynum ) {
	   var data = totalplayer + ':' + mynum;
	   console.log("SET_TOTALPLAYER : " + data );
	   if(window.unityInstance!=null)
	   {
		   window.unityInstance.SendMessage ('NetWork_Wait', 'onTotalplayer', data);
	   }
	}); // end
	
	// 역할 배정
	socket.on('SET_ROLE', function(id, name, role, storyname , storydesc ) {
		
		var currentUserAtr = id+':'+name+':'+ role + ':' + storyname + ':' + storydesc;
		
		console.log(currentUserAtr);
		if(window.unityInstance!=null)
		{
			window.unityInstance.SendMessage ('NetWork_Wait', 'OnSetRole', currentUserAtr);
		}
	});//END_SOCKET.ON

	// 첫 회의 하기
	socket.on('go_firstconference', function() {
		if(window.unityInstance!=null)
		{
			window.unityInstance.SendMessage ('NetWork_Role', 'onConference');
		}
	});//END_SOCKET.ON

	// 타이머
	socket.on('SET_ROLE_TIMER', function (time, minute, second) {
		var timer = time + ':' + minute + ':' + second;
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage('NetWork_Role', 'RoleTimer', timer);
		}
	});

	// 미팅 내 타이머
	socket.on('SET_MEETING_TIMER', function (time, minute, second) {
		// console.log("미팅내타이머");
		var timer = time + ':' + minute + ':' + second;
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage('NetWork_Meeting', 'MeetingTimer', timer);
		}
	});
	
	// 맵으로 이동
	socket.on('GO_MAP', function() {
		if(window.unityInstance!=null)
		{
			window.unityInstance.SendMessage ('NetWork_Meeting', 'onExitMeeting');
		}
	});//END_SOCKET.ON

});//END_window_addEventListener

