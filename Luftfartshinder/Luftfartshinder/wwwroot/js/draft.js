document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".obstacle-map").forEach(function (el) {
        var lat = parseFloat(el.dataset.lat);
        var lng = parseFloat(el.dataset.lng);

        // Create map in this element
        var map = L.map(el, {
            zoomControl: true,
        }).setView([lat, lng], 15);

        el.classList.add("preview-map");

        // Tile layer
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; OpenStreetMap contributors'
        }).addTo(map);

        // Marker at obstacle position
        L.marker([lat, lng]).addTo(map);
    });
});