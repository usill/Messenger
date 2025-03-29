import { sendMessage, findUser } from "./api.js";

export const textAreaInput = (event) => {
    if (event.key === "Enter") {
        event.preventDefault();
        sendMessage();
    }
}

export const findUserByForm = (button) => {
    const form = button.closest("form");
    const formData = new FormData(form);
    const username = formData.get("username");

    findUser(username);
}