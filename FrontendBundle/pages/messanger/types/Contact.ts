import { Message } from "./Message";
import { User } from "./User";


export interface Contact {
    HasNewMessage: boolean,
    linkedMessage: Message,
    recipient: User,
    
}