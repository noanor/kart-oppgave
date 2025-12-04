using Luftfartshinder.DataContext;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class ReportRepositoryTests
    {
        private ApplicationContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationContext(options);
        }

        /// <summary>
        /// GOAL: Test that AddAsync can add a valid report to the database
        /// LOGIC: Creates a report with valid data and calls AddAsync
        /// RESULT: Report should be returned with a generated ID > 0 and correct data
        /// </summary>
        [Fact]
        public async Task AddAsync_ValidReport_ReturnsReportWithId()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ReportRepository(context);
            var report = new Report
            {
                Title = "Test Report",
                Author = "Test Author",
                AuthorId = "author-123",
                ReportDate = DateTime.Now,
                OrganizationId = 1
            };

            // Act
            var result = await repository.AddAsync(report);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("Test Report", result.Title);
            Assert.Equal("Test Author", result.Author);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsReport()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ReportRepository(context);
            var report = new Report
            {
                Title = "Test Report",
                Author = "Test Author",
                AuthorId = "author-123",
                ReportDate = DateTime.Now,
                OrganizationId = 1
            };
            await repository.AddAsync(report);
            var addedId = report.Id;

            // Act
            var result = await repository.GetByIdAsync(addedId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(addedId, result.Id);
            Assert.Equal("Test Report", result.Title);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ReportRepository(context);

            // Act
            var result = await repository.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllReports()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ReportRepository(context);
            
            await repository.AddAsync(new Report
            {
                Title = "Report 1",
                Author = "Author 1",
                AuthorId = "author-1",
                ReportDate = DateTime.Now,
                OrganizationId = 1
            });

            await repository.AddAsync(new Report
            {
                Title = "Report 2",
                Author = "Author 2",
                AuthorId = "author-2",
                ReportDate = DateTime.Now,
                OrganizationId = 1
            });

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByOrgId_ReturnsReportsForOrganization()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ReportRepository(context);
            
            await repository.AddAsync(new Report
            {
                Title = "Org1 Report",
                Author = "Author 1",
                AuthorId = "author-1",
                ReportDate = DateTime.Now,
                OrganizationId = 1
            });

            await repository.AddAsync(new Report
            {
                Title = "Org2 Report",
                Author = "Author 2",
                AuthorId = "author-2",
                ReportDate = DateTime.Now,
                OrganizationId = 2
            });

            // Act
            var result = await repository.GetByOrgId(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Org1 Report", result.First().Title);
        }

        [Fact]
        public async Task UpdateAsync_ExistingReport_UpdatesTitle()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ReportRepository(context);
            var report = new Report
            {
                Title = "Original Title",
                Author = "Test Author",
                AuthorId = "author-123",
                ReportDate = DateTime.Now,
                OrganizationId = 1
            };
            await repository.AddAsync(report);
            var id = report.Id;

            var updatedReport = new Report
            {
                Id = id,
                Title = "Updated Title",
                Author = "Test Author",
                AuthorId = "author-123",
                ReportDate = DateTime.Now,
                OrganizationId = 1
            };

            // Act
            var result = await repository.UpdateAsync(updatedReport);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Updated Title", result.Title);
        }

        [Fact]
        public async Task UpdateAsync_NonExistingReport_ReturnsNull()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ReportRepository(context);
            var report = new Report
            {
                Id = 999,
                Title = "Test Title",
                Author = "Test Author",
                AuthorId = "author-123",
                ReportDate = DateTime.Now,
                OrganizationId = 1
            };

            // Act
            var result = await repository.UpdateAsync(report);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ExistingReport_RemovesReport()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ReportRepository(context);
            var report = new Report
            {
                Title = "To Delete",
                Author = "Test Author",
                AuthorId = "author-123",
                ReportDate = DateTime.Now,
                OrganizationId = 1
            };
            await repository.AddAsync(report);
            var id = report.Id;

            // Act
            var result = await repository.DeleteAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            
            // Verify it's deleted
            var deleted = await repository.GetByIdAsync(id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ReportRepository(context);

            // Act
            var result = await repository.DeleteAsync(999);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// GOAL: Test that DeleteAsync removes both report and associated obstacles
        /// LOGIC: Creates a report with obstacles, deletes the report and verifies that both report and obstacles are removed
        /// RESULT: Both report and all associated obstacles should be deleted from the database
        /// </summary>
        [Fact]
        public async Task DeleteAsync_ReportWithObstacles_RemovesReportAndObstacles()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ReportRepository(context);
            var report = new Report
            {
                Title = "Report with Obstacles",
                Author = "Test Author",
                AuthorId = "author-123",
                ReportDate = DateTime.Now,
                OrganizationId = 1
            };
            await repository.AddAsync(report);
            var id = report.Id;

            // Add obstacles to the report
            var obstacleRepo = new ObstacleRepository(context);
            await obstacleRepo.AddObstacle(new Obstacle
            {
                Type = "Mast",
                Name = "Test Mast",
                Height = 50,
                Latitude = 59.9139,
                Longitude = 10.7522,
                OrganizationId = 1,
                ReportId = id
            });

            // Act
            var result = await repository.DeleteAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            
            // Verify report is deleted
            var deleted = await repository.GetByIdAsync(id);
            Assert.Null(deleted);
        }
    }
}

