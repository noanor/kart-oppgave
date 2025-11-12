setTimeout(() => {
    const alertEl = document.getElementById('logoutAlert');
    if(alertEl) new bootstrap.Alert(alertEl).close();
}, 2500);

setTimeout(() => {
    const alertEl = document.getElementById('registrationAlert');
    if(alertEl) new bootstrap.Alert(alertEl).close();
}, 3500);


document.addEventListener("DOMContentLoaded", function () {
    const passwordInput = document.getElementById("password");
    const togglePassword = document.getElementById("togglePassword");

    console.log("passwordInput:", passwordInput);
    console.log("togglePassword:", togglePassword);

    if (passwordInput && togglePassword) {
        togglePassword.addEventListener("click", function () {
            const icon = this.querySelector("i");
            if (passwordInput.type === "password") {
                passwordInput.type = "text";
                icon.classList.replace("bi-eye", "bi-eye-slash");
            } else {
                passwordInput.type = "password";
                icon.classList.replace("bi-eye-slash", "bi-eye");
            }
        });
    }
});


