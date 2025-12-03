/**
 * @jest-environment jsdom
 */

const {
    obstacleTypeFromChoice,
    getAntiForgeryToken,
    addObstacle
} = require('./testHelperMap');

describe('obstacleTypeFromChoice', () => {
    test('returns correct obstacle type for valid choices', () => {
        expect(obstacleTypeFromChoice(1)).toBe('mast');
        expect(obstacleTypeFromChoice(2)).toBe('point');
        expect(obstacleTypeFromChoice(3)).toBe('line');
        expect(obstacleTypeFromChoice(4)).toBe('powerline');
        expect(obstacleTypeFromChoice(5)).toBe('area');
    });

    test('returns null for invalid choice', () => {
        expect(obstacleTypeFromChoice(0)).toBeNull();
        expect(obstacleTypeFromChoice(999)).toBeNull();
        expect(obstacleTypeFromChoice('foo')).toBeNull();
    });
});

describe('getAntiForgeryToken', () => {
    test('returns token value when present', () => {
        document.body.innerHTML = `
      <form id="antiForgeryForm">
        <input name="__RequestVerificationToken" value="test-token-123" />
      </form>
    `;

        const token = getAntiForgeryToken();
        expect(token).toBe('test-token-123');
    });

    test('returns null when token is missing', () => {
        document.body.innerHTML = `
      <form id="antiForgeryForm">
        <!-- no token input here -->
      </form>
    `;

        const token = getAntiForgeryToken();
        expect(token).toBeNull();
    });
});

describe('addObstacle', () => {
    test('sends correct request and returns result on success', async () => {
        // Arrange DOM with token
        document.body.innerHTML = `
      <form id="antiForgeryForm">
        <input name="__RequestVerificationToken" value="abc123" />
      </form>
    `;

        const mockFetch = jest.fn().mockResolvedValue({
            ok: true,
            json: async () => ({ index: 42, type: 'mast' }),
            text: async () => ''
        });

        // Act
        const result = await addObstacle('mast', 58.1630, 8.003, { fetchImpl: mockFetch });

        // Assert
        expect(mockFetch).toHaveBeenCalledTimes(1);
        const [url, options] = mockFetch.mock.calls[0];

        expect(url).toBe('/obstacles/add-one');
        expect(options.method).toBe('POST');
        expect(options.headers['Content-Type']).toBe('application/json');
        expect(options.headers['RequestVerificationToken']).toBe('abc123');

        const payload = JSON.parse(options.body);
        expect(payload).toEqual({
            type: 'mast',
            latitude: 58.1630,
            longitude: 8.003
        });

        expect(result).toEqual({ index: 42, type: 'mast' });
    });

    test('throws if anti-forgery token is missing', async () => {
        document.body.innerHTML = `
      <form id="antiForgeryForm">
        <!-- no token input -->
      </form>
    `;

        const mockFetch = jest.fn();

        await expect(
            addObstacle('mast', 58.1630, 8.003, { fetchImpl: mockFetch })
        ).rejects.toThrow('Anti-Forgery token is required');

        expect(mockFetch).not.toHaveBeenCalled();
    });

    test('throws if server returns error', async () => {
        document.body.innerHTML = `
      <form id="antiForgeryForm">
        <input name="__RequestVerificationToken" value="abc123" />
      </form>
    `;

        const mockFetch = jest.fn().mockResolvedValue({
            ok: false,
            text: async () => 'Something went wrong'
        });

        await expect(
            addObstacle('mast', 58.1630, 8.003, { fetchImpl: mockFetch })
        ).rejects.toThrow('Add failed: Something went wrong');
    });
});
