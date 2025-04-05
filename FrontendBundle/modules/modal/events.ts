enum ModalState {
    Visible = "visible",
    Invisible = "hidden",
    TransitionDuration = "duration-300",
    ClosedPosition = "translateY(80px)",
    OpenedPosition = "translateY(0px)",
    OpacityZero = "0",
    OpacityOne = "1",
}

export const openModal = (id: string) => {
    const modal = document.getElementById(id);
    if (!modal) return;

    const modalContent: HTMLElement | null = modal.firstElementChild as HTMLElement | null;

    if(!modalContent) return;

    if (!modalContent.classList.contains(ModalState.TransitionDuration)) {
        modalContent.classList.add(ModalState.TransitionDuration);
    }

    modal.style.visibility = ModalState.Visible;
    modalContent.style.transform = ModalState.OpenedPosition;
    modalContent.style.opacity = ModalState.OpacityOne;
}

export const closeModal = (id: string) => {
    const modal: HTMLElement | null = document.getElementById(id);
    if (!modal) return;

    modal.style.visibility = ModalState.Invisible;

    const modalWindow: HTMLElement | null = modal.firstElementChild as HTMLElement | null;

    if(!modalWindow) return;

    modalWindow.style.transform = ModalState.ClosedPosition;
    modalWindow.style.opacity = ModalState.OpacityZero;
}