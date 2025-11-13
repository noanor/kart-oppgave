let formToSubmit = null;
const confirmModal = new bootstrap.Modal(document.getElementById('confirmModal'));

function autoDismissAlerts() {
    setTimeout(function () {
        const successAlert = document.getElementById('successAlert');
        const errorAlert = document.getElementById('errorAlert');

        if (successAlert) new bootstrap.Alert(successAlert).close();
        if (errorAlert) new bootstrap.Alert(errorAlert).close();
    }, 4000);
}

function getButtonStyle(actionType) {
    if (actionType === 'approve') return { text: 'Approve', class: 'btn-success' };
    if (actionType === 'delete') return { text: 'Delete', class: 'btn-danger' };
    if (actionType === 'decline') return { text: 'Decline', class: 'btn-danger' };
    return { text: 'Confirm', class: 'btn-primary' };
}

function showConfirmation(message, buttonText, buttonClass, form) {
    document.getElementById('confirmModalBody').textContent = message;
    const confirmButton = document.getElementById('confirmModalBtn');
    confirmButton.textContent = buttonText;
    confirmButton.className = 'btn ' + buttonClass;
    formToSubmit = form;
    confirmModal.show();
}

function setupConfirmationForms() {
    document.querySelectorAll('form[data-confirm]').forEach(function(form) {
        form.addEventListener('submit', function(event) {
            event.preventDefault();
            const message = form.getAttribute('data-confirm');
            const actionType = form.getAttribute('data-action-type');
            const buttonStyle = getButtonStyle(actionType);
            showConfirmation(message, buttonStyle.text, buttonStyle.class, form);
        });
    });
}

document.getElementById('confirmModalBtn').addEventListener('click', function() {
    if (formToSubmit) {
        confirmModal.hide();
        formToSubmit.submit();
    }
});

document.addEventListener('DOMContentLoaded', function() {
    autoDismissAlerts();
    setupConfirmationForms();
});

function filterUsers() {
    const searchText = document.getElementById('userSearch').value.toLowerCase();
    const roleFilter = document.getElementById('filterRole').value;
    const statusFilter = document.getElementById('filterStatus').value;

    const rows = document.querySelectorAll('table tbody tr');
    rows.forEach(row => {
        const username = row.cells[0].textContent.toLowerCase();
        const email = row.cells[1].textContent.toLowerCase();
        const organization = row.cells[2].textContent.toLowerCase();
        const role = row.cells[3].textContent;
        const status = row.cells[4].textContent.trim().toLowerCase();
        const statusFilterValue = statusFilter.toLowerCase();

        let matchesSearch = username.includes(searchText) || email.includes(searchText) || organization.includes(searchText);
        let matchesRole = roleFilter === "" || role === roleFilter;
        let matchesStatus = statusFilter === "" || status === statusFilterValue;

        row.style.display = (matchesSearch && matchesRole && matchesStatus) ? "" : "none";
    });
}


document.getElementById('userSearch').addEventListener('input', filterUsers);

document.getElementById('applyFilters').addEventListener('click', filterUsers);
document.getElementById('resetFilters').addEventListener('click', () => {
    document.getElementById('userSearch').value = "";
    document.getElementById('filterRole').value = "";
    document.getElementById('filterStatus').value = "";
    filterUsers();
});
