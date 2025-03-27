document.addEventListener("DOMContentLoaded", () => {
    const loginFormPrepare = () => {
        const form = document.querySelector("#login-form");
        const submitButton = form.querySelector("#submit");
    
        submitButton.addEventListener("click", async (ev) => {
            const formData = new FormData(form);
            
            const response = await fetch("/api/auth/login", {
                method: "POST",
                body: formData,
            });

            if(response.ok) {
                location.href = "/";
            }
        })
    
        form.addEventListener("submit", (ev) => {
            ev.preventDefault();
        })
    }

    loginFormPrepare();
})