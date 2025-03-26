

export const openPreloader = (id) => {
    const preloader = document.getElementById(id);
    if(!preloader) return;

    preloader.style.visibility = "visible";
}

export const closePreloader = (id) => {
    const preloader = document.getElementById(id);
    if(!preloader) return;

    preloader.style.visibility = "hidden";
}