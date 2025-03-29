import { openModal, closeModal } from "../modal/events.js";
import { closePreloader } from "../preloader/events.js";
import { stopPropagationFor, preventDefaultFor } from "../../event.js";
import { setChatHeader, clearChat, openChat, closeChat, drawMessage, drawListMessages } from "./ui.js";
import { logout, findUser, sendMessage } from "./api.js";

const connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

connection.on("ReceiveMessage", (message) => {
    drawMessage(message);
});

connection.start();

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

const chatProxy = new Proxy(chat, chatHandler);

document.addEventListener("DOMContentLoaded", () => {

    closePreloader("main-preloader");

    window.openModal = openModal;
    window.closeModal = closeModal;
    window.stopPropagationFor = stopPropagationFor;
    window.preventDefaultFor = preventDefaultFor;
    window.logout = logout;
    window.findUser = findUser;
    window.sendMessage = sendMessage;
})