
import { Contact } from "./types/Contact";
import { Messanger, MessangerKeys } from "./types/Messanger";
import { User } from "./types/User";
import { Message } from "./types/Message";
import { setChatHeader, clearChat, openChat, closeChat, drawListMessages, drawContact, clearContacts } from "./ui";
import { closeModal } from "../../modules/modal/events";

export const initProxy = () => {
    const messanger: Messanger = {} as Messanger;

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
                    drawContact(contact.user.Username, contact.user.Login, contact.user.Avatar, contact.linkedMessage.Text, contact.hasNewMessage, contact.user.Status);
                }
            }
            if (prop === MessangerKeys.IsChatOpen) {
                value = value as boolean;
                value === true ? openChat() : closeChat();
            }

            return true;
        },
    }

    window.chatProxy = new Proxy<Messanger>(messanger, chatHandler);
};