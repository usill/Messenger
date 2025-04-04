﻿import { openModal, closeModal } from "../modal/events";
import { closePreloader } from "../preloader/events";
import { stopPropagationFor, preventDefaultFor } from "../../event";
import { setChatHeader, clearChat, openChat, closeChat, drawMessage, drawListMessages, drawContact, clearContacts } from "./ui";
import { logout, sendMessage, findUser } from "./api";
import { textAreaInput, findUserByForm } from "./events";
import { clearError } from "../../ui/input";

window.connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

window.connection.on("ReceiveMessage", (username, login, message) => {
    console.log("Получено сообщение: ", message);
    if (window.chatProxy.isOpen && window.chatProxy.user.Login == login) {
        drawMessage(message);
    }

    if (!window.chatProxy.isOpen || window.chatProxy.user.Login !== login) {
        const contact = window.chatProxy.contacts.find((item) => item.recipient.Login == login);
        contact.HasNewMessage = true;
    }
});

window.connection.on("AddContact", (username, login, avatar, lastMessage) => {
    console.log("Добавлен контакт:", login);
    window.chatProxy.contacts.push({
        recipient: {
            Login: login,
            Username: username,
            Avatar: avatar,
        },
        linkedMessage: {
            Text: lastMessage,
            SendedAt: Date.now(),
        }
    });
    drawContact(username, login, avatar, lastMessage, true);
});

window.connection.on("UpdateContact", (login, lastMessage) => {
    console.log("перерисовка контактов");
    clearContacts();

    for (const contact of window.chatProxy.contacts) {
        if (contact.recipient.Login === login) {
            contact.linkedMessage.Text = lastMessage;
            contact.linkedMessage.SendedAt = Date.now();
        }
    }

    window.chatProxy.contacts.sort((a, b) => a.linkedMessage.SendedAt - b.linkedMessage.SendedAt);

    for (const contact of window.chatProxy.contacts)
    {
        drawContact(contact.recipient.Username, contact.recipient.Login, contact.recipient.Avatar, contact.linkedMessage.Text, contact.HasNewMessage);
    }
})

window.connection.start();

const chat = {
    user: {
        Id: 0,
        Login: "",
        Username: "",
        Avatar: "",
    },
    contacts: [],
    messages: [],
    isOpen: false,
}

const chatHandler = {
    set(obj, prop, value) {
        obj[prop] = value;
        console.log(value);

        if (prop === "user") {
            setChatHeader(value);
            closeModal("new-chat-modal");
        }
        if (prop === "messages") {
            clearChat();
            drawListMessages(value, obj.user.Id);
        }
        if (prop === "contacts") {
            clearContacts();

            value.sort((a, b) => a.linkedMessage.SendedAt - b.linkedMessage.SendedAt);

            for (var contact of value) {
                drawContact(contact.recipient.Username, contact.recipient.Login, contact.recipient.Avatar, contact.linkedMessage.Text, contact.HasNewMessage);
            }
        }
        if (prop === "isOpen") {
            value === true ? openChat() : closeChat();
        }

        return true;
    },

}

window.chatProxy = new Proxy(chat, chatHandler);

document.addEventListener("DOMContentLoaded", () => {
    
    closePreloader("main-preloader");

    chatProxy.contacts = window.contacts;

    window.openModal = openModal;
    window.closeModal = closeModal;
    window.stopPropagationFor = stopPropagationFor;
    window.preventDefaultFor = preventDefaultFor;
    window.logout = logout;
    window.sendMessage = sendMessage;
    window.textAreaInput = textAreaInput;
    window.findUserByForm = findUserByForm;
    window.findUser = findUser;
    window.clearError = clearError;
})