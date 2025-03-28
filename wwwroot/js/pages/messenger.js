import { openModal, closeModal, stopPropagationFor } from "../modules/modal.js";
import { closePreloader } from "../modules/preloader.js";

document.addEventListener("DOMContentLoaded", () => {
    closePreloader("main-preloader");

    const logout = async () => {
        const response = await fetch("/api/auth/logout", {
            method: "GET",
            credentials: "include"
        });

        if (response.ok) {
            location.href = "/login";
        }
    }

    window.logout = logout;
    window.openModal = openModal;
    window.closeModal = closeModal;
    window.stopPropagationFor = stopPropagationFor;
})