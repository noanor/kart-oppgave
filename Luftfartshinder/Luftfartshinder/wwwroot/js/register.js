window.markValid = function(element) {
    element.classList.add("text-success");
    element.classList.remove("text-muted");
}

window.markInvalid = function(element) {
    element.classList.add("text-muted");
    element.classList.remove("text-success");
}

document.addEventListener("DOMContentLoaded", function () {
    const firstNameInput = document.getElementById("FirstName");
    if (firstNameInput) {
        firstNameInput.focus();
    }
    
    const passwordInput = document.getElementById("password");
    const submitBtn = document.getElementById("submitBtn");

    const togglePassword = document.getElementById("togglePassword");
    if (togglePassword && passwordInput) {
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

    const confirmPasswordInput = document.getElementById("ConfirmPassword");
    const toggleConfirmPassword = document.getElementById("toggleConfirmPassword");

    if (toggleConfirmPassword && confirmPasswordInput) {
        toggleConfirmPassword.addEventListener("click", function () {
            const icon = this.querySelector("i");

            if (confirmPasswordInput.type === "password") {
                confirmPasswordInput.type = "text";
                icon.classList.replace("bi-eye", "bi-eye-slash");
            } else {
                confirmPasswordInput.type = "password";
                icon.classList.replace("bi-eye-slash", "bi-eye");
            }
        });
    }

    const requirements = {
        length: document.getElementById("length"),
        uppercase: document.getElementById("uppercase"),
        lowercase: document.getElementById("lowercase"),
        number: document.getElementById("number"),
        special: document.getElementById("special")
    };
    
    const roleSelect = document.getElementById("role");
    const organizationDiv = document.getElementById("organizationDiv");
    const organizationSelect = document.getElementById("organization");
    const customOrganizationInput = document.getElementById("customOrganization");

    // Only run this code if all elements exist
    if (roleSelect && organizationDiv && organizationSelect && customOrganizationInput) {

        function toggleOrganization() {
            if (roleSelect.value === "FlightCrew") {
                organizationDiv.style.display = "block";
                toggleCustomOrg();
            } else {
                organizationDiv.style.display = "none";
                organizationSelect.value = "";
                customOrganizationInput.style.display = "none";
                customOrganizationInput.value = "";
            }
            checkForm();
        }

        function toggleCustomOrg() {
            if (organizationSelect.value === "Other") {
                customOrganizationInput.style.display = "block";
            } else {
                customOrganizationInput.style.display = "none";
                customOrganizationInput.value = "";
            }
            checkForm();
        }

        roleSelect.addEventListener("change", toggleOrganization);

        organizationSelect.addEventListener("change", function () {
            toggleCustomOrg();
            checkForm();
        });

        const registerForm = document.getElementById("registerForm");
        if (registerForm) {
            registerForm.addEventListener("submit", function (e) {
                if (roleSelect.value === "FlightCrew") {
                    const orgValue = organizationSelect.value;
                    if (!orgValue || (orgValue === "Other" && !customOrganizationInput.value.trim())) {
                        e.preventDefault();
                        alert("Please enter an organization name");
                        return false;
                    }
                }
            });
        }
    }


    window.checkForm = function() {
        const value = passwordInput.value;
        let passwordValid = true;

        if (value.length >= 8) markValid(requirements.length); else { markInvalid(requirements.length); passwordValid = false; }
        if (/[A-Z]/.test(value)) markValid(requirements.uppercase); else { markInvalid(requirements.uppercase); passwordValid = false; }
        if (/[a-z]/.test(value)) markValid(requirements.lowercase); else { markInvalid(requirements.lowercase); passwordValid = false; }
        if (/\d/.test(value)) markValid(requirements.number); else { markInvalid(requirements.number); passwordValid = false; }
        if (/[!#$%^&*(),.?":{}|<>]/.test(value)) markValid(requirements.special); else { markInvalid(requirements.special); passwordValid = false; }

        
        const form = document.getElementById("registerForm");
        const allFieldsValid = form.checkValidity();

        let roleAndOrgValid = false;
        if (roleSelect.value === "Registrar") {
            roleAndOrgValid = true;
        } else if (roleSelect.value === "FlightCrew") {
            if (organizationSelect.value) {
                if (organizationSelect.value === "Other") {
                    if (customOrganizationInput.value.trim() !== "") {
                        roleAndOrgValid = true;
                    }
                } else {
                    roleAndOrgValid = true;
                }
            }
        }

        let confirmMatches = false;
        if (confirmPasswordInput) {
            confirmMatches = confirmPasswordInput.value === passwordInput.value;

            if (confirmMatches) {
                confirmPasswordInput.classList.remove("is-invalid");
            } else {
                confirmPasswordInput.classList.add("is-invalid");
            }
        }

        const canSubmit = passwordValid && allFieldsValid && roleAndOrgValid && (confirmPasswordInput ? confirmMatches : true);

        submitBtn.disabled = !canSubmit;
        submitBtn.classList.toggle("btn-dark", canSubmit);
        submitBtn.classList.toggle("btn-secondary", !canSubmit);
    }

    passwordInput.addEventListener("input", checkForm);
    if (confirmPasswordInput) {
        confirmPasswordInput.addEventListener("input", checkForm);
    }

    customOrganizationInput.addEventListener("input", checkForm);
    customOrganizationInput.addEventListener("change", checkForm);
    
    const inputs = document.querySelectorAll("#registerForm input, #registerForm select");
    inputs.forEach(input => {
        input.addEventListener("input", checkForm);
        input.addEventListener("change", checkForm);
    });
    
    checkForm();

    function hasUserInput() {
        const inputs = document.querySelectorAll("#registerForm input, #registerForm select");
        for (const input of inputs) {
            if (input.type !== "hidden" && input.value.trim() !== "") {
                return true;
            }
        }
        return false;
    }
    
    const cancelBtnUser = document.getElementById("cancelBtnUser");
    if (cancelBtnUser) {
        cancelBtnUser.addEventListener("click", function () {
            if (hasUserInput()) { 
                var cancelModal = new bootstrap.Modal(document.getElementById('cancelModal'));
                cancelModal.show();
                document.getElementById("confirmCancelBtn").dataset.redirect = "/Account/Login";
            } else {
                window.location.href = "/Account/Login"; 
            }
        });
    }


    const cancelBtnAdmin = document.getElementById("cancelBtnAdmin");
    if (cancelBtnAdmin) {
        cancelBtnAdmin.addEventListener("click", function () {
            if (hasUserInput()) { 
                var cancelModal = new bootstrap.Modal(document.getElementById('cancelModal'));
                cancelModal.show();
                document.getElementById("confirmCancelBtn").dataset.redirect = "/Account/Dashboard";
            } else {
                window.location.href = "/Account/Dashboard"; 
            }
        });
    }

    document.getElementById("confirmCancelBtn").addEventListener("click", function () {
        var url = this.dataset.redirect;
        window.location.href = url;
    });
    
    const registerAlert = document.getElementById("registerAlert");
    if (registerAlert) {
        setTimeout(() => {
            registerAlert.style.transition = "opacity 0.5s";
            registerAlert.style.opacity = '0';
            setTimeout(() => registerAlert.remove(), 500);
        }, 6000);
    }
});
