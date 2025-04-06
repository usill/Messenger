import * as signalR from "@microsoft/signalr";
import { Contact } from "./types/Contact";
import { Message } from "./types/Message";
import { User, UserStatus } from "./types/User";
import { clearContacts, clearNotification, drawContact, drawMessage, setStatusInContacts, setStatusInHeader } from "./ui";
import { FindedContact } from "./types/FindedContact";
import { showError } from "../../modules/UI/input";

export enum NotifyState {
    Visible,
    Invisible
}

export const initConnection = () => {
    window.connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

    window.connection.on("ReceiveMessage", receiveMessage);
    window.connection.on("AddContact", addContact);
    window.connection.on("UpdateContact", updateContact);
    window.connection.on("ReceiveContact", receiveContact);
    window.connection.on("UserOnline", userOnline);
    window.connection.on("UserOffline", userOffline);

    window.connection.start();
}

export const checkNotify = (login: string, state: NotifyState) => {
    const contact: Contact | undefined = window.chatProxy.contacts.find((item: Contact) => item.user.Login == login);
    if (!contact) return;

    if ((!window.chatProxy.isChatOpen || window.chatProxy.user.Login !== login) && state === NotifyState.Invisible) {
        contact.hasNewMessage = true;
    }

    if(state === NotifyState.Visible) {
        contact.hasNewMessage = false;
    }
}

const userOnline = (login: string) => {
    console.log("Подключился: ", login)
    setStatus(UserStatus.Online, login);
}

const userOffline = (login: string) => {
    console.log("Отключился: ", login)
    setStatus(UserStatus.Offline, login);
}

const setStatus = (status: UserStatus, login:string) => {
    const contact: Contact | undefined = window.chatProxy.contacts.find((c: Contact) => c.user.Login == login);
    if(!contact) return;
    contact.user.Status = status;

    setStatusInContacts(status, login)
    if(window.chatProxy.user.Login == login) {
        setStatusInHeader(status);
    }
}

const receiveMessage = (login: string, message: string) => {
    if (window.chatProxy.isChatOpen && window.chatProxy.user.Login == login) {
        drawMessage(message);
    }
    
    checkNotify(login, NotifyState.Invisible);
};

const receiveContact = (contact: string) => {
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

    setStatusInHeader(user.Status);
    setStatusInContacts(user.Status, user.Login);
    clearNotification(user.Login);
}

const addContact = (contactJson: string) => {
    const contact: Contact = JSON.parse(contactJson);
    console.log("Добавлен контакт:", contact.user.Login);

    const newContact: Contact = {
        hasNewMessage: contact.hasNewMessage,
        user: {
            Login: contact.user.Login,
            Username: contact.user.Username,
            Avatar: contact.user.Avatar,
        } as User,
        linkedMessage: {
            Text: contact.linkedMessage.Text,
            SendedAt: Date.now(),
        } as Message
    };

    window.chatProxy.contacts.push(newContact);
    drawContact(contact.user.Username, contact.user.Login, contact.user.Avatar, contact.linkedMessage.Text, true, contact.user.Status);
};

const updateContact = (login: string, lastMessage: string) => {
    console.log("перерисовка контактов");
    clearContacts();
    checkNotify(login, NotifyState.Invisible);

    for (const contact of window.chatProxy.contacts) {
        if (contact.user.Login === login) {
            contact.linkedMessage.Text = lastMessage;
            contact.linkedMessage.SendedAt = Date.now();
        }
    }

    window.chatProxy.contacts.sort((a, b) => a.linkedMessage.SendedAt - b.linkedMessage.SendedAt);

    for (const contact of window.chatProxy.contacts)
    {
        console.log("draw");
        drawContact(contact.user.Username, contact.user.Login, contact.user.Avatar, contact.linkedMessage.Text, contact.hasNewMessage, contact.user.Status);
    }
};