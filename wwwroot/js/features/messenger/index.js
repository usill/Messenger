import { openModal, closeModal } from "../modal/events.js";
import { closePreloader } from "../preloader/events.js";
import { stopPropagationFor, preventDefaultFor } from "../../event.js";
import { setChatHeader, clearChat, openChat, closeChat, drawMessage, drawListMessages, drawContact, clearContacts } from "./ui.js";
import { logout, findUser, sendMessage } from "./api.js";
import { textAreaInput, findUserByForm } from "./events.js";

window.connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

window.connection.on("ReceiveMessage", (username, message) => {
    if (window.chatProxy.isOpen && window.chatProxy.user.Username == username) {
        drawMessage(message);
    }
});

window.connection.on("AddContact", (username, avatar, lastMessage) => {
    window.chatProxy.contacts.push({
        recipient: {
            Username: username,
            Avatar: avatar,
        },
        linkedMessage: {
            Text: lastMessage,
            SendedAt: Date.now(),
        }
    });
    drawContact(username, avatar, lastMessage);
});

window.connection.on("UpdateContact", (username, lastMessage) => {
    clearContacts();

    for (const contact of window.chatProxy.contacts) {
        if (contact.recipient.Username === username) {
            contact.linkedMessage.Text = lastMessage;
            contact.linkedMessage.SendedAt = Date.now();
        }
    }

    window.chatProxy.contacts.sort((a, b) => b.linkedMessage.SendedAt - a.linkedMessage.SendedAt);

    for (const contact of window.chatProxy.contacts)
    {
        drawContact(contact.recipient.Username, contact.recipient.Avatar, contact.linkedMessage.Text);
    }
})

window.connection.start();

const chat = {
    user: {
        Id: 0,
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
            for (var contact of value) {
                drawContact(contact.recipient.Username, contact.recipient.Avatar, contact.linkedMessage.Text);
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