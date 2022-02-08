var socket = io() || {};
socket.isReady = false;

// chatroom client .js 
window.addEventListener('load', function() {
    // 첫 투표 씬 가기
	socket.on('GO_VOTE', function() {
		if(window.unityInstance!=null)
		{
			window.unityInstance.SendMessage ('NetWork_Meeting', 'onVote');
		}
	});//END_SOCKET.ON

    // 첫 투표 결과
    socket.on('GO_VOTE_RESULT', function(votes) {
		console.log("[client.js] 첫 투표 들어옴");
		var vote = votes[0] + ':' +votes[1] + ':' +votes[2] + ':' + votes[3] + ':' + votes[4] + ':' + votes[5];
		if(window.unityInstance!=null){
			window.unityInstance.SendMessage ('ClickController', 'onVote', vote);
		}
	});//END_SOCKET.ON
});