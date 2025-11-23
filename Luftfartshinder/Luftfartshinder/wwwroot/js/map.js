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

// Line drawing state for luftspenn
let isDrawingLine = false;
let lineStartPoint = null;
let tempPolyline = null;
let lineStartMarker = null;

// Function to update preview line while drawing
function updateLinePreview(e) {
    if (!isDrawingLine || !lineStartPoint) return;
    
    const { lat, lng } = e.latlng;
    
    // Remove old preview line
    if (tempPolyline) {
        map.removeLayer(tempPolyline);
    }
    
    // Create new preview line
    tempPolyline = L.polyline([
        [lineStartPoint.lat, lineStartPoint.lng],
        [lat, lng]
    ], {
        color: '#ff0000',
        weight: 2,
        opacity: 0.5,
        dashArray: '5, 5'
    }).addTo(map);
}

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

    // If drawing a line (luftspenn), handle line drawing
    if (isDrawingLine) {
        const { lat, lng } = e.latlng;
        
        if (!lineStartPoint) {
            // First click - set start point
            lineStartPoint = { lat, lng };
            lineStartMarker = L.marker([lat, lng], { 
                icon: L.divIcon({
                    className: 'line-start-marker',
                    html: '<div style="width: 12px; height: 12px; background-color: #ff0000; border: 2px solid white; border-radius: 50%;"></div>',
                    iconSize: [12, 12],
                    iconAnchor: [6, 6]
                })
            }).addTo(map);
            
            // Show instruction
            if (toastEl) {
                toastEl.querySelector('.toast-body').textContent = 'Click again to set the end point of the line';
                toast.show();
            }
            
            // Add mouse move handler to show preview line
            map.on('mousemove', updateLinePreview);
        } else {
            // Second click - complete the line
            const endPoint = { lat, lng };
            
            // Remove temporary polyline if exists
            if (tempPolyline) {
                map.removeLayer(tempPolyline);
            }
            
            // Create the line
            const polyline = L.polyline([
                [lineStartPoint.lat, lineStartPoint.lng],
                [endPoint.lat, endPoint.lng]
            ], {
                color: '#ff0000',
                weight: 3,
                opacity: 0.8
            }).addTo(map);
            
            // Add end marker
            const endMarker = L.marker([endPoint.lat, endPoint.lng], {
                icon: L.divIcon({
                    className: 'line-end-marker',
                    html: '<div style="width: 12px; height: 12px; background-color: #ff0000; border: 2px solid white; border-radius: 50%;"></div>',
                    iconSize: [12, 12],
                    iconAnchor: [6, 6]
                })
            }).addTo(map);
            
            // Store line data
            polyline.lineStartPoint = lineStartPoint;
            polyline.lineEndPoint = endPoint;
            polyline.lineStartMarker = lineStartMarker;
            polyline.lineEndMarker = endMarker;
            
            // Save both points as obstacles
            saveLineObstacles(lineStartPoint, endPoint, polyline);
            
            // Reset line drawing state
            isDrawingLine = false;
            lineStartPoint = null;
            tempPolyline = null;
            lineStartMarker = null;
            
            // Remove mouse move handler
            map.off('mousemove', updateLinePreview);
            
            if (toastEl) {
                toastEl.querySelector('.toast-body').textContent = 'Line added to draft!';
                toast.show();
            }
        }
        return;
    }

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


const token = document.querySelector('#antiForgeryForm input[name="__RequestVerificationToken"').value;
async function addObstacle(type, lat, lng) {
    const payload = {
        type,
        latitude: lat,
        longitude: lng
    };

    const res = await fetch('/obstacles/add-one', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        },
        body: JSON.stringify(payload)
    });
    console.log(payload);
    if (!res.ok) {
        const txt = await res.text();
        throw new Error(`Add failed: ${txt}`);
    }
    return res.json();
}

async function saveLineObstacles(startPoint, endPoint, polyline) {
    try {
        // Save start point
        const startResult = await addObstacle('luftspenn', startPoint.lat, startPoint.lng);
        const startMarker = L.marker([startPoint.lat, startPoint.lng]).addTo(map);
        startMarker.draftIndex = startResult.index;
        obstacleMarkers.set(startResult.index, startMarker);
        
        // Save end point
        const endResult = await addObstacle('luftspenn', endPoint.lat, endPoint.lng);
        const endMarker = L.marker([endPoint.lat, endPoint.lng]).addTo(map);
        endMarker.draftIndex = endResult.index;
        obstacleMarkers.set(endResult.index, endMarker);
        
        // Store polyline reference with markers
        polyline.startDraftIndex = startResult.index;
        polyline.endDraftIndex = endResult.index;
        
        console.log(`Line added: start (${startPoint.lat}, ${startPoint.lng}), end (${endPoint.lat}, ${endPoint.lng})`);
    } catch (err) {
        console.error('Error saving line obstacles:', err);
        alert('Failed to save line: ' + err.message);
        // Remove the line if save failed
        if (polyline) map.removeLayer(polyline);
        if (lineStartMarker) map.removeLayer(lineStartMarker);
    }
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
        // Special handling for luftspenn - enter line drawing mode
        if (type === 'luftspenn') {
            isDrawingLine = true;
            lineStartPoint = null;
            tempPolyline = null;
            lineStartMarker = null;
            
            if (toastEl) {
                toastEl.querySelector('.toast-body').textContent = 'Click on the map to set the start point of the line';
                toast.show();
            }
            return;
        }
        
        // Normal obstacle handling for other types
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

// Esc to close wheel or cancel line drawing
document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') {
        if (isOpen) {
            closeWheel();
        } else if (isDrawingLine) {
            // Cancel line drawing
            isDrawingLine = false;
            if (tempPolyline) {
                map.removeLayer(tempPolyline);
                tempPolyline = null;
            }
            if (lineStartMarker) {
                map.removeLayer(lineStartMarker);
                lineStartMarker = null;
            }
            lineStartPoint = null;
            map.off('mousemove', updateLinePreview);
            if (toastEl) {
                toastEl.querySelector('.toast-body').textContent = 'Line drawing cancelled';
                toast.show();
            }
        }
    }
});
