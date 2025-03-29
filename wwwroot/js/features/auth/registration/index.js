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
    }
}

window.registration = registration;
window.preventDefaultFor = preventDefaultFor;