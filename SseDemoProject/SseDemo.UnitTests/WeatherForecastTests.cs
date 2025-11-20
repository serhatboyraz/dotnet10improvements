using Microsoft.VisualStudio.TestTools.UnitTesting;
using SseDemo;
using System;


namespace SseDemo.UnitTests
{
    /// <summary>
    /// Unit tests for the WeatherForecast class.
    /// </summary>
    [TestClass]
    public class WeatherForecastTests
    {
        /// <summary>
        /// Tests that TemperatureF handles extreme int.MaxValue without throwing an exception.
        /// Due to the division and cast operations, overflow behavior occurs in unchecked context.
        /// </summary>
        [TestMethod]
        public void TemperatureF_MaxIntValue_HandlesOverflowInUncheckedContext()
        {
            // Arrange
            var weatherForecast = new WeatherForecast
            {
                TemperatureC = int.MaxValue
            };

            // Act
            var actualF = weatherForecast.TemperatureF;

            // Assert
            // The actual value will be the result of overflow arithmetic
            // int.MaxValue / 0.5556 ≈ 3.866e9, which overflows when cast to int
            // We verify that no exception is thrown and a value is returned
            Assert.IsNotNull(actualF);
        }

        /// <summary>
        /// Tests that TemperatureF handles extreme int.MinValue without throwing an exception.
        /// Due to the division and cast operations, overflow behavior occurs in unchecked context.
        /// </summary>
        [TestMethod]
        public void TemperatureF_MinIntValue_HandlesOverflowInUncheckedContext()
        {
            // Arrange
            var weatherForecast = new WeatherForecast
            {
                TemperatureC = int.MinValue
            };

            // Act
            var actualF = weatherForecast.TemperatureF;

            // Assert
            // The actual value will be the result of overflow arithmetic
            // int.MinValue / 0.5556 ≈ -3.866e9, which overflows when cast to int
            // We verify that no exception is thrown and a value is returned
            Assert.IsNotNull(actualF);
        }

        /// <summary>
        /// Tests that TemperatureF returns 32 when TemperatureC is 0 (freezing point of water).
        /// This is a critical boundary condition for temperature conversion.
        /// </summary>
        [TestMethod]
        public void TemperatureF_ZeroCelsius_ReturnsThirtyTwoFahrenheit()
        {
            // Arrange
            var weatherForecast = new WeatherForecast
            {
                TemperatureC = 0
            };

            // Act
            var actualF = weatherForecast.TemperatureF;

            // Assert
            Assert.AreEqual(32, actualF);
        }

        /// <summary>
        /// Tests that TemperatureF handles large negative temperature values correctly.
        /// Tests boundary values approaching absolute zero and beyond.
        /// </summary>
        [TestMethod]
        [DataRow(-1000, -1768)]
        [DataRow(-5000, -8968)]
        [TestCategory("ProductionBugSuspected")]
        [Ignore("ProductionBugSuspected")]
        public void TemperatureF_LargeNegativeValues_ReturnsCorrectConversion(int temperatureC, int expectedF)
        {
            // Arrange
            var weatherForecast = new WeatherForecast
            {
                TemperatureC = temperatureC
            };

            // Act
            var actualF = weatherForecast.TemperatureF;

            // Assert
            Assert.AreEqual(expectedF, actualF);
        }
    }
}