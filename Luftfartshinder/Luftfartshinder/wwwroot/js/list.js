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

function updateUserCount() {
    const rows = document.querySelectorAll('table tbody tr');
    const visibleUsers = Array.from(rows).filter(row => row.style.display !== 'none').length;
    const totalUsers = document.getElementById('userCount').getAttribute('data-total');
    document.getElementById('userCount').textContent = `Showing ${visibleUsers} of ${totalUsers} users`;
}


function filterUsers() {
    const searchText = (document.getElementById('userSearch').value || '').toLowerCase().trim();
    const roleFilter = (document.getElementById('filterRole').value || '').trim();
    const statusFilter = (document.getElementById('filterStatus').value || '').trim();
    const organizationFilter = (document.getElementById('filterOrganization').value || '').trim();

    const rows = document.querySelectorAll('table tbody tr');
    rows.forEach(row => {
        const username = (row.cells[0].textContent || '').toLowerCase().trim();
        const email = (row.cells[1].textContent || '').toLowerCase().trim();
        const organization = (row.cells[2].textContent || '').trim();
        const role = (row.cells[3].textContent || '').trim();
        const status = (row.cells[4].textContent || '').toLowerCase().trim();
        const statusFilterValue = statusFilter.toLowerCase();

        let matchesSearch = username.includes(searchText) || email.includes(searchText) || organization.toLowerCase().includes(searchText);
        let matchesRole = !roleFilter || role === roleFilter;
        let matchesStatus = !statusFilter || status === statusFilterValue;
        let matchesOrganization = false;

        if (!organizationFilter) {
            matchesOrganization = true;
        } else if (organizationFilter === "Other") {
            const knownOrgs = ["Police", "Norwegian Air Ambulance", "Avinor", "Norwegian Armed Forces"]
                .map(o => o.toLowerCase());
            const orgNameNorm = organization.toLowerCase();
            matchesOrganization = !knownOrgs.includes(orgNameNorm);
        } else {
            matchesOrganization = organization === organizationFilter;
        }

        row.style.display = (matchesSearch && matchesRole && matchesStatus && matchesOrganization) ? "" : "none";
    });

    updateUserCount();
}

document.addEventListener('DOMContentLoaded', function() {
    autoDismissAlerts();
    setupConfirmationForms();
    filterUsers(); 
});


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

function updateFilterChips() {
    const chipContainer = document.getElementById('activeFilters');
    chipContainer.innerHTML = '';

    const roleFilter = document.getElementById('filterRole').value;
    const statusFilter = document.getElementById('filterStatus').value;
    const organizationFilter = document.getElementById('filterOrganization').value;

    if (roleFilter && roleFilter !== "All") {
        createChip('Role', roleFilter);
    }
    if (statusFilter) {
        createChip('Status', statusFilter);
    }
    if (organizationFilter) {
        createChip('Organization', organizationFilter);
    }
}

function createChip(type, value) {
    const chipContainer = document.getElementById('activeFilters');
    const chip = document.createElement('div');
    chip.className = 'filter-chip';
    chip.dataset.type = type;
    
    const textNode = document.createTextNode(value);
    chip.appendChild(textNode);
    
    const removeBtn = document.createElement('span');
    removeBtn.className = 'remove-chip';
    removeBtn.textContent = '×';

    removeBtn.addEventListener('click', function(e) {
        e.stopPropagation(); 
        if (type === 'Role') document.getElementById('filterRole').value = '';
        if (type === 'Status') document.getElementById('filterStatus').value = '';
        if (type === 'Organization') document.getElementById('filterOrganization').value = '';

        const params = new URLSearchParams(window.location.search);
        if (type === 'Role') params.delete('roleFilter');
        if (type === 'Status') params.delete('statusFilter');
        if (type === 'Organization') params.delete('organizationFilter');
        window.location.href = window.location.pathname + '?' + params.toString();
    });

    chip.appendChild(removeBtn);
    chipContainer.appendChild(chip);
}

document.addEventListener('DOMContentLoaded', updateFilterChips);
document.getElementById('applyFilters').addEventListener('click', updateFilterChips);