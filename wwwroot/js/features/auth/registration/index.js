import { preventDefaultFor } from "../../../event.js";

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
            input.classList.remove("border-gray-300");
            input.classList.add("border-red-400");
            const errorLabel = input.nextElementSibling;
            errorLabel.textContent = errors[key];
        }
    }
}

window.registration = registration;
window.preventDefaultFor = preventDefaultFor;