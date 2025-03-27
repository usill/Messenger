
document.addEventListener("DOMContentLoaded", () => {
    const registrationFormPrepare = () => {
        const form = document.querySelector("#registration-form");
        const submitButton = form.querySelector("#submit");

        submitButton.addEventListener("click", async (ev) => {
            const formData = new FormData(form);
            
            const response = await fetch("/api/auth/registration", {
                method: "POST",
                body: formData,
            });

            if(response.ok) {
                location.href = "/";
            }
        });

        form.addEventListener("submit", (ev) => {
            ev.preventDefault();
        });
    }

    registrationFormPrepare();
})