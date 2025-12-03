function getButtonStyle(actionType) {
    if (actionType === 'approve') return { text: 'Approve', class: 'btn-success' };
    if (actionType === 'delete') return { text: 'Delete', class: 'btn-danger' };
    if (actionType === 'decline') return { text: 'Decline', class: 'btn-danger' };
    return { text: 'Confirm', class: 'btn-primary' };
}

function updateUserCount(doc = document) {
    const rows = doc.querySelectorAll('table tbody tr');
    const visibleUsers = Array.from(rows).filter(row => row.style.display !== 'none').length;
    const userCountEl = doc.getElementById('userCount');
    const totalUsers = userCountEl.getAttribute('data-total');
    userCountEl.textContent = `Showing ${visibleUsers} of ${totalUsers} users`;
}

function filterUsers(doc = document) {
    const searchText = (doc.getElementById('userSearch').value || '').toLowerCase().trim();
    const roleFilter = (doc.getElementById('filterRole').value || '').trim();
    const statusFilter = (doc.getElementById('filterStatus').value || '').trim();
    const organizationFilter = (doc.getElementById('filterOrganization').value || '').trim();

    const rows = doc.querySelectorAll('table tbody tr');
    rows.forEach(row => {
        const username = (row.cells[0].textContent || '').toLowerCase().trim();
        const email = (row.cells[1].textContent || '').toLowerCase().trim();
        const organization = (row.cells[2].textContent || '').trim();
        const role = (row.cells[3].textContent || '').trim();
        const status = (row.cells[4].textContent || '').toLowerCase().trim();
        const statusFilterValue = statusFilter.toLowerCase();

        let matchesSearch = username.includes(searchText) ||
            email.includes(searchText) ||
            organization.toLowerCase().includes(searchText);

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

    updateUserCount(doc);
}

function updateFilterChips(doc = document) {
    const chipContainer = doc.getElementById('activeFilters');
    chipContainer.innerHTML = '';

    const roleFilter = doc.getElementById('filterRole').value;
    const statusFilter = doc.getElementById('filterStatus').value;
    const organizationFilter = doc.getElementById('filterOrganization').value;

    if (roleFilter && roleFilter !== "All") {
        createChip('Role', roleFilter, doc);
    }
    if (statusFilter) {
        createChip('Status', statusFilter, doc);
    }
    if (organizationFilter) {
        createChip('Organization', organizationFilter, doc);
    }
}

function createChip(type, value, doc = document) {
    const chipContainer = doc.getElementById('activeFilters');
    const chip = doc.createElement('div');
    chip.className = 'filter-chip';
    chip.dataset.type = type;

    const textNode = doc.createTextNode(value);
    chip.appendChild(textNode);

    const removeBtn = doc.createElement('span');
    removeBtn.className = 'remove-chip';
    removeBtn.textContent = '×';

    // we keep the original behaviour here, but we won't assert on navigation in tests
    removeBtn.addEventListener('click', function (e) {
        e.stopPropagation();
        if (type === 'Role') doc.getElementById('filterRole').value = '';
        if (type === 'Status') doc.getElementById('filterStatus').value = '';
        if (type === 'Organization') doc.getElementById('filterOrganization').value = '';

        const params = new URLSearchParams(window.location.search);
        if (type === 'Role') params.delete('roleFilter');
        if (type === 'Status') params.delete('statusFilter');
        if (type === 'Organization') params.delete('organizationFilter');
        window.location.href = window.location.pathname + '?' + params.toString();
    });

    chip.appendChild(removeBtn);
    chipContainer.appendChild(chip);
}

// Export for Jest
module.exports = {
    getButtonStyle,
    updateUserCount,
    filterUsers,
    updateFilterChips,
    createChip
};
