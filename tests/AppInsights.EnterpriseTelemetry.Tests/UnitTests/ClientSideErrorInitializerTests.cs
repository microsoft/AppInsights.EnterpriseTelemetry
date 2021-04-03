using System.Diagnostics.CodeAnalysis;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppInsights.EnterpriseTelemetry.AppInsightsInitializers;

namespace AppInsights.EnterpriseTelemetry.Tests.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ClientSideErrorInitializerTests
    {
        [TestMethod]
        public void ClientSideErrorInitializer_ShouldOverrideSuccesField_When400StatusCodeRequestIsReceived()
        {
            #region Arrange
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "http://testrequest.com",
                ResponseCode = "400",
                Success = false
            };

            var initializer = new ClientSideErrorInitializer();
            #endregion Arrange

            #region Act
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Assert
            Assert.IsTrue(requestTelemetry.Success.Value, message: "Success status not reverted");
            Assert.IsTrue(requestTelemetry.Properties.ContainsKey("Overridden400s"), message: "Overridden property not present");
            #endregion Assert
        }

        [TestMethod]
        public void ClientSideErrorInitializer_ShouldOverrideSuccesField_When404StatusCodeRequestIsReceived()
        {
            #region Arrange
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "http://testrequest.com",
                ResponseCode = "404",
                Success = false
            };

            var initializer = new ClientSideErrorInitializer();
            #endregion Arrange

            #region Act
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Assert
            Assert.IsTrue(requestTelemetry.Success.Value, message: "Success status not reverted");
            Assert.IsTrue(requestTelemetry.Properties.ContainsKey("Overridden400s"), message: "Overridden property not present");
            #endregion Assert
        }

        //Edge case test
        [TestMethod]
        public void ClientSideErrorInitializer_ShouldOverrideSuccesField_When499StatusCodeRequestIsReceived() 
        {
            #region Arrange
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "http://testrequest.com",
                ResponseCode = "404",
                Success = false
            };

            var initializer = new ClientSideErrorInitializer();
            #endregion Arrange

            #region Act
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Assert
            Assert.IsTrue(requestTelemetry.Success.Value, message: "Success status not reverted");
            Assert.IsTrue(requestTelemetry.Properties.ContainsKey("Overridden400s"), message: "Overridden property not present");
            #endregion Assert
        }

        [TestMethod]
        public void ClientSideErrorInitializer_ShouldNotOverrideSuccesField_WhenStatusCode500IsReceived()
        {
            #region Arrange
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "http://testrequest.com",
                ResponseCode = "500",
                Success = false
            };

            var initializer = new ClientSideErrorInitializer();
            #endregion Arrange

            #region Act
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Assert
            Assert.IsFalse(requestTelemetry.Success.Value, message: "Success status reverted");
            Assert.IsFalse(requestTelemetry.Properties.ContainsKey("Overridden400s"), message: "Overridden property present");
            #endregion Assert
        }

        [TestMethod]
        public void ClientSideErrorInitializer_ShouldNotOverrideSuccesField_WhenStatusCode200IsReceived()
        {
            #region Arrange
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "http://testrequest.com",
                ResponseCode = "200",
                Success = false
            };

            var initializer = new ClientSideErrorInitializer();
            #endregion Arrange

            #region Act
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Assert
            Assert.IsFalse(requestTelemetry.Success.Value, message: "Success status reverted");
            Assert.IsFalse(requestTelemetry.Properties.ContainsKey("Overridden400s"), message: "Overridden property present");
            #endregion Assert
        }

        [TestMethod]
        public void ClientSideErrorInitializer_ShouldNotOverrideSuccesField_WhenStatusCodeIsInvalid()
        {
            #region Arrange
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "http://testrequest.com",
                ResponseCode = "INVALID",
                Success = false
            };

            var initializer = new ClientSideErrorInitializer();
            #endregion Arrange

            #region Act
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Assert
            Assert.IsFalse(requestTelemetry.Success.Value, message: "Success status reverted");
            Assert.IsFalse(requestTelemetry.Properties.ContainsKey("Overridden400s"), message: "Overridden property present");
            #endregion Assert
        }

        [TestMethod]
        public void ClientSideErrorInitializer_ShouldNotOverrideSuccesField_WhenRequestTelemetryIsNotSend()
        {
            #region Arrange
            var eventTelemetry = new EventTelemetry()
            {
                Name = "Sample Event"
            };

            var initializer = new ClientSideErrorInitializer();
            #endregion Arrange

            #region Act
            initializer.Initialize(eventTelemetry);
            #endregion Act

            #region Assert
            Assert.IsFalse(eventTelemetry.Properties.ContainsKey("Overridden400s"), message: "Overridden property present");
            #endregion Assert
        }
    }
}
