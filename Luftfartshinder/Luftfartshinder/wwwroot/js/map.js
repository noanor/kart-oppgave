// ===== KART INITIALISERING =====
const map = L.map('map').setView([58.1630, 8.003], 13);

L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '© OpenStreetMap contributors'
}).addTo(map);

// Geolokasjonskontroll for å sentrere kartet på brukerens posisjon
const geoletControl = L.geolet({
    position: 'bottomleft',
    className: 'geolet-btn',
    activeClassName: 'geolet-btn-active',
    html: `
      <span class="visually-hidden">Center map on my location</span>
      <svg viewBox="0 0 24 24" width="24" height="24" aria-hidden="true" focusable="false">
        <circle cx="12" cy="12" r="7" fill="none" stroke="currentColor" stroke-width="2" />
        <circle cx="12" cy="12" r="3" fill="currentColor" />
        <line x1="12" y1="2" x2="12" y2="6" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
        <line x1="12" y1="18" x2="12" y2="22" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
        <line x1="2" y1="12" x2="6" y2="12" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
        <line x1="18" y1="12" x2="22" y2="12" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
      </svg>
    `
}).addTo(map);

L.DomUtil.addClass(geoletControl.getContainer(), 'geolet-container');

// ===== DATA LAGRING =====
// Map for å lagre alle obstacle-markører, nøkkel er draft-index
const obstacleMarkers = new Map();

// ===== LASTE EKSISTERENDE DRAFT OBSTACLES =====
// Henter alle draft obstacles fra serveren og viser dem på kartet ved oppstart
async function loadDraftObstacles() {
    try {
        const res = await fetch('/obstacles/draft-json');
        if (!res.ok) {
            console.warn('Failed to load draft obstacles:', res.status);
            return;
        }

        const obstacles = await res.json();
        console.log(`Loading ${obstacles.length} draft obstacles...`);

        obstacles.forEach((o, idx) => {
            try {
                console.log(`Loading obstacle ${idx + 1}/${obstacles.length}:`, {
                    index: o.index,
                    type: o.type,
                    lat: o.latitude,
                    lng: o.longitude
                });
                
                const marker = L.marker([o.latitude, o.longitude]).addTo(map);
                marker.draftIndex = o.index;
                obstacleMarkers.set(o.index, marker);
                console.log(`Obstacle ${o.index} loaded successfully`);
            } catch (err) {
                console.error(`Error loading obstacle ${o.index}:`, err);
            }
        });
        
        console.log(`Successfully loaded ${obstacleMarkers.size} obstacles`);
    } catch (err) {
        console.error('Error in loadDraftObstacles:', err);
    }
}

map.whenReady(() => {
    console.log('Map is ready, loading draft obstacles...');
    loadDraftObstacles();
});

document.getElementById('map').addEventListener('contextmenu', e => e.preventDefault());

// ===== RADIAL MENY (WHEEL) SETUP =====
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

// Åpner radial menyen på gitt posisjon
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

// Lukker radial menyen og fjerner midlertidig markør hvis ingen valg ble gjort
function closeWheel() {
    wheel.classList.remove('on');
    setTimeout(() => wheel.classList.add('hidden'), 300);

    if (!selectionMade && tempMarker) {
        setTimeout(() => {
            if (tempMarker && map.hasLayer(tempMarker)) {
                map.removeLayer(tempMarker);
            }
            tempMarker = null;
        }, 50);
    }

    selectionMade = false;
    wheel.setAttribute('data-chosen', 0);
    isOpen = false;
}

// Hover-effekt på segmenter i radial menyen
arcs.forEach((arc, i) => {
    arc.addEventListener('mouseenter', () => { if (isOpen) wheel.setAttribute('data-chosen', i + 1); });
    arc.addEventListener('mouseleave', () => { if (isOpen) wheel.setAttribute('data-chosen', 0); });
});

// ===== KART KLIKK-HÅNDTERING =====
// Sporer panning for å unngå uønskede klikk etter at kartet er flyttet
let isPanning = false;
let justEndedPan = false;

map.on('movestart', () => { isPanning = true; });
map.on('moveend', () => {
    isPanning = false;
    // Ignorerer syntetisk klikk som Leaflet sender etter panning
    justEndedPan = true;
    setTimeout(() => { justEndedPan = false; }, 0);
});

let lastClick = { lat: 0, lng: 0 };
let tempMarker = null;

// Setter koordinater og oppretter draggable markør
function setLL(lat, lng) {
    const latInput = document.getElementById('latitude');
    const lngInput = document.getElementById('longitude');
    if (latInput) latInput.value = lat.toFixed(6);
    if (lngInput) lngInput.value = lng.toFixed(6);

    if (tempMarker && map.hasLayer(tempMarker)) {
        map.removeLayer(tempMarker);
    }

    tempMarker = L.marker([lat, lng], { draggable: true }).addTo(map);
    tempMarker.on('dragend', (e) => {
        const p = e.target.getLatLng();
        setLL(p.lat, p.lng);
    });

    const display = document.getElementById('llDisplay');
    if (display) display.textContent = `Lat ${lat.toFixed(6)}, Lon ${lng.toFixed(6)}`;

    lastClick = { lat, lng };
}

// Ved klikk på kartet: sett koordinater og åpne radial meny
map.on('click', async (e) => {
    if (isPanning || justEndedPan || isOpen) return;

    const { lat, lng } = e.latlng;
    setLL(lat, lng);
    openWheel(centerX, centerY);
});

// ===== OBSTACLE TYPE MAPPING =====
// Konverterer segment-nummer (1-5) til obstacle-type
function obstacleTypeFromChoice(choice) {
    switch (choice) {
        case 1: return 'mast';
        case 2: return 'point';
        case 3: return 'line';
        case 4: return 'powerline';
        case 5: return 'area';
        default: return null;
    }
}

// ===== OBSTACLE HÅNDTERING =====
// Henter Anti-Forgery token fra skjemaet
function getAntiForgeryToken() {
    const tokenInput = document.querySelector('#antiForgeryForm input[name="__RequestVerificationToken"]');
    if (!tokenInput) {
        console.error('Anti-Forgery token not found');
        return null;
    }
    return tokenInput.value;
}

// Sender obstacle til serveren og lagrer i draft
async function addObstacle(type, lat, lng) {
    const token = getAntiForgeryToken();
    if (!token) {
        throw new Error('Anti-Forgery token is required');
    }

    const payload = {
        type: type,
        latitude: lat,
        longitude: lng
    };

    console.log('Sending payload:', JSON.stringify(payload, null, 2));
    
    const res = await fetch('/obstacles/add-one', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        },
        body: JSON.stringify(payload)
    });
    
    if (!res.ok) {
        const txt = await res.text();
        console.error('Failed to add obstacle:', txt);
        throw new Error(`Add failed: ${txt}`);
    }
    
    const result = await res.json();
    console.log('Obstacle added successfully:', result);
    return result;
}

const toastEl = document.getElementById('toastObstacleAdded');
const toast = new bootstrap.Toast(toastEl);

// ===== RADIAL MENY KLIKK-HÅNDTERING =====
// Ved klikk i radial meny: velg type og legg til obstacle
wheel.addEventListener('click', async (e) => {
    const arc = e.target.closest('.arc');
    if (!arc) return;

    const index = arcs.indexOf(arc) + 1;
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
                toastEl.querySelector('.toast-body').textContent = 'Obstacle added to draft!';
                toast.show();
            }

            console.log(`Added, lat: ${lastClick.lat} lng: ${lastClick.lng}`);
        } catch (err) {
            console.error(err);
            alert(err.message);
        }
    }
});

// Lukker radial meny ved klikk utenfor eller Escape-tast
document.addEventListener('click', (e) => {
    if (!isOpen || justOpened) return;
    if (!e.target.closest('.wheel')) closeWheel();
});

document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && isOpen) {
        closeWheel();
    }
});