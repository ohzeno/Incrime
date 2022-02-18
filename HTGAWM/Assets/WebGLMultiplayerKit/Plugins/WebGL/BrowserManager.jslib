mergeInto(LibraryManager.library, {

  OpenConnection: function () {
  
    socket.isReady = true;
	console.log("connected");
  },

  Emit: function (callback_id,data) {
  
   console.log("waiting to send message");
   if(data!="")
   {
      console.log("sending message to server");
	   var RegExp = /[{|}]/g;
 	var pack =data.toString().replace(RegExp,"");
    alert(data);
	
	 result ='{'+data+'}';
	  alert(result);
	 socket.emit(callback_id, JSON.parse(data));
   
	  console.log("message sent");
   }
   else
   {
     console.log("try to send message to server");
    socket.emit(callback_id);
	 console.log("message sent");
   }
   
  },
 

});