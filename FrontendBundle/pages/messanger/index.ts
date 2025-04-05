import * as signalR from "@microsoft/signalr";

import { openModal, closeModal } from "../../modules/modal/events";
import { closePreloader } from "../../modules/preloader/events";
import { stopPropagationFor, preventDefaultFor } from "../../modules/event";
import { setChatHeader, clearChat, openChat, closeChat, drawMessage, drawListMessages, drawContact, clearContacts } from "./ui";
import { logout, sendMessage, findUser } from "./api";
import { textAreaInput, findUserByForm } from "./events";
import { clearError } from "../../modules/UI/input";
import { Contact } from "./types/Contact";
import { Messanger } from "./types/Messanger";
import { User } from "./types/User";
import { Message } from "./types/Message";

window.connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

window.connection.on("ReceiveMessage", (username, login, message) => {
    console.log("Получено сообщение: ", username);

    if (window.chatProxy.isChatOpen && window.chatProxy.user.Login == login) {
        drawMessage(message);
    }

    if (!window.chatProxy.isChatOpen || window.chatProxy.user.Login !== login) {
        const contact: Contact | undefined = window.chatProxy.contacts.find((item: Contact) => item.recipient.Login == login);

        if(!contact) return;

        contact.HasNewMessage = true;
    }
});

window.connection.on("AddContact", (username, login, avatar, lastMessage: string) => {
    console.log("Добавлен контакт:", login);

    const newContact: Contact = {
        HasNewMessage: true,
        recipient: {
            Login: login,
            Username: username,
            Avatar: avatar,
        } as User,
        linkedMessage: {
            Text: lastMessage,
            SendedAt: Date.now(),
        } as Message
    };

    window.chatProxy.contacts.push(newContact);
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

const chat: Messanger = {} as Messanger;

const chatHandler = {
    set(obj: Messanger, prop: string, value: User | Contact[] | Message[] | boolean) {
        obj[prop] = value;
        console.log(value);

        if (prop === "user") {
            value = value as User;
            setChatHeader(value);
            closeModal("new-chat-modal");
        }
        if (prop === "messages") {
            value = value as Message[];
            clearChat();
            drawListMessages(value, obj.user.Id ?? 0);
        }
        if (prop === "contacts") {
            clearContacts();

            value = value as Contact[]
            value.sort((a: Contact, b: Contact) => a.linkedMessage.SendedAt - b.linkedMessage.SendedAt);

            for (var contact of value as Contact[]) {
                drawContact(contact.recipient.Username, contact.recipient.Login, contact.recipient.Avatar, contact.linkedMessage.Text, contact.HasNewMessage);
            }
        }
        if (prop === "isChatOpen") {
            value = value as boolean;
            value === true ? openChat() : closeChat();
        }

        return true;
    },

}

window.chatProxy = new Proxy<Messanger>(chat, chatHandler);

document.addEventListener("DOMContentLoaded", () => {
    
    closePreloader("main-preloader");

    window.chatProxy.contacts = window.contacts;

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