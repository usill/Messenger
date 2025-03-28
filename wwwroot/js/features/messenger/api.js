﻿import { drawMessage } from "./ui.js";

export const findUser = async (username) => {
    const response = await fetch(`/api/user/find/${username}`);

    if (response.ok) {
        const findUser = await response.json();
        window.chatProxy.user = findUser.recipient;
        window.chatProxy.messages = findUser.linkedMessages;
        window.chatProxy.isOpen = true;
    }
}

export const sendMessage = () => {
    const textarea = document.querySelector("#chat-textarea");

    const message = textarea.value;
    const userId = JSON.stringify(window.chatProxy.user.Id);
    window.connection.invoke("SendMessage", userId, message).catch(function (error) {
        return console.error(error.toString());
    });

    drawMessage(message, true);
    textarea.value = "";
}

export const logout = async () => {
    const response = await fetch("/api/auth/logout", {
        method: "GET",
        credentials: "include"
    });

    if (response.ok) {
        location.href = "/login";
    }
}