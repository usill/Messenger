import { drawMessage } from "./ui";

export const findContact = async (login: string) => {
    window.connection.invoke("FindContact", login).catch((error) => {
        return console.error(error.toString());
    });
}

export const getMessages = async (receiverId: number, index: number) => {
    window.connection.invoke("GetMessages", receiverId.toString(), index.toString()).catch((error) => {
        return console.error(error.toString());
    });
}

export const sendMessage = () => {
    const textarea: HTMLTextAreaElement | null = document.querySelector("#chat-textarea");

    if(!textarea) return;

    const message: string = textarea.value;

    if (!message) return;

    const userId = JSON.stringify(window.chatProxy.user.Id);
    window.connection.invoke("SendMessage", userId, message).catch((error) => {
        return console.error(error.toString());
    });

    drawMessage(message, true);
    textarea.value = "";
}

export const logout = async () => {
    const response = await fetch("/api/auth/logout", {
        method: "GET",
        credentials: "include"
    });

    if (response.ok) {
        location.href = "/login";
    }
}