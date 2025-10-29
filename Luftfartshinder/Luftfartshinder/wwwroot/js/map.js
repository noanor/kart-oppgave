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

function openWheel(x, y) {
  wheel.style.setProperty('--x', `${x}px`);
  wheel.style.setProperty('--y', `${y}px`);
  wheel.setAttribute('data-chosen', 0);
  wheel.classList.add('on');
  isOpen = true;
}

function closeWheel() {
  wheel.classList.remove('on');
  wheel.setAttribute('data-chosen', 0);
  isOpen = false;
}

/* Click anywhere to open (unless you clicked a slice while open) */
document.addEventListener('click', (e) => {
  const arc = e.target.closest('.arc');

  // If the wheel is open and a slice was clicked → select it.
  if (isOpen && arc) {
    const index = arcs.indexOf(arc) + 1; // 1..N
    wheel.setAttribute('data-chosen', index);

    // TODO: trigger your action here
    // handleChoice(index);

    closeWheel();
    return;
  }

  // If the wheel is open and you clicked outside → close it.
  if (isOpen && !arc && !e.target.closest('.wheel')) {
    closeWheel();
    return;
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


var popup = L.popup();

function onMapClick(e) {
    popup
        .setLatLng(e.latlng)
}

map.on('click', function (e) {
    var latitudevar = e.latlng.lat.toFixed(4);
    var longitudevar = e.latlng.lng.toFixed(4);
    let myData = `${latitudevar}, ${longitudevar}`;

    let textinput = document.getElementById("coords");
    textinput.value = myData;
});

map.on('click', onMapClick)