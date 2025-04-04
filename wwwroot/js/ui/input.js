
export const clearError = (input) => {
    input.classList.remove("border-red-400");
    input.classList.add("border-gray-300");
    const errorLabel = input.nextElementSibling;
    errorLabel.textContent = "";
}

export const showError = (input, message) => {
    input.classList.remove("border-gray-300");
    input.classList.add("border-red-400");
    const errorLabel = input.nextElementSibling;
    errorLabel.textContent = message;
}