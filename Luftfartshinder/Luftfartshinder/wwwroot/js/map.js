var map = L.map('map').setView([58.1630, 8.003], 13);

L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '© OpenStreetMap contributors'
}).addTo(map);

// // Source: https://codepen.io/wheatup/pen/GbgyLY

// OPTIONAL: keep this if you still want to block context menus
document.getElementById('map').addEventListener('contextmenu', e => e.preventDefault());

const wheel = document.querySelector('.wheel');
const arcs  = Array.from(wheel.querySelectorAll('.arc'));
const centerX = window.innerWidth / 2;
const centerY = window.innerHeight / 2;


let isOpen = false;

let isPanning = false;
let justEndedPan = false;

map.on('movestart', () => { isPanning = true; });
map.on('moveend', () => {
    isPanning = false;
    // Leaflet may still fire a click after a pan; ignore that one
    justEndedPan = true;
    setTimeout(() => { justEndedPan = false; }, 0);
});


function openWheel(x, y) {
  wheel.style.setProperty('--x', `${x}px`);
  wheel.style.setProperty('--y', `${y}px`);
  wheel.setAttribute('data-chosen', 0);
    setTimeout(() => { wheel.classList.add('on') }, 50) ;
    wheel.classList.remove('hidden');
  isOpen = true;
}

function closeWheel() {
    wheel.classList.remove('on');
    setTimeout(() => { wheel.classList.add('hidden') }, 500);
    wheel.setAttribute('data-chosen', 0);
    isOpen = false;
}

function setLL(lat, lon) {
    // write to hidden inputs (5–6 decimals is 1–10 m precision)
    document.getElementById('latitude').value = lat.toFixed(6);
    document.getElementById('longitude').value = lon.toFixed(6);

    // add/update marker
    if (marker) marker.setLatLng([lat, lon]);
    else marker = L.marker([lat, lon], { draggable: true }).addTo(map);

    const display = document.getElementById('llDisplay');
    if (display) display.textContent = `Lat ${lat.toFixed(6)}, Lon ${lon.toFixed(6)}`;
    console.log(lat);
    console.log(lon);
}

function addMarker(lat, lon) {
    // Write to hidden inputs (updates latest clicked coords)
    document.getElementById('latitude').value = lat.toFixed(6);
    document.getElementById('longitude').value = lon.toFixed(6);

    // Create a new marker
    const m = L.marker([lat, lon], { draggable: true }).addTo(map);
    markers.push(m);

    // Update lat/lon when dragging finishes
    m.on('dragend', (e) => {
        const p = e.target.getLatLng();
        document.getElementById('latitude').value = p.lat.toFixed(6);
        document.getElementById('longitude').value = p.lng.toFixed(6);
    });

    // Optional: show coords onscreen
    const display = document.getElementById('llDisplay');
    if (display) display.textContent = `Lat ${lat.toFixed(6)}, Lon ${lon.toFixed(6)}`;
}

function handleChoice(index) {
    const choice = parseInt(wheel.getAttribute('data-chosen'), 10);
    

    console.log("User picked:", choice);
    let typeOfObstacle;

    switch (choice) {
        case 1:
            typeOfObstacle = 'mast'
            break;
        case 2:
            typeOfObstacle = 'punkt'
            break;
        case 3:
            typeOfObstacle = 'linje'
            break;
        case 4:
            typeOfObstacle = 'luftspenn'
            break;
        case 5:
            typeOfObstacle = 'flate'
            break;
    }

    document.getElementById('obstacletype').value = typeOfObstacle;
    
    
}




/* Click anywhere to open (unless you clicked a slice while open) */
document.addEventListener('click', (e) => {
  const arc = e.target.closest('.arc');

  // If the wheel is open and a slice was clicked → select it.
  if (isOpen && arc) {
    const index = arcs.indexOf(arc) + 1; // 1..N
    wheel.setAttribute('data-chosen', index);

    // TODO: trigger your action here
      handleChoice(index);

    closeWheel();
    return;
  }

  // If the wheel is open and you clicked outside → close it.
  if (isOpen && !arc && !e.target.closest('.wheel')) {
    closeWheel();
    return;
  }

const clickedInsideMap = map.getContainer().contains(e.target);
if (!isOpen) {
    if (clickedInsideMap && (isPanning || justEndedPan)) {
        // It was a drag/pan (or the synthetic click right after) → do nothing
        return;
    }
    // Otherwise, open centered as before
    openWheel(centerX, centerY);
}

  // If the wheel is closed → open it at the click position.
  if (!isOpen) {
    openWheel(centerX, centerY);
  }
});

/* Esc to close */
document.addEventListener('keydown', (e) => {
  if (e.key === 'Escape' && isOpen) closeWheel();
});

/* Optional: preview highlight on hover while open */
arcs.forEach((arc, i) => {
  arc.addEventListener('mouseenter', () => {
    if (!isOpen) return;
    wheel.setAttribute('data-chosen', i + 1);
  });
  arc.addEventListener('mouseleave', () => {
    if (!isOpen) return;
    wheel.setAttribute('data-chosen', 0);
  });
});

let marker;

map.on('click', e => setLL(e.latlng.lat, e.latlng.lng));