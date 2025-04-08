import { Contact } from "./Contact"
import { Message } from "./Message"
import { User } from "./User"

export enum MessangerKeys {
    User = "user",
    Contacts = "contacts",
    Messages = "messages",
    IsChatOpen = "isChatOpen",
    MessagesPage = "messagesPage",
    HasMoreMessages = "hasMoreMessages"
}

export interface Messanger {
    user: User,
    contacts: Contact[],
    messages: Message[],
    isChatOpen: boolean,
    messagesPage: number,
    hasMoreMessages: boolean,
    [key: string]: any;
}