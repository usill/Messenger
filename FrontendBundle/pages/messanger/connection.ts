import * as signalR from "@microsoft/signalr";
import { Contact } from "./types/Contact";
import { Message } from "./types/Message";
import { User } from "./types/User";
import { clearContacts, drawContact, drawMessage } from "./ui";

export const initConnection = () => {
    window.connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

    window.connection.on("ReceiveMessage", receiveMessage);
    window.connection.on("AddContact", addContact);
    window.connection.on("UpdateContact", updateContact);

    window.connection.start();
}

export const receiveMessage = (username: string, login: string, message: string) => {
    console.log("Получено сообщение: ", username);

    if (window.chatProxy.isChatOpen && window.chatProxy.user.Login == login) {
        drawMessage(message);
    }

    if (!window.chatProxy.isChatOpen || window.chatProxy.user.Login !== login) {
        const contact: Contact | undefined = window.chatProxy.contacts.find((item: Contact) => item.recipient.Login == login);

        if(!contact) return;

        contact.HasNewMessage = true;
    }
};

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
};