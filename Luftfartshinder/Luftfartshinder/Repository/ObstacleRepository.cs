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
        public async Task<Obstacle> GetObstacleById(int id)
        {
            var findById = await _context.Obstacles.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (findById != null)
            {
                return findById;

            }
            else
            {
                return null;
            }
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
            _context.Obstacles.Update(obstacleData);
            await _context.SaveChangesAsync();
            return obstacleData;
        }
        public async Task<IEnumerable<Obstacle>> GetAllObstacles(Obstacle obstacleData)
        {
            var getAllData = await _context.Obstacles.Take(50).ToListAsync();
            return getAllData;
        }
    }
}