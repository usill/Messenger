import { Contact } from "./Contact"
import { Message } from "./Message"
import { User } from "./User"

export enum MessangerKeys {
    User = "user",
    Contacts = "contacts",
    Messages = "messages",
    IsChatOpen = "isChatOpen",
}

export interface Messanger {
    user: User,
    contacts: Contact[],
    messages: Message[],
    isChatOpen: boolean,
    [key: string]: any;
}