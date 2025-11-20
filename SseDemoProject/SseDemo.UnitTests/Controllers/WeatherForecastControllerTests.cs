using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SseDemo.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;


namespace SseDemo.Controllers.UnitTests
{
    /// <summary>
    /// Unit tests for the WeatherForecastController class.
    /// </summary>
    [TestClass]
    public class WeatherForecastControllerTests
    {
        /// <summary>
        /// Tests that the Get method returns exactly five weather forecasts.
        /// </summary>
        [TestMethod]
        public void Get_ReturnsExactlyFiveForecasts()
        {
            // Arrange
            var controller = new WeatherForecastController();

            // Act
            var result = controller.Get();

            // Assert
            Assert.IsNotNull(result);
            var forecasts = result.ToArray();
            Assert.AreEqual(5, forecasts.Length);
        }

        /// <summary>
        /// Tests that all returned forecasts have valid Date, TemperatureC, and Summary properties.
        /// Validates that dates are sequential starting from tomorrow, temperatures are within the valid range [-20, 54],
        /// and summaries are from the predefined list of valid weather descriptions.
        /// </summary>
        [TestMethod]
        public void Get_AllForecastsHaveValidDateTemperatureAndSummary()
        {
            // Arrange
            var controller = new WeatherForecastController();
            var validSummaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            var today = DateTime.Now.Date;

            // Act
            var result = controller.Get();

            // Assert
            Assert.IsNotNull(result);
            var forecasts = result.ToArray();

            for (int i = 0; i < forecasts.Length; i++)
            {
                var forecast = forecasts[i];

                // Verify forecast is not null
                Assert.IsNotNull(forecast, $"Forecast at index {i} should not be null");

                // Verify Date is sequential (starting from tomorrow, i.e., today + 1, today + 2, etc.)
                var expectedDate = DateOnly.FromDateTime(today.AddDays(i + 1));
                Assert.AreEqual(expectedDate, forecast.Date, $"Forecast at index {i} should have date {expectedDate}");

                // Verify TemperatureC is within valid range [-20, 54] (maxValue 55 is exclusive)
                Assert.IsTrue(forecast.TemperatureC >= -20 && forecast.TemperatureC <= 54,
                    $"Forecast at index {i} has temperature {forecast.TemperatureC} which is outside the valid range [-20, 54]");

                // Verify Summary is from the valid list
                Assert.IsNotNull(forecast.Summary, $"Forecast at index {i} should have a non-null Summary");
                Assert.IsTrue(validSummaries.Contains(forecast.Summary),
                    $"Forecast at index {i} has Summary '{forecast.Summary}' which is not in the valid list");
            }
        }
    }
}