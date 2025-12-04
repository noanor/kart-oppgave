using Luftfartshinder.DataContext;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class ObstacleRepositoryTests
    {
        private ApplicationContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationContext(options);
        }

        /// <summary>
        /// MÅL: Teste at AddObstacle kan legge til en gyldig obstacle i databasen
        /// LOGIKK: Oppretter en obstacle med gyldig data og kaller AddObstacle
        /// RESULTAT: Obstacle skal returneres med en generert ID > 0 og korrekt data
        /// </summary>
        [Fact]
        public async Task AddObstacle_ValidObstacle_ReturnsObstacleWithId()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ObstacleRepository(context);
            
            // First create a report
            var reportRepo = new ReportRepository(context);
            var report = new Report
            {
                Title = "Test Report",
                Author = "Test Author",
                AuthorId = "author-123",
                ReportDate = DateTime.Now,
                OrganizationId = 1
            };
            await reportRepo.AddAsync(report);
            
            var obstacle = new Obstacle
            {
                Type = "Mast",
                Name = "Test Mast",
                Height = 50,
                Latitude = 59.9139,
                Longitude = 10.7522,
                OrganizationId = 1,
                ReportId = report.Id
            };

            // Act
            var result = await repository.AddObstacle(obstacle);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("Test Mast", result.Name);
            Assert.Equal(50, result.Height);
        }

        /// <summary>
        /// MÅL: Teste at GetObstacleById kan hente en obstacle med en eksisterende ID
        /// LOGIKK: Legger til en obstacle, henter den med ID og verifiserer at riktig obstacle returneres
        /// RESULTAT: Obstacle med matchende ID skal returneres med korrekt data
        /// </summary>
        [Fact]
        public async Task GetObstacleById_ExistingId_ReturnsObstacle()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ObstacleRepository(context);
            
            // First create a report
            var reportRepo = new ReportRepository(context);
            var report = new Report
            {
                Title = "Test Report",
                Author = "Test Author",
                AuthorId = "author-123",
                ReportDate = DateTime.Now,
                OrganizationId = 1
            };
            await reportRepo.AddAsync(report);
            
            var obstacle = new Obstacle
            {
                Type = "Mast",
                Name = "Test Mast",
                Height = 50,
                Latitude = 59.9139,
                Longitude = 10.7522,
                OrganizationId = 1,
                ReportId = report.Id
            };
            await repository.AddObstacle(obstacle);
            var addedId = obstacle.Id;

            // Act
            var result = await repository.GetObstacleById(addedId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(addedId, result.Id);
            Assert.Equal("Test Mast", result.Name);
        }

        /// <summary>
        /// MÅL: Teste at GetObstacleById håndterer ikke-eksisterende ID korrekt
        /// LOGIKK: Prøver å hente en obstacle med en ID som ikke finnes i databasen
        /// RESULTAT: Metoden skal returnere null når ID ikke finnes
        /// </summary>
        [Fact]
        public async Task GetObstacleById_NonExistingId_ReturnsNull()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ObstacleRepository(context);

            // Act
            var result = await repository.GetObstacleById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllObstacles()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ObstacleRepository(context);
            
            // First create a report
            var reportRepo = new ReportRepository(context);
            var report = new Report
            {
                Title = "Test Report",
                Author = "Test Author",
                AuthorId = "author-123",
                ReportDate = DateTime.Now,
                OrganizationId = 1
            };
            await reportRepo.AddAsync(report);
            
            await repository.AddObstacle(new Obstacle
            {
                Type = "Mast",
                Name = "Mast 1",
                Height = 50,
                Latitude = 59.9139,
                Longitude = 10.7522,
                OrganizationId = 1,
                ReportId = report.Id
            });

            await repository.AddObstacle(new Obstacle
            {
                Type = "Powerline",
                Name = "Powerline 1",
                Height = 30,
                Latitude = 60.0,
                Longitude = 11.0,
                OrganizationId = 1,
                ReportId = report.Id
            });

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByOrgId_ReturnsObstaclesForOrganization()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ObstacleRepository(context);
            
            // First create reports
            var reportRepo = new ReportRepository(context);
            var report1 = new Report
            {
                Title = "Report 1",
                Author = "Author 1",
                AuthorId = "author-1",
                ReportDate = DateTime.Now,
                OrganizationId = 1
            };
            var report2 = new Report
            {
                Title = "Report 2",
                Author = "Author 2",
                AuthorId = "author-2",
                ReportDate = DateTime.Now,
                OrganizationId = 2
            };
            await reportRepo.AddAsync(report1);
            await reportRepo.AddAsync(report2);
            
            await repository.AddObstacle(new Obstacle
            {
                Type = "Mast",
                Name = "Org1 Mast",
                Height = 50,
                Latitude = 59.9139,
                Longitude = 10.7522,
                OrganizationId = 1,
                ReportId = report1.Id
            });

            await repository.AddObstacle(new Obstacle
            {
                Type = "Powerline",
                Name = "Org2 Powerline",
                Height = 30,
                Latitude = 60.0,
                Longitude = 11.0,
                OrganizationId = 2,
                ReportId = report2.Id
            });

            // Act
            var result = await repository.GetByOrgId(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Org1 Mast", result.First().Name);
        }

        [Fact]
        public async Task UpdateObstacle_ExistingObstacle_UpdatesProperties()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ObstacleRepository(context);
            
            // First create a report
            var reportRepo = new ReportRepository(context);
            var report = new Report
            {
                Title = "Test Report",
                Author = "Test Author",
                AuthorId = "author-123",
                ReportDate = DateTime.Now,
                OrganizationId = 1
            };
            await reportRepo.AddAsync(report);
            
            var obstacle = new Obstacle
            {
                Type = "Mast",
                Name = "Original Name",
                Height = 50,
                Latitude = 59.9139,
                Longitude = 10.7522,
                OrganizationId = 1,
                ReportId = report.Id
            };
            await repository.AddObstacle(obstacle);
            var id = obstacle.Id;

            var updatedObstacle = new Obstacle
            {
                Id = id,
                Type = "Powerline",
                Name = "Updated Name",
                Height = 60,
                Latitude = 60.0,
                Longitude = 11.0,
                OrganizationId = 1,
                ReportId = report.Id
            };

            // Act
            var result = await repository.UpdateObstacle(updatedObstacle);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Updated Name", result.Name);
            Assert.Equal("Powerline", result.Type);
            Assert.Equal(60, result.Height);
        }

        [Fact]
        public async Task UpdateObstacle_NonExistingObstacle_ReturnsNull()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ObstacleRepository(context);
            var obstacle = new Obstacle
            {
                Id = 999,
                Type = "Mast",
                Name = "Test",
                Height = 50,
                Latitude = 59.9139,
                Longitude = 10.7522,
                OrganizationId = 1,
                ReportId = 1
            };

            // Act
            var result = await repository.UpdateObstacle(obstacle);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteById_ExistingId_RemovesObstacle()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ObstacleRepository(context);
            
            // First create a report
            var reportRepo = new ReportRepository(context);
            var report = new Report
            {
                Title = "Test Report",
                Author = "Test Author",
                AuthorId = "author-123",
                ReportDate = DateTime.Now,
                OrganizationId = 1
            };
            await reportRepo.AddAsync(report);
            
            var obstacle = new Obstacle
            {
                Type = "Mast",
                Name = "To Delete",
                Height = 50,
                Latitude = 59.9139,
                Longitude = 10.7522,
                OrganizationId = 1,
                ReportId = report.Id
            };
            await repository.AddObstacle(obstacle);
            var id = obstacle.Id;

            // Act
            var result = await repository.DeleteById(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            
            // Verify it's deleted
            var deleted = await repository.GetObstacleById(id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteById_NonExistingId_ReturnsNull()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ObstacleRepository(context);

            // Act
            var result = await repository.DeleteById(999);

            // Assert
            Assert.Null(result);
        }
    }
}

