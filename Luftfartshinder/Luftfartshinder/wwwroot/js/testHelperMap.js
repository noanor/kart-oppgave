// 1) Same mapping as in your code
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

// 2) Token lookup (DOM-based, but easy to test with jsdom)
function getAntiForgeryToken(doc = document) {
    const tokenInput = doc.querySelector('#antiForgeryForm input[name="__RequestVerificationToken"]');
    if (!tokenInput) {
        console.error('Anti-Forgery token not found');
        return null;
    }
    return tokenInput.value;
}

// 3) Pure-ish function to call the server (we inject fetch + doc so we can mock them in tests)
async function addObstacle(type, lat, lng, { fetchImpl = fetch, doc = document } = {}) {
    const token = getAntiForgeryToken(doc);
    if (!token) {
        throw new Error('Anti-Forgery token is required');
    }

    const payload = {
        type: type,
        latitude: lat,
        longitude: lng
    };

    console.log('Sending payload:', JSON.stringify(payload, null, 2));

    const res = await fetchImpl('/obstacles/add-one', {
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

// Export for Jest (CommonJS style)
module.exports = {
    obstacleTypeFromChoice,
    getAntiForgeryToken,
    addObstacle
};
