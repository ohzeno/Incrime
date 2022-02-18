/** @format */

// chatroom client .js
window.addEventListener("load", function () {
	// 탐색 후 1차 회의 시간
	socket.on("SHARE_PROOF", function (_data) {
		if (window.unityInstance != null) {
			console.log(window.unityInstance);
			console.log("공유받은 증거 Unity로 전송");
			window.unityInstance.SendMessage("Player", "ReceiveSharedProof", _data);
		}
	}); //END_SOCKET.ON
}); //END_window_addEventListener
