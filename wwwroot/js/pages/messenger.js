import { openModal, closeModal } from "../modules/modal.js";
import { closePreloader } from "../modules/preloader.js";
import { stopPropagationFor, preventDefaultFor } from "../modules/event.js";
import { setChatHeader, clearChat, openChat, closeChat } from "../proxy/messanger.js";

const connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

connection.on("ReceiveMessage", (message) => {
    const li = document.createElement("li");
    const chat = document.querySelector("#chat-content");

    li.classList.add("message");
    li.textContent = message;

    chat.appendChild(li);
});

connection.start();

const chat = {
    user: {
        Id: 0,
        Username: "",
        Avatar: "",
        Connections: [],
    },
    isOpen: false,
}

const chatHandler = {
    set(obj, prop, value) {
        obj[prop] = value;

        if (prop === "user") {
            setChatHeader(value);
            clearChat();
            closeModal("new-chat-modal");
        }
        if (prop === "isOpen") {
            value === true ? openChat() : closeChat();
        }

        return true;
    },

}

const chatProxy = new Proxy(chat, chatHandler);

document.addEventListener("DOMContentLoaded", () => {
    const logout = async () => {
        const response = await fetch("/api/auth/logout", {
            method: "GET",
            credentials: "include"
        });

        if (response.ok) {
            location.href = "/login";
        }
    }

    const findUser = async (clickedButton) => {
        const form = clickedButton.closest("form");
        const formData = new FormData(form);
        const username = formData.get("username");

        const response = await fetch(`/api/user/find/${username}`);

        if (response.ok) {
            const findUser = await response.json();
            chatProxy.user = findUser;
            chatProxy.isOpen = true;
        }
    }

    const sendMessage = () => {
        const textarea = document.querySelector("#chat-textarea");

        const connections = chatProxy.user.Connections;
        const message = textarea.value;
        console.log(1);
        connection.invoke("SendMessage", connections, message).catch(function (error) {
            return console.error(err.toString());
        });
    }

    closePreloader("main-preloader");

    window.openModal = openModal;
    window.closeModal = closeModal;
    window.stopPropagationFor = stopPropagationFor;
    window.preventDefaultFor = preventDefaultFor;
    window.logout = logout;
    window.findUser = findUser;
    window.sendMessage = sendMessage;
})