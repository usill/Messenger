import { getMessages } from "./api";
import { checkNotify, NotifyState } from "./connection";
import { Message } from "./types/Message";
import { User, UserStatus } from "./types/User";

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

export const scrollChatToEnd = () => {
    const chat = document.querySelector("#chat-content");
    chat?.scrollTo({
        top: chat.scrollHeight
    });
}

export const openChat = () => {
    const chatSide: HTMLElement | null = document.querySelector("#chat-side");
    if(!chatSide) return;
    chatSide.style.visibility = "visible";

    scrollChatToEnd();
}

export const drawMessage = (message: string, isOwn = false, toEnd: boolean = false) => {
    const li: HTMLElement = document.createElement("li");
    const chat: HTMLElement | null = document.querySelector("#chat-content");

    if(!chat) return;

    li.classList.add("message");
    li.textContent = message;

    if (isOwn) {
        li.classList.add("message-my");
    }

    if(!toEnd) {
        chat.append(li);
        scrollChatToEnd();
    } else {
        chat.prepend(li);
    }
}

const drawObserver = () => {
    const chat: HTMLElement | null = document.querySelector("#chat-content");
    if (!chat) return;

    chat.innerHTML += `
        <li id="chat-observer" class="min-h-8 w-full flex justify-center items-center">
            <div id="chat-preloader" class="flex">
                <div role="status">
                    <svg aria-hidden="true" id="spiner" style="height: 1.5rem; width: 1.5rem" class="animate-spin fill-blue-600" viewBox="0 0 100 101" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z" fill="currentColor" />
                        <path d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z" fill="currentFill" />
                    </svg>
                </div>
            </div>
        </li>
    `;

    const observerElem: HTMLElement | null = chat.querySelector("#chat-observer");
    if (!observerElem) return;

    const observerHandler: IntersectionObserverInit = {
        root: chat,
        rootMargin: "0px",
        threshold: 0
    };

    const callback: IntersectionObserverCallback = (entries: IntersectionObserverEntry[], observer: IntersectionObserver) => {
        entries.forEach((entry: IntersectionObserverEntry) => {
            if (entry.isIntersecting) {
                observer.unobserve(entry.target);
                entry.target.remove();
                
                if(window.chatProxy.hasMoreMessages) {
                    drawMoreMessages(observer, entry.target, chat);
                }
            }
        })
    }

    const observer: IntersectionObserver = new IntersectionObserver(callback, observerHandler);
    observer.observe(observerElem);
}

const drawMoreMessages = (observer: IntersectionObserver, observerElem: Element, chat: HTMLElement) => {
    const receiverId = window.chatProxy.user.Id;
    const page = ++window.chatProxy.messagesPage;

    const scrollBottom = chat.scrollHeight - chat.scrollTop;

    if (!receiverId) return;

    getMessages(receiverId, page).then(() => {
        observer.observe(observerElem);
        chat.prepend(observerElem);
        chat.scrollTo({
            top: chat.scrollHeight - scrollBottom
        })
    });
}

export const drawListMessages = (messages: Message[], recipientId: number | string) => {
    drawObserver();
    drawMessages(messages, recipientId);
}

export const drawMessages = (messages: Message[], recipientId: number | string, toEnd: boolean = false) => {
    if(!toEnd) {
        messages.sort((a: Message, b: Message) => a.SendedAt - b.SendedAt);
    } else {
        messages.sort((a: Message, b: Message) => b.SendedAt - a.SendedAt);
    }

    for (const msg of messages) {
        drawMessage(msg.Text, msg.RecipientId == recipientId, toEnd);
    }
}

export const drawContact = (username: string, login: string, avatar: string, lastMessage: string, hasNewMessage: boolean, status: UserStatus) => {
    const contactList: HTMLElement | null = document.querySelector("#contacts-content");
    if(!contactList) return;
    
    console.log("Статус:", status);
    const colorStatus = status == UserStatus.Offline ? "bg-gray-300" : "bg-green-500";

    const newContact = `
        <li id="${login}" onclick="findContact('${login}')" class="flex items-center gap-4 px-2 py-3 relative cursor-pointer hover:bg-gray-100">
            <div class="relative">
                <img src="/img/avatar/${avatar}" alt="" class="min-w-[40px] min-h-[40px] max-w-[40px] max-h-[40px]">
                <div id="contact-status" class="absolute -right-1 -bottom-1 w-3 h-3 rounded-full ${colorStatus}"></div>
            </div>
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

    contactList.insertAdjacentHTML("afterbegin", newContact);
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
        checkNotify(login, NotifyState.Visible);
    }
} 

export const setStatusInHeader = (status: UserStatus) => {
    const statusLabel: HTMLElement | null = document.querySelector("#chat-header-status");
    if(!statusLabel) return;
    
    if(status == UserStatus.Online) {
        statusLabel.innerHTML = `<div class="w-2 h-2 bg-green-500 rounded-full"></div><span>В сети</span>`
        return;
    }
    if(status == UserStatus.Offline) {
        statusLabel.innerHTML = `<div class="w-2 h-2 bg-gray-300 rounded-full"></div><span>Не в сети</span>`
        return;
    }
}

export const setStatusInContacts = (status: UserStatus, login: string) => {
    const contactsBlock: HTMLElement | null = document.querySelector("#contacts-content");
    if(!contactsBlock) return;
    const contact: HTMLElement | null = contactsBlock.querySelector("#" + login);
    if(!contact) return;
    const statusLabel: HTMLElement = contact.querySelector("#contact-status") as HTMLElement;

    if(status == UserStatus.Online) {
        statusLabel.classList.remove("bg-gray-300");
        statusLabel.classList.add("bg-green-500");
        return;
    }
    if(status == UserStatus.Offline) {
        statusLabel.classList.remove("bg-green-500");
        statusLabel.classList.add("bg-gray-300");
        return;
    }
}