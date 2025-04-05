import { sendMessage, findUser } from "./api";
import { showError } from "../../modules/UI/input";

export const textAreaInput = (event: KeyboardEvent) => {
    if (event.key === "Enter") {
        event.preventDefault();
        sendMessage();
    }
}

export const findUserByForm = async (button: HTMLButtonElement) => {
    const form: HTMLFormElement | null = button.closest("form");

    if(!form) return;

    const formData = new FormData(form);
    const username: string | null = formData.get("username") as string | null;

    if(!username) return;

    const response = await findUser(username);

    if (response.status == 404) {
        const inputLogin: HTMLInputElement | null = form.querySelector("#login");
        if(!inputLogin) return;
        showError(inputLogin, "Пользователь не найден");
    }
}