import { sendMessage, findContact } from "./api";

export const textAreaInput = (event: KeyboardEvent) => {
    if (event.key === "Enter") {
        event.preventDefault();
        sendMessage();
    }
}

export const findContactByForm = async (button: HTMLButtonElement) => {
    const form: HTMLFormElement | null = button.closest("form");
    if(!form) return;

    const formData = new FormData(form);
    const username: string | null = formData.get("username") as string | null;
    if(!username) return;

    findContact(username);
}