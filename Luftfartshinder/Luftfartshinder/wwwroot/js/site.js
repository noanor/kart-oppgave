document.addEventListener("DOMContentLoaded", function () {
    const confirmLogoutBtn = document.getElementById("confirmLogout");

    if (confirmLogoutBtn) {
        confirmLogoutBtn.addEventListener("click", function () {
            window.location.href = '/Account/SignOut';
        });
    }
});