import { preventDefaultFor } from "../../modules/event";
import { showError, clearError } from "../../modules/UI/input";

const sendLogin = async (button: HTMLButtonElement) => {
    const form: HTMLFormElement | null = button.closest("form");
    if(!form) return;

    const formData = new FormData(form);

    const response = await fetch("/api/auth/login", {
        method: "POST",
        body: formData,
    });

    if (response.ok) {
        location.href = "/";
    } else {
        const errors = (await response.json()).errors;
        for (const key in errors) {
            const input: HTMLInputElement | null = form.querySelector(`[name='${key}']`);
            if(!input) return;

            showError(input, errors[key]);
        }
    }
}

window.sendLogin = sendLogin;
window.preventDefaultFor = preventDefaultFor;
window.clearError = clearError;