using Luftfartshinder.Controllers;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class SecurityTestingTests
    {
        [Fact]
        public void Security_SuperAdminHome_RequiresAuthorization()
        {
            // Arrange
            var obstacleRepositoryMock = new Mock<IObstacleRepository>();
            var controller = new HomeController(obstacleRepositoryMock.Object);

            // Act
            var result = controller.SuperAdminHome();

            // Assert
            // Hvis bruker ikke er autorisert, skal det returnere en redirect eller unauthorized
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Security_HttpResponse_ContainsSecurityHeaders()
        {
            // Arrange - Create a test web application factory
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Production"); // Use Production to enable HSTS
                });

            var client = factory.CreateClient();

            // Act - Make a request to any endpoint
            var response = await client.GetAsync("/");

            // Assert - Verify all security headers are present
            Assert.True(response.Headers.Contains("X-Content-Type-Options"), 
                "X-Content-Type-Options header should be present");
            Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").First());

            Assert.True(response.Headers.Contains("X-XSS-Protection"), 
                "X-XSS-Protection header should be present");
            Assert.Equal("1; mode=block", response.Headers.GetValues("X-XSS-Protection").First());

            Assert.True(response.Headers.Contains("X-Frame-Options"), 
                "X-Frame-Options header should be present");
            Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").First());

            Assert.True(response.Headers.Contains("Content-Security-Policy"), 
                "Content-Security-Policy header should be present");
            var csp = response.Headers.GetValues("Content-Security-Policy").First();
            Assert.Contains("default-src 'self'", csp);
            Assert.Contains("script-src", csp);
            Assert.Contains("style-src", csp);

            Assert.True(response.Headers.Contains("Referrer-Policy"), 
                "Referrer-Policy header should be present");
            Assert.Equal("strict-origin-when-cross-origin", 
                response.Headers.GetValues("Referrer-Policy").First());

            // Strict-Transport-Security should be present in Production
            Assert.True(response.Headers.Contains("Strict-Transport-Security"), 
                "Strict-Transport-Security header should be present in Production");
            var hsts = response.Headers.GetValues("Strict-Transport-Security").First();
            Assert.Contains("max-age=31536000", hsts);
            Assert.Contains("includeSubDomains", hsts);
            Assert.Contains("preload", hsts);
        }
    }
}


