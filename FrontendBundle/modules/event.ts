export const stopPropagationFor = (event: Event) => {
    event.stopPropagation();
}

export const preventDefaultFor = (event: Event) => {
    event.preventDefault();
}