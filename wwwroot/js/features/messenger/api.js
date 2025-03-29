export const findUser = async (clickedButton) => {
    const form = clickedButton.closest("form");
    const formData = new FormData(form);
    const username = formData.get("username");

    const response = await fetch(`/api/user/find/${username}`);

    if (response.ok) {
        const findUser = await response.json();
        chatProxy.user = findUser.recipient;
        chatProxy.messages = findUser.linkedMessages;
        chatProxy.isOpen = true;
    }
}

export const sendMessage = () => {
    const textarea = document.querySelector("#chat-textarea");

    const message = textarea.value;
    const userId = JSON.stringify(chatProxy.user.Id);
    connection.invoke("SendMessage", userId, message).catch(function (error) {
        return console.error(error.toString());
    });

    drawMessage(message, true);
    textarea.value = "";
}

export const logout = async () => {
    const response = await fetch("/api/auth/logout", {
        method: "GET",
        credentials: "include"
    });

    if (response.ok) {
        location.href = "/login";
    }
}