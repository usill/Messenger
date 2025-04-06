import { openModal, closeModal } from "../../modules/modal/events";
import { closePreloader } from "../../modules/preloader/events";
import { stopPropagationFor, preventDefaultFor } from "../../modules/event";
import { logout, sendMessage, findContact } from "./api";
import { textAreaInput, findContactByForm } from "./events";
import { clearError } from "../../modules/UI/input";
import { initConnection } from "./connection";
import { initProxy } from "./proxy";

initConnection();
initProxy();

document.addEventListener("DOMContentLoaded", () => {
    closePreloader("main-preloader");

    window.chatProxy.contacts = window.contacts;

    window.openModal = openModal;
    window.closeModal = closeModal;
    window.stopPropagationFor = stopPropagationFor;
    window.preventDefaultFor = preventDefaultFor;
    window.logout = logout;
    window.sendMessage = sendMessage;
    window.textAreaInput = textAreaInput;
    window.findContactByForm = findContactByForm;
    window.findContact = findContact;
    window.clearError = clearError;
})