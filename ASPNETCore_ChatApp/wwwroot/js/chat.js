"use strict";

window.onload = function () {

    document.getElementsByClassName("chat-sect")[0].style.display = "none";
    document.getElementsByClassName("no-chat")[0].style.display = "flex";


    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    var username = window.sessionStorage.getItem("uname");
    var senderId;
    var receiverId;
    var userConnId;

    connection.start().then(function () {
        document.getElementById("sendButton").disabled = false;

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
                var span = $("<span class='text-secondary text-nowrap'>" + item.date + "</span>");
                var div = $("<div class='ml-2'></div>").append(p, span);
                var img = $("<img src='/img/" + item.image + "' class='my-auto img-round' width='34' height='34' />");
                var navItem = $("<div class='nav-item d-flex' data-receiverid='"+item.id+"' data-uname='" + item.username + "' data-connId='" + item.connectionId + "'></div>").append(img, div);
                $(".nav").append(navItem);
            });
        } else {
            $(".nav").html($("<li class='my-2'><p class='text-white'>No other user online.</p></li>"));
        }

    });

    connection.on("ReceiveMessage", function (message) {
        console.log(message);
        receivedMessage(message);
    });

    connection.on("SentMessage", function (message) {
        console.log(message);
        sentMessage(message);
    });

    $(".nav").on("click", ".nav-item", function (event) {
        $(".message-box").empty();
        var width = window.innerWidth;
        var username = event.currentTarget.dataset.uname;
        senderId = document.getElementById("userImg").dataset.senderid;
        receiverId = event.currentTarget.dataset.receiverid;
        userConnId = event.currentTarget.dataset.connid;

        $('.nav div.active').removeClass('active');
        $(this).addClass('active');
        $(this).find('p').addClass("text-dark");
        console.log("senderId: " + senderId);
        console.log("receiverId: " + receiverId);

        document.getElementsByClassName("chat-sect")[0].style.display = "flex";
        document.getElementsByClassName("no-chat")[0].style.display = "none";
        document.getElementById("convo_partner_name").innerHTML = username;

        if (width <= 574) {
            pageTransition();
        }

        connection.invoke("GetMessages", senderId, receiverId).then(function (data) {
            console.log("data: ");
            console.log(data);
            
            data.forEach(function (data) {
                console.log("data.from: " + data.from + " senderId: " + senderId);

                if (parseInt(data.from) === parseInt(senderId)) {
                    console.log("true");
                    sentMessage(data);
                }
                else {
                    receivedMessage(data);
                }; 
            });
        });
    });

    document.getElementById("sendButton").addEventListener("keypress", function (event) {
        console.log(event.currentTarget.value);
    });

    document.getElementById("sendButton").addEventListener("click", function (event) {
        var message = document.getElementById("messageInput").value;
        connection.invoke("SendMessage", senderId, receiverId, message, userConnId).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });

    function pageTransition() {
        console.log("click");
        //document.getElementsByClassName("message-panel").style.display = "flex";
        document.getElementsByClassName("list-panel")[0].style.width = 0;
        document.getElementsByClassName("list-panel")[0].style.padding = 0;

        document.getElementsByClassName("message-panel")[0].style.width = "100%";
        document.getElementsByClassName("message-panel")[0].style.padding = "0 10px";

    };

    function receivedMessage(message) {
        var msg = message.message_text.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        // Text
        var responseTxt = document.createTextNode(msg);
        var timeElapseTxt = document.createTextNode(message.date_sent);
        var p = $("<p></p>").append(responseTxt);
        var msgDiv = $("<div class='msg-container receiver'></div>").append(p);
        var timeElapse = $("<span />").append(timeElapseTxt);
        var div = $("<div></div>").append(msgDiv, timeElapse);
        var img = $("<img src='/img/" + message.image + "' class='img-round' width='34' height='34' />");
        var messageDiv = $("<div class='message mr-auto'></div>").append(img, div);
        var messageContainer = $("<div class='message-container'></div>").append(messageDiv);

        $(".message-box").append(messageContainer);
    }

    function sentMessage(message) {
        var msg = message.message_text.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        // Text
        var responseTxt = document.createTextNode(msg);
        var timeElapseTxt = document.createTextNode(message.date_sent);
        var p = $("<p></p>").append(responseTxt);
        var msgDiv = $("<div class='msg-container sender'></div>").append(p);
        var timeElapse = document.createElement("span").textContent = timeElapseTxt;
        timeElapse.setAttribute("class", "text-right");
        var div = $("<div></div>").append(msgDiv, timeElapse);
        var img = $("<img src='/img/" + message.image + "' class='img-round' width='34' height='34' />");
        var messageDiv = $("<div class='message ml-auto'></div>").append(div, img);
        var messageContainer = $("<div class='message-container'></div>").append(messageDiv);
        //li.textContent = encodedMsg;

        $(".message-box").append(messageContainer);
    }

    document.getElementById("backBtn").addEventListener("click", function () {
        console.log("click");
        //document.getElementsByClassName("message-panel").style.display = "flex";
        document.getElementsByClassName("list-panel")[0].style.width = "100%";
        document.getElementsByClassName("list-panel")[0].style.padding = "0 10px";

        document.getElementsByClassName("message-panel")[0].style.width = 0;
        document.getElementsByClassName("message-panel")[0].style.padding = 0;

    });

};
