using Oppgave1.Models;

namespace Oppgave1.Repository
{
    public interface IDataRepocs
    {
        Task<ObstacleData> AddObstacle(ObstacleData obstacleData);

        Task<ObstacleData> GetObstacleById(int id);

        Task<IEnumerable<ObstacleData>> GetAllObstacles(ObstacleData obstacleData);

        Task<ObstacleData>DeleteById(int id);

        Task<ObstacleData> UpdateObstacle(ObstacleData obstacleData);


    }
}
