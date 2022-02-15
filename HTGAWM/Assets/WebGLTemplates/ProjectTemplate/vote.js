
// chatroom client .js 
window.addEventListener('load', function() {
    // 첫 투표 씬 가기
	socket.on('GO_VOTE', function() {
		if(window.unityInstance!=null)
		{
			window.unityInstance.SendMessage ('PlayerObject', 'onVote');
		}
	});//END_SOCKET.ON

    // 첫 투표 결과
    socket.on('GO_VOTE_RESULT', function(votes) {
		console.log("[system] 첫 투표가 끝이 났습니다.");
		var vote = votes[0] + ':' +votes[1] + ':' +votes[2] + ':' + votes[3] + ':' + votes[4] + ':' + votes[5];
		if(window.unityInstance!=null){
			window.unityInstance.SendMessage ('ClickController', 'onVote', vote);
		}
	});//END_SOCKET.ON

	// 투표 내 타이머
	socket.on('SET_VOTE_TIMER', function (time, minute, second) {
		var timer = time + ':' + minute + ':' + second;
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage('ClickController', 'VoteTimer', timer);
		}
	});//END_SOCKET.ON

	// 투표에서 동표가 아닐경우
	socket.on('GO_SINGLE_RESULT_VOTE', function (data) {
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage('Typing', 'VoteTextResult', data);
		}
	});//END_SOCKET.ON

	// 투표에서 동표일 경우
	socket.on('GO_MULTI_RESULT_VOTE', function (data) {

		console.log("여기가 문제지?");
		var vote = "";
		data.forEach(function(i) {
			if(i != null){
				vote += i + ":";
			}
		});
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage('Typing', 'MultiVoteTextResult', vote);
		}
	});//END_SOCKET.ON

	// 두번째 투표 씬으로 이동
	socket.on('GO_MOVE_SECOND_VOTE', function (data) {
		var again = "";
		data.forEach(function(i) {
			if(i != null){
				again += i + ":";
			}
		});
		console.log("vote.js" + again);
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage('VoteSecondController', 'SecondVote', again);
		}
	});//END_SOCKET.ON

	// 투표 내 타이머
	socket.on('SET_SECOND_VOTE_TIMER', function (time, minute, second) {
		var timer = time + ':' + minute + ':' + second;
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage('VoteSecondController', 'VoteTimer', timer);
		}
	});//END_SOCKET.ON

	// 두번째 투표 결과 창으로 이동
	socket.on('GO_SECOND_VOTE', function(votes){
		console.log("[vote.js] 두번째 투표 들어옴");
		var vote = votes[0] + ':' +votes[1] + ':' +votes[2] + ':' + votes[3] + ':' + votes[4] + ':' + votes[5];
		if(window.unityInstance!=null){
			window.unityInstance.SendMessage ('VoteSecondController', 'onVote', vote);
		}
	})

	// 두번째 투표에서 동표가 아닐경우
	socket.on('GO_SINGLE_RESULT_SECOND_VOTE', function (data) {
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage('Typing', 'VoteTextResult', data);
		}
	});//END_SOCKET.ON

	// 두번째 투표에서 동표일 경우
	socket.on('GO_MULTI_RESULT_SECOND_VOTE', function (data) {
		var vote = "";
		data.forEach(function(i) {
			if(i != null){
				vote += i + ":";
			}
		});
		if (window.unityInstance != null) {
			window.unityInstance.SendMessage('Typing', 'MultiSecondVoteTextResult', vote);
		}
	});//END_SOCKET.ON
	
});
