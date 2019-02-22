"use strict";

window.onload = function() {

    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    var connection_id;
   
    connection.start().then(function () {
        connection.invoke("getConnectionId").then(function (connectionId) {
            console.log("getConnectionId " + connectionId);
            connection_id = connectionId;
        });

        console.log("connection: " + connection.id);
        document.getElementById("sendButton").disabled = false;
    }).catch(function (err) {
        return console.error(err.toString());
    });


    if (document.getElementById("sendButton") !== null) {
        //Disable send button until connection is established
        //document.getElementById("sendButton").disabled = true;
    }

    connection.on("GetConnectedUsers", function (data) {
        console.log(data);
        
        $("#usersCount").html(data.length-1);
        $(".nav").html();
        if (data.length - 1) {
            data.filter((item) => {
                return;
            }).map((item, key) => {
                console.log(key + "!==" + connection_id);
                if (key !== connection_id) {

                    var p = $("<p>" + item.username + "</p>");
                    var span = $("<span class='text-secondary'>" + new Date().getFullYear() + "</span>");
                    var div = $("<div class='ml-2'></div>").append(p, span);
                    var img = $("<img src='/img/" + item.image + "' class='my-auto img-round' width='34' height='34' />");
                    var navItem = $("<div class='nav-item d-flex'></div>").append(img, div);
                    $(".nav").append(navItem);
                }
            });
        } else {
            $(".nav").append($("<li class='my-4'><p class='text-white'>No one else is online.</p></li>"));
        }
        
    });

    connection.on("ReceiveMessage", function (user, message) {
        console.log(user + " " + message);
        var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        // Text
        var responseTxt = document.createTextNode(msg);
        var timeElapseTxt = document.createTextNode("4 mins ago");

        //
        var p = $("<p></p>").append(responseTxt);
        var msgDiv = $("<div class='msg-container receiver'></div>").append(p);
        var timeElapse = $("<span />").append(timeElapseTxt);
        var div = $("<div></div>").append(msgDiv, timeElapse);
        var img = $("<img src='/img/" + user.image + "' class='img-round' width='34' height='34' />");
        var messageDiv = $("<div class='message mr-auto'></div>").append(img, div);
        var messageContainer = $("<div class='message-container'></div>").append(messageDiv);
        //li.textContent = encodedMsg;

        $(".message-box").append(messageContainer);
    });

    connection.on("SentMessage", function (user, message) {
        console.log(user);
        var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        // Text
        var responseTxt = document.createTextNode(msg);
        var timeElapseTxt = document.createTextNode("4 mins ago");

        //
        var p = $("<p></p>").append(responseTxt);
        var msgDiv = $("<div class='msg-container sender'></div>").append(p);
        var timeElapse = $("<span />").append(timeElapseTxt);
        var div = $("<div></div>").append(msgDiv, timeElapse);
        var img = $("<img src='/img/" + user.image + "' class='img-round' width='34' height='34' />")
        var messageDiv = $("<div class='message ml-auto'></div>").append(div, img);
        var messageContainer = $("<div class='message-container'></div>").append(messageDiv);
        //li.textContent = encodedMsg;

        $(".message-box").append(messageContainer);
    });

    document.getElementById("sendButton").addEventListener("click", function (event) {
        var user = document.getElementById("userInput").value;
        var message = document.getElementById("messageInput").value;
        connection.invoke("SendMessage", connection_id, message).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });

    document.getElementById("sendButton").addEventListener("click", function (event) {
        //var username = document.getElementById("uname").value;
        //var password = document.getElementById("pwrd").value;
        /*connection.invoke("SendMessage", "user", "message").catch(function (err) {
            return console.error(err.toString());
        });*/
        event.preventDefault();
        //console.log("username: " + username + ", " + "password: " + password);
    });

};
