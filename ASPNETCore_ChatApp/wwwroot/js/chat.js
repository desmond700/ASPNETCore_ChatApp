"use strict";

window.onload = function () {

    document.getElementsByClassName("chat-sect")[0].style.display = "none";
    document.getElementsByClassName("no-chat")[0].style.display = "flex";


    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    var username = window.sessionStorage.getItem("uname");
    var userId;

    window.connection_id = "connect test";

    connection.start().then(function () {
        connection.invoke("getConnectionId").then(function (connectionId) {
            console.log("get cookie: " + document.cookie);
            console.log("getConnectionId " + connectionId);

            //console.log("username: " + window.username);
            console.log(window.username);
            document.getElementById("sendButton").disabled = false;
        }).catch(function (err) {
            return console.error(err.toString());
        });

        if (document.getElementById("sendButton") !== null) {
            //Disable send button until connection is established
            //document.getElementById("sendButton").disabled = true;
        }
    });

    connection.on("GetConnectedUsers", function (data) {
        console.log("data lenght: " + data.length - 1);
        console.log(data);
        $("#usersCount").html(data.length - 1);
        $(".nav").children().remove();

        if (data.length - 1) {
            data.filter((item) => {
                return item.username !== username;
            }).map((item, key) => {
                console.log("key: "+key);
                var p = $("<p>" + item.username + "</p>");
                var span = $("<span class='text-secondary'>" + new Date().getFullYear() + "</span>");
                var div = $("<div class='ml-2'></div>").append(p, span);
                var img = $("<img src='/img/" + item.image + "' class='my-auto img-round' width='34' height='34' />");
                var navItem = $("<div class='nav-item d-flex' data-uname='" + item.username + "' data-connId='" + item.connectionId + "'></div>").append(img, div);
                $(".nav").append(navItem);
            });
        } else {
            $(".nav").html($("<li class='my-2'><p class='text-white'>No other user online.</p></li>"));
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

    $(".nav").on("click", ".nav-item", function (event) {

        $('.nav div.active').removeClass('active');
        $(this).addClass('active');
        $(this).find('p').addClass("text-dark");
        console.log($(this).find('p'));
        console.log($(".nav").children());
        var username = event.currentTarget.dataset.uname;
        userId = event.currentTarget.dataset.connid;
        console.log("userId: "+userId);
        document.getElementsByClassName("chat-sect")[0].style.display = "flex";
        document.getElementsByClassName("no-chat")[0].style.display = "none";
        document.getElementById("convo_partner_name").innerHTML = username;
    });

    document.getElementById("sendButton").addEventListener("click", function (event) {
        var message = document.getElementById("messageInput").value;
        connection.invoke("SendMessage", userId, message).catch(function (err) {
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


    $("form").submit(function () {
        alert($("#username").val());
        //window.username = $("#username").val();
    });
};
