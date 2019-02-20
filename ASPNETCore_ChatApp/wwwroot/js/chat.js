"use strict";

window.onload = function() {

    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    if (document.getElementById("sendButton") !== null) {
        //Disable send button until connection is established
        //document.getElementById("sendButton").disabled = true;
    }

    connection.on("ReceiveMessage", function (user, message) {
        var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        // Text
        var responseTxt = document.createTextNode(msg);
        var timeElapseTxt = document.createTextNode("4 mins ago");

        //
        var p = $("<p></p>").append(responseTxt);
        var msgDiv = $("<div class='msg-container'></div>").append(p);
        var timeElapse = $("<span />").append(timeElapseTxt);
        var div = $("<div></div>").append(msgDiv, timeElapse);
        //var img = document.createElement("img").setAttribute("", "");
        var messageDiv = $("<div class='message mr-auto'></div>").append(div);
        var messageContainer = $("<div></div>").append(messageDiv);
        //li.textContent = encodedMsg;

        $(".message-box").append(messageContainer);
    });

    connection.start().then(function () {
        document.getElementById("sendButton").disabled = false;
    }).catch(function (err) {
        return console.error(err.toString());
    });

    document.getElementById("sendButton").addEventListener("click", function (event) {
        var user = document.getElementById("userInput").value;
        var message = document.getElementById("messageInput").value;
        connection.invoke("SendMessage", user, message).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });

    document.getElementById("sendButton").addEventListener("click", function (event) {
        //var username = document.getElementById("uname").value;
        //var password = document.getElementById("pwrd").value;
        connection.invoke("SendMessage", "user", "message").catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
        //console.log("username: " + username + ", " + "password: " + password);
    });

};
