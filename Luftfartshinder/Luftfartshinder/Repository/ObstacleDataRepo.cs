using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Luftfartshinder.DataContext;
using Luftfartshinder.Models;

namespace Luftfartshinder.Repository
{
    public class ObstacleDataRepo : IDataRepocs
    {
        private readonly ApplicationContext _context;
        
        public ObstacleDataRepo(ApplicationContext context)
        {
            _context = context;

        }

        public async Task <ObstacleData> AddObstacle(ObstacleData obstacleValue)
        {
            await _context.Obstacles.AddAsync(obstacleValue);
            await _context.SaveChangesAsync();
            return obstacleValue;

        }
        public async Task<ObstacleData> GetObstacleById(int id)
        {
            var findById =  await _context.Obstacles.Where(x => x.ObstacleID == id).FirstOrDefaultAsync();
            if(findById != null)
            {
                return findById;

            }
            else
            {
                return null;
            }
        }
        public async Task<ObstacleData> DeleteById(int id)
        {
            var elementById = await _context.Obstacles.FindAsync(id);
            if(elementById != null)
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
        public async Task<ObstacleData> UpdateObstacle(ObstacleData obstacleData)
        {
            _context.Obstacles.Update(obstacleData);
            await _context.SaveChangesAsync();
            return obstacleData;
        }
        public async Task<IEnumerable<ObstacleData>> GetAllObstacles(ObstacleData obstacleData)
        {
            var getAllData = await _context.Obstacles.Take(50).ToListAsync();
            return getAllData;
        }
    }
}
