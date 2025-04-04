import { preventDefaultFor } from "../../../event.js";
import { showError, clearError } from "../../../ui/input.js";

const registration = async (button) => {
    const form = button.closest("form");
    const formData = new FormData(form);

    const response = await fetch("/api/auth/registration", {
        method: "POST",
        body: formData,
    });

    if (response.ok) {
        location.href = "/";
    } else {
        const errors = (await response.json()).errors;
        for (const key in errors) {
            const input = form.querySelector(`[name='${key}']`);
            showError(input, errors[key]);
        }
    }
}

window.registration = registration;
window.preventDefaultFor = preventDefaultFor;
window.clearError = clearError;