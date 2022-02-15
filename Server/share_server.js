module.exports = {
  shareProof: function (io) {
    io.on("connection", function (socket) {
      //to store current client connection
      var currentUser;
      // 콜백  EmitJoin()
      socket.on("SHARE_PROOF", function (_data, _roomNumber ) {
        console.log("Broadcast 증거 공유");
        socket.broadcast.to( 'room' + _roomNumber ).emit("SHARE_PROOF", _data);
      }); //END_SOCKET_ON
    });
  },
};
