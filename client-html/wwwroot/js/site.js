"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("http://localhost:5000/hubs/IOHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendContent", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

window.addEventListener("beforeunload", function() {
    connection.off("ReceiveOutput");
});

window.addEventListener("load", function() {
    connection.start().then(function () {
        document.getElementById("sendButton").disabled = false;
    }).catch(function (err) {
        return console.error(err.toString());
    });
    
    connection.on("ReceiveOutput", function (user, message) {
        var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        var encodedMsg = user + " says " + msg;
        var li = document.createElement("li");
        li.textContent = encodedMsg;
        document.getElementById("messagesList").appendChild(li);
    }); 
});