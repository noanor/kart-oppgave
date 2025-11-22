// ===== Leaflet setup =====
const map = L.map('map').setView([58.1630, 8.003], 13);

L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '© OpenStreetMap contributors'
}).addTo(map);

const geoletControl = L.geolet({
    position: 'bottomleft',
    className: 'geolet-btn',
    activeClassName: 'geolet-btn-active',
    html: `
      <span class="visually-hidden">Center map on my location</span>
      <svg
        viewBox="0 0 24 24"
        width="24"
        height="24"
        aria-hidden="true"
        focusable="false"
      >
        <circle
          cx="12"
          cy="12"
          r="7"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
        />
        <circle
          cx="12"
          cy="12"
          r="3"
          fill="currentColor"
        />
        <line
          x1="12"
          y1="2"
          x2="12"
          y2="6"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
        />
        <line
          x1="12"
          y1="18"
          x2="12"
          y2="22"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
        />
        <line
          x1="2"
          y1="12"
          x2="6"
          y2="12"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
        />
        <line
          x1="18"
          y1="12"
          x2="22"
          y2="12"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
        />
      </svg>
    `
}).addTo(map);

L.DomUtil.addClass(geoletControl.getContainer(), 'geolet-container');


const draftMarkers = new Map();
const obstacleMarkers = new Map();

async function loadDraftObstacles() {
    const res = await fetch('/obstacles/draft-json');
    if (!res.ok) return;

    const obstacles = await res.json(); // array of { index, type, latitude, longitude, name }

    obstacles.forEach(o => {
        const marker = L.marker([o.latitude, o.longitude]).addTo(map);
        marker.draftIndex = o.index;
        obstacleMarkers.set(o.index, marker);
    });
}

// after map is initialized:
loadDraftObstacles();


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

let selectionMade = false;

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
    setTimeout(() => wheel.classList.add('hidden'), 300);


    if (!selectionMade && tempMarker) {
        setTimeout(function removeMarker() {
            map.removeLayer(tempMarker);
            tempMarker = null;
        }, 50)
    }

    selectionMade = false;

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

let tempMarker = null;
let obstacleMerkers = [];

function setLL(lat, lng) {
    // write to inputs (5–6 decimals ≈ 1–10 m precision)
    const latInput = document.getElementById('latitude');
    const lngInput = document.getElementById('longitude');
    if (latInput) latInput.value = lat.toFixed(6);
    if (lngInput) lngInput.value = lng.toFixed(6);

    // add/update a single marker
    if (tempMarker) tempMarker.setLatLng([lat, lng]);
    else {
        tempMarker = L.marker([lat, lng], { draggable: true }).addTo(map);
        tempMarker.on('dragend', (e) => {
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

function createDraftObstacle(type, lat, lng) {
    let draftId = 0;

    const obstacleDraft = {
        draftId,
        type,
        latitude: lat,
        longitude: lng
    }
    draftObstacles.push(obstacleDraft);

    const marker = L.marker([lat, lng]).addTo(map);
    marker.draftId = draftId;
    draftMarkers.set(draftId, marker);

    return obstacleDraft;
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
const toastEl = document.getElementById('toastObstacleAdded');
const toast = new bootstrap.Toast(toastEl);

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
            const result = await addObstacle(type, lastClick.lat, lastClick.lng);
            
            const draftIndex = result.index;

            const marker = L.marker([lastClick.lat, lastClick.lng]).addTo(map);
            marker.draftIndex = draftIndex;
            obstacleMarkers.set(draftIndex, marker);

            if (toast) {
                toast.show();
            }

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
