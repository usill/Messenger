

export const openModal = (id) => {
    const modal = document.getElementById(id);
    if(!modal) return;

    const modalContent = modal.firstElementChild;

    if(!modalContent.classList.contains("duration-300")) {
        modalContent.classList.add("duration-300");
    }

    modal.style.visibility = "visible";
    modalContent.style.transform = "translateY(0px)";
    modalContent.style.opacity = "1";
}

export const closeModal = (id) => {
    const modal = document.getElementById(id);
    if(!modal) return;

    modal.style.visibility = "hidden";
    modal.firstElementChild.style.transform = "translateY(80px)";
    modal.firstElementChild.style.opacity = "0";
}