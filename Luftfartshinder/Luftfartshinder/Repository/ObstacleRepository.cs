using Luftfartshinder.DataContext;
using Luftfartshinder.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Luftfartshinder.Repository
{
    public class ObstacleRepository : IObstacleRepository
    {
        private readonly ApplicationContext _context;

        public ObstacleRepository(ApplicationContext context)
        {
            _context = context;

        }

        public async Task<Obstacle> AddObstacle(Obstacle obstacleValue)
        {
            await _context.Obstacles.AddAsync(obstacleValue);
            await _context.SaveChangesAsync();
            return obstacleValue;

        }
        public async Task<Obstacle?> GetObstacleById(int id)
        {
            var findById = await _context.Obstacles
                .Include(o => o.Report)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (findById != null)
            {
                return findById;

            }
            else
            {
                return null;
            }
        }

        public async Task<List<Obstacle>> GetAllAsync()
        {
            return await _context.Obstacles
                .Include(o => o.Report)
                .ToListAsync();
        }

        public async Task<Obstacle> DeleteById(int id)
        {
            var elementById = await _context.Obstacles.FindAsync(id);
            if (elementById != null)
            {
                _context.Obstacles.Remove(elementById);
                await _context.SaveChangesAsync();
                return elementById;
            }
            else
            {
                return null;
            }
        }
        public async Task<Obstacle> UpdateObstacle(Obstacle obstacleData)
        {
            var existing = await _context.Obstacles.FindAsync(obstacleData.Id);

            if (existing == null)
                return null;

            // Map values manually
            existing.Type = obstacleData.Type;
            existing.Name = obstacleData.Name;
            existing.Description = obstacleData.Description;
            existing.Height = obstacleData.Height;
            existing.Latitude = obstacleData.Latitude;
            existing.Longitude = obstacleData.Longitude;
            existing.ReportId = obstacleData.ReportId;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<List<Obstacle>> GetByOrgId(int id)
        {
            return await _context.Obstacles
                        .Where(o => o.OrganizationId == id)
                        .ToListAsync();
        }
    }
}