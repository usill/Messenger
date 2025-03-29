

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