import { preventDefaultFor } from "../../../event.js";

const sendLogin = async (button) => {
    const form = button.closest("form");
    const formData = new FormData(form);

    const response = await fetch("/api/auth/login", {
        method: "POST",
        body: formData,
    });

    if (response.ok) {
        location.href = "/";
    }
}

window.sendLogin = sendLogin;
window.preventDefaultFor = preventDefaultFor;