import * as signalR from "@microsoft/signalr";
import { Contact } from "./types/Contact";
import { Message } from "./types/Message";
import { User } from "./types/User";
import { clearContacts, clearNotification, drawContact, drawMessage } from "./ui";
import { FindedContact } from "./types/FindedContact";
import { showError } from "../../modules/UI/input";

export enum NotifyState {
    Visible,
    Invisible
}

export const checkNotify = (login: string, state: NotifyState) => {
    const contact: Contact | undefined = window.chatProxy.contacts.find((item: Contact) => item.recipient.Login == login);
    if (!contact) return;

    if ((!window.chatProxy.isChatOpen || window.chatProxy.user.Login !== login) && state === NotifyState.Invisible) {
        contact.HasNewMessage = true;
    }

    if(state === NotifyState.Visible) {
        contact.HasNewMessage = false;
    }
}

export const initConnection = () => {
    window.connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

    window.connection.on("ReceiveMessage", receiveMessage);
    window.connection.on("AddContact", addContact);
    window.connection.on("UpdateContact", updateContact);
    window.connection.on("ReceiveContact", receiveContact);

    window.connection.start();
}

export const receiveMessage = (login: string, message: string) => {
    if (window.chatProxy.isChatOpen && window.chatProxy.user.Login == login) {
        drawMessage(message);
    }
    
    checkNotify(login, NotifyState.Invisible);
};

export const receiveContact = (contact: string) => {
    const data: FindedContact = JSON.parse(contact);
    console.log("Контакт найден: ", data)

    if(!data.isFind) {
        const form: HTMLFormElement | null = document.querySelector("#new-chat-modal");
        if(!form) return;
        const inputLogin: HTMLInputElement | null = form.querySelector("#login");
        if(!inputLogin) return;
        showError(inputLogin, "Пользователь не найден");
        return;
    }

    const user = data.user as User;
    const messages = data.linkedMessages as Message[];

    window.chatProxy.user = user;
    window.chatProxy.messages = messages;
    window.chatProxy.isChatOpen = true;

    clearNotification(user.Login);
}

export const addContact = (username: string, login: string, avatar: string, lastMessage: string) => {
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
};

export const updateContact = (login: string, lastMessage: string) => {
    console.log("перерисовка контактов");
    clearContacts();
    checkNotify(login, NotifyState.Invisible);

    for (const contact of window.chatProxy.contacts) {
        if (contact.recipient.Login === login) {
            contact.linkedMessage.Text = lastMessage;
            contact.linkedMessage.SendedAt = Date.now();
        }
    }

    window.chatProxy.contacts.sort((a, b) => a.linkedMessage.SendedAt - b.linkedMessage.SendedAt);

    for (const contact of window.chatProxy.contacts)
    {
        console.log("draw");
        drawContact(contact.recipient.Username, contact.recipient.Login, contact.recipient.Avatar, contact.linkedMessage.Text, contact.HasNewMessage);
    }
};