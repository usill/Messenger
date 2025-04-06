import { Message } from "./Message";
import { User } from "./User";


export interface Contact {
    hasNewMessage: boolean,
    linkedMessage: Message,
    user: User,
    
}