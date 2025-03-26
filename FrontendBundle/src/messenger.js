import "./style.css";
import {openModal, closeModal, stopPropagationFor} from "./modal.js";
import { closePreloader } from "./preloader.js";

window.openModal = openModal;
window.closeModal = closeModal;
window.stopPropagationFor = stopPropagationFor;

document.addEventListener('DOMContentLoaded', () => {
    closePreloader("main-preloader");
})
