using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.Repository
{
    public interface IObstacleRepository
    {
        Task<Obstacle> AddObstacle(Obstacle obstacleData);

        Task<Obstacle> GetObstacleById(int id);

        Task<IEnumerable<Obstacle>> GetAllObstacles(Obstacle obstacleData);

        Task<Obstacle> DeleteById(int id);

        Task<Obstacle> UpdateObstacle(Obstacle obstacleData);


    }
}
