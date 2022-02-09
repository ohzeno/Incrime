module.exports = {
  shareProof: function (io) {
    io.on("connection", function (socket) {
      //to store current client connection
      var currentUser;
      // 콜백  EmitJoin()
      socket.on("SHARE_PROOF", function (_data) {
        console.log("Broadcast 증거 공유");
        socket.broadcast.emit("SHARE_PROOF", _data);
      }); //END_SOCKET_ON
    });
  },
};
