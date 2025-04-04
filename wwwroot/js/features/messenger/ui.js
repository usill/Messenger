﻿

export const setChatHeader = (user) => {
    const header = document.querySelector("#chat-header");
    const username = header.querySelector("#chat-header-username");
    const avatar = header.querySelector("#chat-header-avatar");

    username.textContent = user.Username;
    avatar.src = "/img/avatar/" + user.Avatar;
}

export const clearChat = () => {
    const chat = document.querySelector("#chat-content");
    chat.innerHTML = "";
}

export const closeChat = () => {
    const chatSide = document.querySelector("#chat-side");
    chatSide.style.visibility = "hiddent";
}

export const openChat = () => {
    const chatSide = document.querySelector("#chat-side");
    chatSide.style.visibility = "visible";
}

export const drawMessage = (message, isOwn = false) => {
    const li = document.createElement("li");
    const chat = document.querySelector("#chat-content");

    li.classList.add("message");
    li.textContent = message;

    if (isOwn) {
        li.classList.add("message-my");
    }

    chat.appendChild(li);
}

export const drawListMessages = (messages, recipientId) => {
    for (const msg of messages) {
        drawMessage(msg.Text, msg.RecipientId == recipientId);
    }
}

export const drawContact = (username, login, avatar, lastMessage, hasNewMessage) => {
    const contactList = document.querySelector("#contacts-content");

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

    function checkNewMessage(hasNewMessage) {
        if (hasNewMessage) {
            return `<div id="notify" class="absolute right-1.5 top-1.5 bg-blue-500 w-2 h-2 rounded-full flex items-center justify-center text-white text-xs"></div>`;
        }

        return "";
    }

    contactList.innerHTML = newContact + contactList.innerHTML;
}

export const clearContacts = () => {
    const contactList = document.querySelector("#contacts-content");
    contactList.innerHTML = "";
}

export const clearNotification = (login) => {
    const contact = document.querySelector(`#${login}`);

    if (!contact) return;

    const notify = contact.querySelector("#notify");

    if (notify) {
        notify.remove();
    }
} 