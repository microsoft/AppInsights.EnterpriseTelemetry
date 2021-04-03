using System.Net;
using System.Diagnostics.CodeAnalysis;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppInsights.EnterpriseTelemetry.AppInsightsInitializers;

namespace AppInsights.EnterpriseTelemetry.Tests.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ResponseCodeTranslationInitializerTests
    {
        [TestMethod]
        public void ResponseCodeTranslationInitializer_ShouldAddResponseCodeDescription_WhenRequestTelemetryIsSent()
        {
            #region Arrange
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "http://testrequest.com",
                ResponseCode = "400",
                Success = false
            };

            var initializer = new ResponseCodeTranslationIntitializer();
            #endregion Arrange

            #region Act
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Assert
            Assert.IsTrue(requestTelemetry.Properties.ContainsKey("Response Code Description"), message: "Response code property not present");
            Assert.AreEqual(HttpStatusCode.BadRequest.ToString(), requestTelemetry.Properties["Response Code Description"], message: "Response code property not present");
            #endregion Assert
        }

        [TestMethod]
        public void ResponseCodeTranslationInitializer_ShouldNotAddResponseCodeDescription_WhenRequestTelemetryIsNotSent()
        {
            #region Arrange
            var eventTelemetry = new EventTelemetry()
            {
                Name = "Sample Event"
            };

            var initializer = new ResponseCodeTranslationIntitializer();
            #endregion Arrange

            #region Act
            initializer.Initialize(eventTelemetry);
            #endregion Act

            #region Assert
            Assert.IsFalse(eventTelemetry.Properties.ContainsKey("Response Code Description"), message: "Response code property present");
            #endregion Assert
        }

        [TestMethod]
        public void ResponseCodeTranslationInitializer_ShouldAddResponseCodeDescription_WhenStatusCodeIsInvalid()
        {
            #region Arrange
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "http://testrequest.com",
                ResponseCode = "INVALID",
                Success = false
            };

            var initializer = new ResponseCodeTranslationIntitializer();
            #endregion Arrange

            #region Act
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Assert
            Assert.IsFalse(requestTelemetry.Properties.ContainsKey("Response Code Description"), message: "Response code property present");
            #endregion Assert
        }

        [TestMethod]
        public void ResponseCodeTranslationInitializer_ShouldAddResponseCodeDescription_WhenWrongStatusCodeIsSent()
        {
            #region Arrange
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "http://testrequest.com",
                ResponseCode = "1000",
                Success = false
            };

            var initializer = new ResponseCodeTranslationIntitializer();
            #endregion Arrange

            #region Act
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Assert
            Assert.IsFalse(requestTelemetry.Properties.ContainsKey("Response Code Description"), message: "Response code property present");
            #endregion Assert
        }
    }
}
