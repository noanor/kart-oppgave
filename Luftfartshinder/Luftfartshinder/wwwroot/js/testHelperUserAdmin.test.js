/**
 * @jest-environment jsdom
 */

const {
    getButtonStyle,
    updateUserCount,
    filterUsers,
    updateFilterChips,
    createChip
} = require('./testHelperUserAdmin');

describe('getButtonStyle', () => {
    test('returns approve style', () => {
        expect(getButtonStyle('approve')).toEqual({
            text: 'Approve',
            class: 'btn-success'
        });
    });

    test('returns delete style', () => {
        expect(getButtonStyle('delete')).toEqual({
            text: 'Delete',
            class: 'btn-danger'
        });
    });

    test('returns decline style', () => {
        expect(getButtonStyle('decline')).toEqual({
            text: 'Decline',
            class: 'btn-danger'
        });
    });

    test('returns default style for unknown action', () => {
        expect(getButtonStyle('something-else')).toEqual({
            text: 'Confirm',
            class: 'btn-primary'
        });
        expect(getButtonStyle(null)).toEqual({
            text: 'Confirm',
            class: 'btn-primary'
        });
    });
});

describe('updateUserCount', () => {
    beforeEach(() => {
        document.body.innerHTML = `
      <span id="userCount" data-total="5"></span>
      <table>
        <tbody>
          <tr style=""><td>user1</td></tr>
          <tr style=""><td>user2</td></tr>
          <tr style="display:none"><td>user3</td></tr>
          <tr style=""><td>user4</td></tr>
          <tr style="display:none"><td>user5</td></tr>
        </tbody>
      </table>
    `;
    });

    test('counts only visible users', () => {
        updateUserCount();

        const countEl = document.getElementById('userCount');
        expect(countEl.textContent).toBe('Showing 3 of 5 users');
    });
});

describe('filterUsers', () => {
    beforeEach(() => {
        document.body.innerHTML = `
      <input id="userSearch" value="" />
      <select id="filterRole">
        <option value="">All</option>
        <option value="Admin">Admin</option>
      </select>
      <select id="filterStatus">
        <option value="">All</option>
        <option value="Active">Active</option>
        <option value="Inactive">Inactive</option>
      </select>
      <select id="filterOrganization">
        <option value="">All</option>
        <option value="Police">Police</option>
        <option value="Norwegian Air Ambulance">Norwegian Air Ambulance</option>
        <option value="Other">Other</option>
      </select>

      <span id="userCount" data-total="3"></span>

      <table>
        <tbody>
          <tr>
            <td>alice</td>
            <td>alice@example.com</td>
            <td>Police</td>
            <td>Admin</td>
            <td>Active</td>
          </tr>
          <tr>
            <td>bob</td>
            <td>bob@example.com</td>
            <td>Norwegian Air Ambulance</td>
            <td>User</td>
            <td>Inactive</td>
          </tr>
          <tr>
            <td>charlie</td>
            <td>charlie@example.com</td>
            <td>Some Other Org</td>
            <td>User</td>
            <td>Active</td>
          </tr>
        </tbody>
      </table>
    `;
    });

    test('shows all users by default and updates count', () => {
        filterUsers();

        const rows = document.querySelectorAll('table tbody tr');
        const visible = Array.from(rows).filter(r => r.style.display !== 'none');
        expect(visible.length).toBe(3);

        const countEl = document.getElementById('userCount');
        expect(countEl.textContent).toBe('Showing 3 of 3 users');
    });

    test('filters by search text (username/email/org)', () => {
        document.getElementById('userSearch').value = 'alice';
        filterUsers();

        const rows = document.querySelectorAll('table tbody tr');
        const visibleRows = Array.from(rows).filter(r => r.style.display !== 'none');
        expect(visibleRows.length).toBe(1);
        expect(visibleRows[0].cells[0].textContent).toBe('alice');
    });

    test('filters by role', () => {
        document.getElementById('filterRole').value = 'Admin';
        filterUsers();

        const rows = document.querySelectorAll('table tbody tr');
        const visibleRows = Array.from(rows).filter(r => r.style.display !== 'none');
        expect(visibleRows.length).toBe(1);
        expect(visibleRows[0].cells[0].textContent).toBe('alice');
    });

    test('filters by status', () => {
        document.getElementById('filterStatus').value = 'Active';
        filterUsers();

        const rows = document.querySelectorAll('table tbody tr');
        const visibleNames = Array.from(rows)
            .filter(r => r.style.display !== 'none')
            .map(r => r.cells[0].textContent);

        expect(visibleNames).toEqual(['alice', 'charlie']);
    });

    test('filters by organization "Other" (not one of the known orgs)', () => {
        document.getElementById('filterOrganization').value = 'Other';
        filterUsers();

        const rows = document.querySelectorAll('table tbody tr');
        const visibleRows = Array.from(rows).filter(r => r.style.display !== 'none');
        expect(visibleRows.length).toBe(1);
        expect(visibleRows[0].cells[0].textContent).toBe('charlie'); // "Some Other Org"
    });

    test('updates userCount after filtering', () => {
        document.getElementById('filterStatus').value = 'Inactive';
        filterUsers();

        const countEl = document.getElementById('userCount');
        expect(countEl.textContent).toBe('Showing 1 of 3 users');
    });
});

describe('updateFilterChips & createChip', () => {
    beforeEach(() => {
        document.body.innerHTML = `
      <div id="activeFilters"></div>
      <select id="filterRole">
        <option value="">All</option>
        <option value="Admin" selected>Admin</option>
      </select>
      <select id="filterStatus">
        <option value="">All</option>
        <option value="Active" selected>Active</option>
      </select>
      <select id="filterOrganization">
        <option value="">All</option>
        <option value="Police" selected>Police</option>
      </select>
    `;
    });

    test('creates chips for active filters', () => {
        updateFilterChips();

        const chips = document.querySelectorAll('#activeFilters .filter-chip');
        const chipTexts = Array.from(chips).map(chip => chip.textContent.replace('×', '').trim());
        const chipTypes = Array.from(chips).map(chip => chip.dataset.type);

        expect(chips.length).toBe(3);
        // values
        expect(chipTexts).toEqual(expect.arrayContaining(['Admin', 'Active', 'Police']));
        // types
        expect(chipTypes).toEqual(expect.arrayContaining(['Role', 'Status', 'Organization']));
    });

    test('createChip appends a chip with correct content and type', () => {
        const container = document.getElementById('activeFilters');
        expect(container.children.length).toBe(0);

        createChip('Role', 'Admin');

        const chip = container.querySelector('.filter-chip');
        expect(chip).not.toBeNull();
        expect(chip.dataset.type).toBe('Role');
        expect(chip.textContent).toContain('Admin');
        expect(chip.querySelector('.remove-chip')).not.toBeNull();
    });
});
