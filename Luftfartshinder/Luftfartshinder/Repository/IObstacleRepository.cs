using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.Repository
{
    public interface IObstacleRepository
    {
        Task<Obstacle> AddObstacle(Obstacle obstacleData);

        Task<Obstacle?> GetObstacleById(int id);
        Task<List<Obstacle>> GetByOrgId(int id);

        Task<List<Obstacle>> GetAllAsync();

        Task<Obstacle> DeleteById(int id);

        Task<Obstacle> UpdateObstacle(Obstacle obstacleData);


    }
}

