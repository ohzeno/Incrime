var sigle_votenum = 0;
var multi_votenum = 0;
var second_votenum = 0; // 두번째보트씬 접속 수
var second_vote_number = 0; // 두번째 vote btn 클릭 수
var second_votes = [0,0,0,0,0,0];
var gamephase = 1; // 두번째 투표에서 버튼 클릭 시 시간 억제 기능
let timerId;
var clients	= [];
module.exports = {
    voteResult: function (io) {
      io.on("connection", function (socket) {
        //to store current client connection
        var currentUser;
        // 콜백  EmitJoin()
        socket.on("SINGLE_RESULT_VOTE", function (data) { // 동표가 안나왔을 때
            
            sigle_votenum++;
            if(sigle_votenum == 6){
                console.log("텍스트 싱글 결과 창 " + data);
                io.emit('GO_SINGLE_RESULT_VOTE', data);
                sigle_votenum = 0;
            }
          
        }); //END_SOCKET_ON

        socket.on("MULTI_RESULT_VOTE", function (data) { // 동표일 때
            multi_votenum++;
            if(multi_votenum == 6){
                console.log("텍스트 멀티 결과 창 " + data);
                io.emit('GO_MULTI_RESULT_VOTE', data);
                multi_votenum = 0;
            }
            
        }); //END_SOCKET_ON

        socket.on("MOVE_SECOND_VOTE", function(data){ // 두번째 투표
            currentUser = {
                id : socket.id, // 계속 사용할socket.id
            };
            clients.push(currentUser);
            second_votenum++;
            console.log("두번째투표 접속자 수 " + second_votenum);
            if(second_votenum == 6){
                console.log("vote_server.js " + data);
                io.emit('GO_MOVE_SECOND_VOTE',data);
            }

            setTimeout(function () {
                if(gamephase != 2){
                    clearInterval(timerId);
                    console.log("[system] 투표 시간이 끝났습니다. ");
                    // 
                    gamephase = 2;
                    clients.forEach( function(i) {
                        // 투표가 끝난 경우
                        clearInterval(timerId);
                        // 결과 창으로 이동
                        io.to(i.id).emit('GO_SECOND_VOTE', second_votes);
                    }); //end_forEach
                }
                
            }, 60000); 

            Timeset(1, gamephase); // 1분 동안 투표
        }); //END_SOCKET_ON

        socket.on("SECOND_VOTE", function(data){ // 투표 버튼 클릭 시
            second_vote_number++; // 접속 유저

            if(data == "Ma"){ 
                second_votes[0]++;
            }else if(data == "Kim"){
                second_votes[1]++;
            }else if(data == "Chun"){
                second_votes[2]++;
            }else if(data == "Jang"){
                second_votes[3]++;
            }else if(data == "Choi"){
                second_votes[4]++;
            }else if(data == "Yun"){
                second_votes[5]++;
            }
            if(second_vote_number == 6){
                io.emit('GO_SECOND_VOTE', second_votes);
                gamephase = 2;
                clearInterval(timerId);
            }
        }); //END_SOCKET_ON

        socket.on("SINGLE_RESULT_SECOND_VOTE", function (data) { // 동표가 안나왔을 때
            
            sigle_votenum++;
            if(sigle_votenum == 6){
                console.log("텍스트 싱글 결과 창 " + data);
                io.emit('GO_SINGLE_RESULT_SECOND_VOTE', data);
                sigle_votenum = 0;
            }
          
        }); //END_SOCKET_ON

        socket.on("MULTI_RESULT_SECOND_VOTE", function (data) { // 동표일 때
            multi_votenum++;
            console.log("두번째 투표 결과 접속 수 " + multi_votenum);
            if(multi_votenum == 6){
                console.log("텍스트 멀티 결과 창 " + data);
                io.emit('GO_MULTI_RESULT_SECOND_VOTE', data);
            }
            
        }); //END_SOCKET_ON

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
                // console.log(phase + " --> " + minute + ":" + second );
                if ( phase == 1  ) {
                    // 2 : 역할 시간 업데이트
                    io.emit('SET_SECOND_VOTE_TIMER', time, minute, second);
                } else {
                    clearInterval(timerId);
                }
                
                if ( second == 0 ){
                    minute -= 1;
                    second = 60;
                }
                // 나중에 min 0 && second 0 이 되었을 때 게임페이즈에 따라 맵으로 보내줘야함

            }, 1000);


            // 역할 배치 완료.
        };

      });
    },
};

