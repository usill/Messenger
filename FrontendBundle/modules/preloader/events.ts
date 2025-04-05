export const openPreloader = (id: string) => {
    const preloader: HTMLElement | null = document.getElementById(id);
    if (!preloader) return;

    preloader.style.visibility = "visible";
}

export const closePreloader = (id: string) => {
    const preloader: HTMLElement | null = document.getElementById(id);
    if (!preloader) return;

    preloader.style.visibility = "hidden";
}