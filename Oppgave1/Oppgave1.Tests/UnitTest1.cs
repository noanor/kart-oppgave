//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using Oppgave1.Models;
//using Xunit;

//namespace Oppgave1.Tests
//{
//    public class ObstacleDataTests
//    {
//        // Validerer modellen med DataAnnotations
//        private bool ValidateModel(object model, out List<ValidationResult> results)
//        {
//            var context = new ValidationContext(model, null, null);
//            results = new List<ValidationResult>();
//            return Validator.TryValidateObject(model, context, results, true);
//        }
//        [Fact]
//        public void ObstacleData_ShouldBeValid_WhenAllFieldsAreCorrect()
//        {
//            var obstacle = new ObstacleData
//            {
//                ObstacleName = "Tre",
//                ObstacleHeight = 50,
//                ObstacleDescription = "Et stort tre nær landingsområdet",
//                ObstacleCoords = "58.146, 7.995"
//            };

//            var isValid = ValidateModel(obstacle, out var results);

//            Assert.True(isValid);
//            Assert.Empty(results);
//        }

//        [Fact]
//        public void ObstacleData_ShouldFail_WhenHeightIsTooHigh()
//        {
//            var obstacle = new ObstacleData
//            {
//                ObstacleName = "Stolpe",
//                ObstacleHeight = 300,
//                ObstacleDescription = "En veldig høy stolpe",
//                ObstacleCoords = "58.146, 7.995"
//            };

//            var isValid = ValidateModel(obstacle, out var results);

//            Assert.False(isValid);
//            Assert.Contains(results, r => r.ErrorMessage.Contains("between 0 and 200"));
//        }

//        [Fact]
//        public void ObstacleData_ShouldFail_WhenRequiredFieldsAreMissing()
//        {
//            var obstacle = new ObstacleData
//            {
//                ObstacleName = "",
//                ObstacleHeight = 100,
//                ObstacleDescription = "",
//                ObstacleCoords = ""
//            };

//            var isValid = ValidateModel(obstacle, out var results);

//            Assert.False(isValid);
//            Assert.Equal(3, results.Count);
//        }
//    }
//}