import { Message } from "./types/Message";
import { User } from "./types/User";

export const setChatHeader = (user: User) => {
    const header: HTMLElement | null = document.querySelector("#chat-header");
    if(!header) return;
    
    const username: HTMLElement | null = header.querySelector("#chat-header-username");
    const avatar: HTMLImageElement | null = header.querySelector("#chat-header-avatar");

    if(!username || !avatar) return;

    username.textContent = user.Username;
    avatar.src = "/img/avatar/" + user.Avatar;
}

export const clearChat = () => {
    const chat: HTMLElement | null = document.querySelector("#chat-content");
    if(!chat) return;

    chat.innerHTML = "";
}

export const closeChat = () => {
    const chatSide: HTMLElement | null = document.querySelector("#chat-side");
    if(!chatSide) return;
    chatSide.style.visibility = "hiddent";
}

export const openChat = () => {
    const chatSide: HTMLElement | null = document.querySelector("#chat-side");
    if(!chatSide) return;
    chatSide.style.visibility = "visible";
}

export const drawMessage = (message: string, isOwn = false) => {
    const li: HTMLElement = document.createElement("li");
    const chat: HTMLElement | null = document.querySelector("#chat-content");

    if(!chat) return;

    li.classList.add("message");
    li.textContent = message;

    if (isOwn) {
        li.classList.add("message-my");
    }

    chat.appendChild(li);
}

export const drawListMessages = (messages: Message[], recipientId: number | string) => {
    for (const msg of messages) {
        drawMessage(msg.Text, msg.RecipientId == recipientId);
    }
}

export const drawContact = (username: string, login: string, avatar: string, lastMessage: string, hasNewMessage: boolean) => {
    const contactList: HTMLElement | null = document.querySelector("#contacts-content");
    if(!contactList) return;

    const newContact = `
        <li id="${login}" onclick="findUser('${login}')" class="flex items-center gap-4 px-2 py-3 relative cursor-pointer hover:bg-gray-100">
            <img src="/img/avatar/${avatar}" alt="" class="w-10 h-10">
            <div class="flex flex-col overflow-hidden">
                <span class="text-sm font-medium leading-4 whitespace-nowrap">${username}</span>
                <span class="text-sm leading-4 w-max text-gray-500 whitespace-nowrap">${lastMessage}</span>
            </div>
            ${checkNewMessage(hasNewMessage)}
        </li>
    `;

    function checkNewMessage(hasNewMessage: boolean) {
        if (hasNewMessage) {
            return `<div id="notify" class="absolute right-1.5 top-1.5 bg-blue-500 w-2 h-2 rounded-full flex items-center justify-center text-white text-xs"></div>`;
        }

        return "";
    }

    contactList.innerHTML = newContact + contactList.innerHTML;
}

export const clearContacts = () => {
    const contactList: HTMLElement | null = document.querySelector("#contacts-content");
    if(!contactList) return;
    contactList.innerHTML = "";
}

export const clearNotification = (login: string) => {
    const contact: HTMLElement | null = document.querySelector(`#${login}`);

    if (!contact) return;

    const notify: HTMLElement | null = contact.querySelector("#notify");

    if (notify) {
        notify.remove();
    }
} 