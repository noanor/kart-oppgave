var map = L.map('map').setView([58.1630, 8.003], 13);

L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '© OpenStreetMap contributors'
}).addTo(map);

var popup = L.popup();

function onMapClick(e) {
    popup
        .setLatLng(e.latlng)
        .setContent("Du trykket på kartet på " + e.latlng.toString())
        .openOn(map);
}

map.on('click', function (e) {
    var latitudevar = e.latlng.lat.toFixed(4);
    var longitudevar = e.latlng.lng.toFixed(4);
    let myData = `${latitudevar}, ${longitudevar}`;

    let textinput = document.getElementById("coords");
    textinput.value = myData;
});

map.on('click', onMapClick)