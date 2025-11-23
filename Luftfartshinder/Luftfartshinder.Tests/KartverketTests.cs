using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Luftfartshinder.Tests
{
  
    /// Enkle tester som dekker fire hovedkategorier:
    /// 1. Enhetstesting - tester enkeltkomponenter isolert
    /// 2. Systemstesting - tester hele systemet sammen
    /// 3. Sikkerhetstesting - tester sikkerhetsaspekter
    /// 4. Brukervennlighetstesting - tester brukervennlighet
    /// </summary>
    public class KartverketTests
    {
        /
        // 1. ENHETSTESTING (Unit Testing)
        // Tester enkeltkomponenter isolert
      

        [Fact]
        public void UnitTest_ObstacleHeightValidation_AcceptsValidHeight()
        {
            // Arrange
            var obstacle = new Obstacle { Name = "Test", Latitude = 60.0, Longitude = 10.0 };

            // Act
            obstacle.Height = 150.0;

            // Assert
            Assert.Equal(150.0, obstacle.Height);
        }

        [Fact]
        public void UnitTest_ObstacleHeightValidation_RejectsHeightOver200()
        {
            // Arrange
            var obstacle = new Obstacle { Name = "Test", Latitude = 60.0, Longitude = 10.0 };

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => obstacle.Height = 250.0);
        }
        \
        // 2. SYSTEMSTESTING (System Testing)
        // Tester at hele systemet fungerer sammen
       

        [Fact]
        public void SystemTest_LoginViewModel_ValidatesRequiredFields()
        {
            // Arrange
            var model = new LoginViewModel();

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Username"));
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Password"));
        }

        [Fact]
        public void SystemTest_LoginViewModel_AcceptsValidInput()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Username = "testuser",
                Password = "Password123"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

            // Assert
            Assert.True(isValid);
        }

      
        // 3. SIKKERHETSTESTING (Security Testing)
        // Tester sikkerhetsaspekter
      

        [Fact]
        public void SecurityTest_ObstacleName_IsRequired()
        {
            // Arrange
            var obstacle = new Obstacle { Latitude = 60.0, Longitude = 10.0 };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(obstacle, new ValidationContext(obstacle), validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Name"));
        }

        [Fact]
        public void SecurityTest_ObstacleCoordinates_AreRequired()
        {
            // Arrange
            var obstacle = new Obstacle { Name = "Test" };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(obstacle, new ValidationContext(obstacle), validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Latitude"));
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Longitude"));
        }

        // ============================================
        // 4. BRUKERVENNLIGHETSTESTING (Usability Testing)
        // Tester at systemet er brukervennlig
        // ============================================

        [Fact]
        public void UsabilityTest_Obstacle_CanBeCreatedWithMinimalData()
        {
            // Arrange & Act
            var obstacle = new Obstacle
            {
                Name = "Test Obstacle",
                Latitude = 60.0,
                Longitude = 10.0,
                Height = 50.0
            };

            // Assert
            Assert.NotNull(obstacle);
            Assert.Equal("Test Obstacle", obstacle.Name);
            Assert.Equal(60.0, obstacle.Latitude);
            Assert.Equal(10.0, obstacle.Longitude);
            Assert.Equal(50.0, obstacle.Height);
        }

        [Fact]
        public void UsabilityTest_Obstacle_ProvidesHelpfulErrorMessages()
        {
            // Arrange
            var obstacle = new Obstacle { Latitude = 60.0, Longitude = 10.0 };

            // Act
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(obstacle, new ValidationContext(obstacle), validationResults, true);

            // Assert
            var nameError = validationResults.FirstOrDefault(v => v.MemberNames.Contains("Name"));
            Assert.NotNull(nameError);
            Assert.Contains("required", nameError.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }
    }
}

