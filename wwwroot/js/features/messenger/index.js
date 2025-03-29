import { openModal, closeModal } from "../modal/events.js";
import { closePreloader } from "../preloader/events.js";
import { stopPropagationFor, preventDefaultFor } from "../../event.js";
import { setChatHeader, clearChat, openChat, closeChat, drawMessage, drawListMessages, drawContact } from "./ui.js";
import { logout, findUser, sendMessage } from "./api.js";
import { textAreaInput, findUserByForm } from "./events.js";

window.connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

window.connection.on("ReceiveMessage", (message) => {
    drawMessage(message);
});

window.connection.on("AddContact", (username, avatar, lastMessage) => {
    drawContact(username, avatar, lastMessage);
});

window.connection.start();

const chat = {
    user: {
        Id: 0,
        Username: "",
        Avatar: "",
    },
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
        if (prop === "isOpen") {
            value === true ? openChat() : closeChat();
        }

        return true;
    },

}

window.chatProxy = new Proxy(chat, chatHandler);

document.addEventListener("DOMContentLoaded", () => {

    closePreloader("main-preloader");

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