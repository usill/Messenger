import { openModal, closeModal } from "../modal/events.js";
import { closePreloader } from "../preloader/events.js";
import { stopPropagationFor, preventDefaultFor } from "../../event.js";
import { setChatHeader, clearChat, openChat, closeChat, drawMessage, drawListMessages, drawContact, clearContacts } from "./ui.js";
import { logout, findUser, sendMessage } from "./api.js";
import { textAreaInput, findUserByForm } from "./events.js";

window.connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

window.connection.on("ReceiveMessage", (username, login, message) => {
    console.log("Получено сообщение: ", message);
    if (window.chatProxy.isOpen && window.chatProxy.user.Login == login) {
        drawMessage(message);
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
    drawContact(username, login, avatar, lastMessage);
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
        drawContact(contact.recipient.Username, contact.recipient.Login, contact.recipient.Avatar, contact.linkedMessage.Text);
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
                drawContact(contact.recipient.Username, contact.recipient.Login, contact.recipient.Avatar, contact.linkedMessage.Text);
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
    window.findUser = findUser;
    window.sendMessage = sendMessage;
    window.textAreaInput = textAreaInput;
    window.findUserByForm = findUserByForm;
})