import { sendMessage, findUser } from "./api.js";
import { showError } from "../../ui/input.js";

export const textAreaInput = (event) => {
    if (event.key === "Enter") {
        event.preventDefault();
        sendMessage();
    }
}

export const findUserByForm = async (button) => {
    const form = button.closest("form");
    const formData = new FormData(form);
    const username = formData.get("username");

    const response = await findUser(username);

    if (response.status == 404) {
        const inputLogin = form.querySelector("#login");
        showError(inputLogin, "Пользователь не найден");
    }
}