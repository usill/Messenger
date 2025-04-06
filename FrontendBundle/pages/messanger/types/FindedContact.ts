import { Message } from "./Message";
import { User } from "./User";

export interface FindedContact {
    user: User | null,
    linkedMessages: Message[] | null,
    isFind: boolean;
}