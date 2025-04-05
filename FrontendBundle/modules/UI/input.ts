
export const clearError = (input: Element) => {
    input.classList.remove("border-red-400");
    input.classList.add("border-gray-300");
    const errorLabel: Element | null = input.nextElementSibling;

    if(!errorLabel) return;

    errorLabel.textContent = "";
}

export const showError = (input: Element, message: string) => {
    input.classList.remove("border-gray-300");
    input.classList.add("border-red-400");
    const errorLabel: Element | null = input.nextElementSibling;

    if(!errorLabel) return;

    errorLabel.textContent = message;
}