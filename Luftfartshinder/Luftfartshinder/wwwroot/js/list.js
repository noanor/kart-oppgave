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
    const organizationFilter = document.getElementById('filterOrganization').value;

    const rows = document.querySelectorAll('table tbody tr');
    rows.forEach(row => {
        const username = row.cells[0].textContent.toLowerCase();
        const email = row.cells[1].textContent.toLowerCase();
        const organization = row.cells[2].textContent.trim();
        const role = row.cells[3].textContent;
        const status = row.cells[4].textContent.trim().toLowerCase();
        const statusFilterValue = statusFilter.toLowerCase();

        let matchesSearch = username.includes(searchText) || email.includes(searchText) || organization.toLowerCase().includes(searchText);
        let matchesRole = roleFilter === "" || role === roleFilter;
        let matchesStatus = statusFilter === "" || status === statusFilterValue;
        let matchesOrganization = organizationFilter === "" || organization === organizationFilter;

        row.style.display = (matchesSearch && matchesRole && matchesStatus && matchesOrganization) ? "" : "none";
    });
}


document.getElementById('userSearch').addEventListener('input', filterUsers);

document.getElementById('applyFilters').addEventListener('click', function() {
    const roleFilter = document.getElementById('filterRole').value;
    const statusFilter = document.getElementById('filterStatus').value;
    const organizationFilter = document.getElementById('filterOrganization').value;
    
    const roleFilterParam = roleFilter ? roleFilter : "All";
    const params = new URLSearchParams();
    
    params.append('roleFilter', roleFilterParam);
    
    if (statusFilter) {
        params.append('statusFilter', statusFilter);
    }
    
    if (organizationFilter) {
        params.append('organizationFilter', organizationFilter);
    }
    
    const baseUrl = window.location.pathname.split('?')[0];
    window.location.href = baseUrl + '?' + params.toString();
});

document.getElementById('resetFilters').addEventListener('click', function() {
    const baseUrl = window.location.pathname.split('?')[0];
    window.location.href = baseUrl + '?roleFilter=All';
});
