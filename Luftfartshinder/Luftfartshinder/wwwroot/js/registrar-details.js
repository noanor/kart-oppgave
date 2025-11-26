document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".obstacle-map").forEach(function (el) {
        var lat = parseFloat(el.dataset.lat);
        var lng = parseFloat(el.dataset.lng);
        var type = el.dataset.type ? el.dataset.type.toLowerCase() : '';
        var linePointsJson = el.dataset.linePoints || '';

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
        var marker = L.marker([lat, lng]).addTo(map);

        // Check if obstacle type is Powerline or Line (Linje/Luftspenn in Norwegian)
        var isLineType = type === 'powerline' || type === 'line' || 
                        type === 'luftspenn' || type === 'linje';

        if (isLineType && linePointsJson && linePointsJson.trim() !== '') {
            try {
                // Parse line points from JSON
                var linePoints = JSON.parse(linePointsJson);
                
                if (linePoints && Array.isArray(linePoints) && linePoints.length > 1) {
                    // Convert to Leaflet format [lat, lng] and validate
                    var leafletPoints = [];
                    for (var j = 0; j < linePoints.length; j++) {
                        var point = linePoints[j];
                        if (Array.isArray(point) && point.length >= 2 && 
                            typeof point[0] === 'number' && typeof point[1] === 'number') {
                            leafletPoints.push([point[0], point[1]]);
                        }
                    }
                    
                    if (leafletPoints.length > 1) {
                        // Draw the line
                        var polyline = L.polyline(leafletPoints, {
                            color: '#ff0000',
                            weight: 5,
                            opacity: 0.8,
                            lineCap: 'round',
                            lineJoin: 'round'
                        }).addTo(map);
                        
                        // Fit map to show both marker and line
                        try {
                            var group = new L.FeatureGroup([marker, polyline]);
                            map.fitBounds(group.getBounds().pad(0.1));
                        } catch (boundsErr) {
                            // If bounds calculation fails, just center on marker
                            console.warn('Could not fit bounds, centering on marker:', boundsErr);
                        }
                    }
                }
            } catch (err) {
                console.error('Error parsing line points:', err, 'JSON:', linePointsJson);
            }
        }
    });
});

