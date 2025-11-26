document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".obstacle-map").forEach(function (el) {
        var lat = parseFloat(el.dataset.lat);
        var lng = parseFloat(el.dataset.lng);
        var type = el.dataset.type ? el.dataset.type.toLowerCase() : '';
        var obstacleIndex = parseInt(el.dataset.index) || 0;

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

        if (isLineType) {
            // Enable drawing mode for line obstacles
            var isDrawing = false;
            var linePoints = [[lat, lng]]; // Start point at marker position
            var polyline = null;
            var savedLinePoints = null;

            // Add instruction text
            var instruction = L.control({ position: 'topright' });
            instruction.onAdd = function(map) {
                var div = L.DomUtil.create('div', 'line-drawing-instruction');
                div.style.cssText = 'background: white; padding: 10px; border-radius: 4px; font-size: 12px; box-shadow: 0 2px 8px rgba(0,0,0,0.3); z-index: 1000;';
                div.innerHTML = '<strong>Draw Line:</strong><br>Click to add points<br>Double-click map to finish<br>Double-click line to delete';
                return div;
            };
            instruction.addTo(map);

            // Load saved line points from server
            fetch('/obstacles/draft-json')
                .then(response => response.json())
                .then(data => {
                    var obstacleData = data.find(o => o.index === obstacleIndex);
                    if (obstacleData && obstacleData.linePoints && obstacleData.linePoints.length > 0) {
                        savedLinePoints = obstacleData.linePoints.map(function(point) {
                            return [point[0], point[1]];
                        });
                        // Use saved points instead of just marker
                        linePoints = savedLinePoints;
                        // Draw saved line
                        if (linePoints.length > 1) {
                            polyline = L.polyline(linePoints, {
                                color: '#ff0000',
                                weight: 5,
                                opacity: 0.8,
                                lineCap: 'round',
                                lineJoin: 'round'
                            }).addTo(map);
                            
                            // Add double-click handler to delete the line
                            polyline.on('dblclick', function(e) {
                                e.originalEvent.preventDefault();
                                e.originalEvent.stopPropagation();
                                clearLine();
                            });
                            
                            el._savedPolyline = polyline;
                            
                            // Update instruction
                            var instructionDiv = instruction.getContainer();
                            if (instructionDiv) {
                                instructionDiv.innerHTML = '<strong>Line loaded:</strong><br>' + linePoints.length + ' points<br>Click to add more<br>Double-click line to delete';
                            }
                        }
                    }
                })
                .catch(err => console.error('Error loading line points:', err));

            // Function to save line points to server
            function saveLinePoints() {
                // Convert to format expected by server (array of [lat, lng])
                var pointsToSave = linePoints.length > 1 ? linePoints.map(function(point) {
                    return [point[0], point[1]];
                }) : null;

                fetch('/obstacles/save-line-points', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                    },
                    body: JSON.stringify({
                        index: obstacleIndex,
                        points: pointsToSave
                    })
                })
                .then(response => response.json())
                .then(data => {
                    console.log('Line points saved:', data);
                })
                .catch(err => console.error('Error saving line points:', err));
            }

            // Function to clear/delete the line
            function clearLine() {
                // Remove polyline from map
                if (polyline) {
                    map.removeLayer(polyline);
                    polyline = null;
                }
                
                // Reset line points to just marker position
                linePoints = [[lat, lng]];
                isDrawing = false;
                savedLinePoints = null;
                
                // Update instruction
                var instructionDiv = instruction.getContainer();
                if (instructionDiv) {
                    instructionDiv.innerHTML = '<strong>Line deleted</strong><br>Click to start drawing again';
                }
                
                // Save empty line points to server (this will remove them from session)
                saveLinePoints();
            }

            // Click handler to add points to the line
            map.on('click', function(e) {
                // If we have saved points, continue from there, otherwise start from marker
                if (!isDrawing && (!savedLinePoints || savedLinePoints.length === 0)) {
                    // Start drawing - first click starts from marker
                    isDrawing = true;
                    linePoints = [[lat, lng]]; // Start from marker position
                } else if (!isDrawing && savedLinePoints && savedLinePoints.length > 0) {
                    // Continue from saved points
                    isDrawing = true;
                    // linePoints already contains saved points
                }
                
                // Add new point (skip if it's the same as last point)
                var newPoint = [e.latlng.lat, e.latlng.lng];
                if (linePoints.length > 0) {
                    var lastPoint = linePoints[linePoints.length - 1];
                    if (Math.abs(lastPoint[0] - newPoint[0]) < 0.0001 && 
                        Math.abs(lastPoint[1] - newPoint[1]) < 0.0001) {
                        // Skip if clicking on same point
                        return;
                    }
                }
                
                linePoints.push(newPoint);
                
                // Remove existing polyline if it exists
                if (polyline) {
                    map.removeLayer(polyline);
                }
                
                // Create new polyline with all points
                if (linePoints.length > 1) {
                    // Remove existing polyline if it exists
                    if (polyline) {
                        map.removeLayer(polyline);
                    }
                    
                    polyline = L.polyline(linePoints, {
                        color: '#ff0000',
                        weight: 5,
                        opacity: 0.8,
                        lineCap: 'round',
                        lineJoin: 'round'
                    }).addTo(map);
                    
                    // Add double-click handler to delete the line
                    polyline.on('dblclick', function(e) {
                        e.originalEvent.preventDefault();
                        e.originalEvent.stopPropagation();
                        clearLine();
                    });
                }

                // Save to server
                saveLinePoints();
            });

            // Double-click on map to finish drawing (but not on the line itself)
            map.on('dblclick', function(e) {
                // Check if the click was on the polyline
                if (polyline && L.DomUtil.hasClass(e.originalEvent.target, 'leaflet-interactive')) {
                    // Let the polyline handle its own double-click
                    return;
                }
                
                e.originalEvent.preventDefault();
                e.originalEvent.stopPropagation();
                if (isDrawing && linePoints.length > 1) {
                    isDrawing = false;
                    // Update instruction
                    var instructionDiv = instruction.getContainer();
                    if (instructionDiv) {
                        instructionDiv.innerHTML = '<strong>Line drawn:</strong><br>' + linePoints.length + ' points<br>Double-click line to delete';
                    }
                    // Final save
                    saveLinePoints();
                }
            });

            // Store reference to allow clearing
            el._lineDrawing = {
                map: map,
                polyline: polyline,
                linePoints: linePoints,
                isDrawing: isDrawing,
                instruction: instruction,
                obstacleIndex: obstacleIndex
            };
        }
    });
});