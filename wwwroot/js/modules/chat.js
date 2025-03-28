"use strict";

const sendButton = document.getElementById("sendButton");
const connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();
sendButton.disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);

    li.textContent = `${user} says ${message}`;
});

connection.start().then(function () {
    sendButton.disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
})

sendButton.addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;

    connection.invoke("SendMessage", user, message).catch(function (error) {
        return console.error(err.toString());
    })

    event.preventDefault();
});