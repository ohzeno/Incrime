var sigle_votenum = 0;
var multi_votenum = 0;
var second_votenum = 0;

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
            }
            
        }); //END_SOCKET_ON

        socket.on("SECOND_VOTE", function(data){ // 두번째 투표
            second_votenum++;
            if(second_votenum == 6){
                console.log("vote_server.js " + data);
                io.emit('GO_SECOND_VOTE',data);
            }
        }); //END_SOCKET_ON

      });
    },
  };
  