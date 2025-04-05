import * as signalR from "@microsoft/signalr";
import { openModal, closeModal } from "../../modules/modal/events";
import { closePreloader } from "../../modules/preloader/events";
import { stopPropagationFor, preventDefaultFor } from "../../modules/event";
import { setChatHeader, clearChat, openChat, closeChat, drawListMessages, drawContact, clearContacts } from "./ui";
import { logout, sendMessage, findUser } from "./api";
import { textAreaInput, findUserByForm } from "./events";
import { clearError } from "../../modules/UI/input";
import { Contact } from "./types/Contact";
import { Messanger, MessangerKeys } from "./types/Messanger";
import { User } from "./types/User";
import { Message } from "./types/Message";
import { receiveMessage, addContact, updateContact } from "./connection";

window.connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

window.connection.on("ReceiveMessage", receiveMessage);
window.connection.on("AddContact", addContact);
window.connection.on("UpdateContact", updateContact);

window.connection.start();

const chat: Messanger = {} as Messanger;

const chatHandler = {
    set(obj: Messanger, prop: string, value: User | Contact[] | Message[] | boolean) {
        obj[prop] = value;
        console.log(value);

        if (prop === MessangerKeys.User) {
            value = value as User;
            setChatHeader(value);
            closeModal("new-chat-modal");
        }
        if (prop === MessangerKeys.Messages) {
            value = value as Message[];
            clearChat();
            drawListMessages(value, obj.user.Id ?? 0);
        }
        if (prop === MessangerKeys.Contacts) {
            clearContacts();

            value = value as Contact[]
            value.sort((a: Contact, b: Contact) => a.linkedMessage.SendedAt - b.linkedMessage.SendedAt);

            for (var contact of value as Contact[]) {
                drawContact(contact.recipient.Username, contact.recipient.Login, contact.recipient.Avatar, contact.linkedMessage.Text, contact.HasNewMessage);
            }
        }
        if (prop === MessangerKeys.IsChatOpen) {
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