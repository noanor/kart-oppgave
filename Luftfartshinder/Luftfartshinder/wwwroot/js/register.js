window.markValid = function(element) {
    element.classList.add("text-success");
    element.classList.remove("text-muted");
}

window.markInvalid = function(element) {
    element.classList.add("text-muted");
    element.classList.remove("text-success");
}

document.addEventListener("DOMContentLoaded", function () {
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
    const hiddenOrganization = document.getElementById("hiddenOrganization");

    function updateHiddenOrganization() {
        let value = null;
        if (roleSelect.value === "FlightCrew") {
            value = (organizationSelect.value === "Other") ? customOrganizationInput.value.trim() : organizationSelect.value;
        }
        hiddenOrganization.value = value || "";
    }

    customOrganizationInput.addEventListener("input", updateHiddenOrganization);

    function toggleOrganization() {
        if (roleSelect.value === "FlightCrew") {
            organizationDiv.style.display = "block";
            toggleCustomOrg();
        } else {
            organizationDiv.style.display = "none";
            organizationSelect.value = "";
            customOrganizationInput.style.display = "none";
        }
        checkForm()
    }

    function toggleCustomOrg() {
        if (organizationSelect.value === "Other") {
            customOrganizationInput.style.display = "block";
        } else {document
            customOrganizationInput.style.display = "none";
        }
        checkForm()
    }

    roleSelect.addEventListener("change", toggleOrganization);
    organizationSelect.addEventListener('change', function() {
        customOrganizationInput.style.display = (this.value === 'Other') ? 'block' : 'none';
        updateHiddenOrganization(); 
    });

    document.getElementById("registerForm").addEventListener("submit", function(e){
        updateHiddenOrganization(); 
        
        if (roleSelect.value === "FlightCrew" && !hiddenOrganization.value) {
            e.preventDefault();
            alert("Please enter an organization name");
            return false;
        }
    });

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
        
        const canSubmit = passwordValid && allFieldsValid && roleAndOrgValid;
        submitBtn.disabled = !canSubmit;
        submitBtn.classList.toggle("btn-dark", canSubmit);
        submitBtn.classList.toggle("btn-secondary", !canSubmit);
    }

    passwordInput.addEventListener("input", checkForm);

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

    function confirmCancel(redirectUrl) {
        if (hasUserInput()) {
            if (confirm("You have unsaved information. Are you sure you want to cancel the registration?")) {
                window.location.href = redirectUrl;
            }
        } else {
            window.location.href = redirectUrl;
        }
    }
    
    const cancelBtnUser = document.getElementById("cancelBtnUser");
    if (cancelBtnUser) {
        cancelBtnUser.addEventListener("click", function () {
            confirmCancel("/Account/Login");
        });
    }
    
    const cancelBtnAdmin = document.getElementById("cancelBtnAdmin");
    if (cancelBtnAdmin) {
        cancelBtnAdmin.addEventListener("click", function () {
            confirmCancel("/Home/SuperAdminHome");
        });
    }
});
