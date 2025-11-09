// ===== Leaflet setup =====
const map = L.map('map').setView([58.1630, 8.003], 13);

L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '© OpenStreetMap contributors'
}).addTo(map);

// Optional: block default right-click menu on the map container
document.getElementById('map').addEventListener('contextmenu', e => e.preventDefault());

// ===== Wheel (radial menu) =====
const wheel = document.querySelector('.wheel');
const arcs = Array.from(wheel.querySelectorAll('.arc'));

let centerX = window.innerWidth / 2;
let centerY = window.innerHeight / 2;

window.addEventListener('resize', () => {
    centerX = window.innerWidth / 2;
    centerY = window.innerHeight / 2;
});

let isOpen = false;
let justOpened = false;

function openWheel(x, y) {
    wheel.style.setProperty('--x', `${x}px`);
    wheel.style.setProperty('--y', `${y}px`);
    wheel.setAttribute('data-chosen', 0);
    wheel.classList.remove('hidden');
    setTimeout(() => wheel.classList.add('on'), 0);
    isOpen = true;
    justOpened = true;
    setTimeout(() => { justOpened = false; }, 0);
}

function closeWheel() {
    wheel.classList.remove('on');
    setTimeout(() => wheel.classList.add('hidden'), 250);
    wheel.setAttribute('data-chosen', 0);
    isOpen = false;
}

// Optional: hover highlight while open
arcs.forEach((arc, i) => {
    arc.addEventListener('mouseenter', () => { if (isOpen) wheel.setAttribute('data-chosen', i + 1); });
    arc.addEventListener('mouseleave', () => { if (isOpen) wheel.setAttribute('data-chosen', 0); });
});

// ===== Map click → set lat/lng and open wheel =====
let isPanning = false;
let justEndedPan = false;

map.on('movestart', () => { isPanning = true; });
map.on('moveend', () => {
    isPanning = false;
    // Leaflet may fire a synthetic click after a pan: ignore that one
    justEndedPan = true;
    setTimeout(() => { justEndedPan = false; }, 0);
});

let lastClick = { lat: 0, lng: 0 };   // <— use consistent property names
let marker = null;

function setLL(lat, lng) {
    // write to inputs (5–6 decimals ≈ 1–10 m precision)
    const latInput = document.getElementById('latitude');
    const lngInput = document.getElementById('longitude');
    if (latInput) latInput.value = lat.toFixed(6);
    if (lngInput) lngInput.value = lng.toFixed(6);

    // add/update a single marker
    if (marker) marker.setLatLng([lat, lng]);
    else {
        marker = L.marker([lat, lng], { draggable: true }).addTo(map);
        marker.on('dragend', (e) => {
            const p = e.target.getLatLng();
            setLL(p.lat, p.lng);
        });
    }

    const display = document.getElementById('llDisplay');
    if (display) display.textContent = `Lat ${lat.toFixed(6)}, Lon ${lng.toFixed(6)}`;

    lastClick = { lat, lng };
}

map.on('click', (e) => {
    if (isPanning || justEndedPan || isOpen) return;

    const { lat, lng } = e.latlng;
    setLL(lat, lng);

    // Always open wheel at the center of the screen
    openWheel(centerX, centerY);
});

// ===== Choosing a slice triggers the action =====
function obstacleTypeFromChoice(choice) {
    // choice is 1..N (matches your slices order)
    switch (choice) {
        case 1: return 'mast';
        case 2: return 'punkt';
        case 3: return 'linje';
        case 4: return 'luftspenn';
        case 5: return 'flate';
        default: return null;
    }
}

async function addObstacle(type, lat, lng) {
    const payload = {
        type,
        latitude: lat,
        longitude: lng
    };
    const res = await fetch('/obstacles/add-one', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    });
    console.log(payload);
    if (!res.ok) {
        const txt = await res.text();
        throw new Error(`Add failed: ${txt}`);
    }
    return res.json();
}

// Click inside the wheel → pick the slice
wheel.addEventListener('click', async (e) => {
    const arc = e.target.closest('.arc');
    if (!arc) return;

    const index = arcs.indexOf(arc) + 1; // 1..N
    wheel.setAttribute('data-chosen', index);

    const type = obstacleTypeFromChoice(index);
    const hiddenTypeInput = document.getElementById('obstacletype');
    if (hiddenTypeInput) hiddenTypeInput.value = type || '';

    closeWheel();

    if (type) {
        try {
            await addObstacle(type, lastClick.lat, lastClick.lng);
            console.log(`Added, lat: ${lastClick.lat} lng: ${lastClick.lng}`);
        } catch (err) {
            console.error(err);
            alert(err.message);
        }
    }
});

// Click anywhere outside the wheel → close it
document.addEventListener('click', (e) => {
    if (!isOpen || justOpened) return;
    if (!e.target.closest('.wheel')) closeWheel();
});

// Esc to close
document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && isOpen) closeWheel();
});
