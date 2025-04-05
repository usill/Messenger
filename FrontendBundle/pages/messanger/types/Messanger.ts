import { Contact } from "./Contact"
import { Message } from "./Message"
import { User } from "./User"

export interface Messanger {
    user: User,
    contacts: Contact[],
    messages: Message[],
    isChatOpen: boolean,
    [key: string]: any;
}