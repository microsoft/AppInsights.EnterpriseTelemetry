using Moq;
using System;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using AppInsights.EnterpriseTelemetry.Client;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppInsights.EnterpriseTelemetry.Configurations;

namespace AppInsights.EnterpriseTelemetry.Tests.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class AppInsightRequestBodyLoggerTests
    {
        [TestMethod]
        public void AppInsightsLogger_ShouldNotLogRequestBody_WhenTeleletryEnahcementIsOff()
        {
            #region Arrange
            var fakeRequestBody = Guid.NewGuid().ToString();
            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose,
                RequestTelemetryEnhanced = false,
                RequestBodyTrackingEnabled = true
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackRequest(It.IsAny<RequestTelemetry>()));

            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Host = new HostString("test.com");
            defaultHttpContext.Request.Scheme = "https";
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(hca => hca.HttpContext)
                .Returns(defaultHttpContext);

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), stubIHttpContextAccessor.Object);
            #endregion Arrange

            #region Act
            logger.LogRequestBody(fakeRequestBody);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify(client => client.TrackRequest(It.IsAny<RequestTelemetry>()), Times.Never);
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldNotLogRequestBody_WhenRequestBodyTrackingIsDisabled()
        {
            #region Arrange
            var fakeRequestBody = Guid.NewGuid().ToString();
            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose,
                RequestTelemetryEnhanced = true,
                RequestBodyTrackingEnabled = false
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackRequest(It.IsAny<RequestTelemetry>()));

            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Host = new HostString("test.com");
            defaultHttpContext.Request.Scheme = "https";
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(hca => hca.HttpContext)
                .Returns(defaultHttpContext);

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), stubIHttpContextAccessor.Object);
            #endregion Arrange

            #region Act
            logger.LogRequestBody(fakeRequestBody);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify(client => client.TrackRequest(It.IsAny<RequestTelemetry>()), Times.Never);
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldNotLogRequestBody_WhenHttpContextAccessorIsNull()
        {
            #region Arrange
            var fakeRequestBody = Guid.NewGuid().ToString();
            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose,
                RequestTelemetryEnhanced = true,
                RequestBodyTrackingEnabled = true
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackRequest(It.IsAny<RequestTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), null);
            #endregion Arrange

            #region Act
            logger.LogRequestBody(fakeRequestBody);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify(client => client.TrackRequest(It.IsAny<RequestTelemetry>()), Times.Never);
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldLogRequestBody_OfTypeString()
        {
            #region Arrange
            var fakeRequestBody = Guid.NewGuid().ToString();
            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose,
                RequestTelemetryEnhanced = true,
                RequestBodyTrackingEnabled = true
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackRequest(It.IsAny<RequestTelemetry>()));

            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Host = new HostString("test.com");
            defaultHttpContext.Request.Scheme = "https";
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(hca => hca.HttpContext)
                .Returns(defaultHttpContext);

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), stubIHttpContextAccessor.Object);
            #endregion Arrange

            #region Act
            logger.LogRequestBody(fakeRequestBody);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify(client => client.TrackRequest(It.IsAny<RequestTelemetry>()), Times.Once);
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldLogRequestBody_OfTypeObj()
        {
            #region Arrange
            var fakeRequestBody = new
            {
                Val = "Value"
            };
            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose,
                RequestTelemetryEnhanced = true,
                RequestBodyTrackingEnabled = true
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackRequest(It.IsAny<RequestTelemetry>()));

            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Host = new HostString("test.com");
            defaultHttpContext.Request.Scheme = "https";
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(hca => hca.HttpContext)
                .Returns(defaultHttpContext);

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), stubIHttpContextAccessor.Object);
            #endregion Arrange

            #region Act
            logger.LogRequestBody(fakeRequestBody);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify(client => client.TrackRequest(It.IsAny<RequestTelemetry>()), Times.Once);
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldLogEmptyRequestBody()
        {
            #region Arrange
            string fakeRequestBody = null;
            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose,
                RequestTelemetryEnhanced = true,
                RequestBodyTrackingEnabled = true
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackRequest(It.IsAny<RequestTelemetry>()));

            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Host = new HostString("test.com");
            defaultHttpContext.Request.Scheme = "https";
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(hca => hca.HttpContext)
                .Returns(defaultHttpContext);

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), stubIHttpContextAccessor.Object);
            #endregion Arrange

            #region Act
            logger.LogRequestBody(fakeRequestBody);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify(client => client.TrackRequest(It.IsAny<RequestTelemetry>()), Times.Once);
            stubIAppInsightsTelemetryClientWrapper.Verify(client => client.TrackRequest(It.Is<RequestTelemetry>(rt => rt.Properties["Request:Body"] == "__EMPTY__")), Times.Once);
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldAddRequestBody_OfTypeString_WhenRequestTelemetryIsPresent()
        {
            #region Arrange
            var fakeRequestBody = Guid.NewGuid().ToString();
            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose,
                RequestTelemetryEnhanced = true,
                RequestBodyTrackingEnabled = true
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackRequest(It.IsAny<RequestTelemetry>()));

            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Host = new HostString("test.com");
            defaultHttpContext.Request.Scheme = "https";
            var fakeRequestTelemetry = new RequestTelemetry();
            defaultHttpContext.Features.Set(fakeRequestTelemetry);
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(hca => hca.HttpContext)
                .Returns(defaultHttpContext);

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), stubIHttpContextAccessor.Object);
            #endregion Arrange

            #region Act
            logger.LogRequestBody(fakeRequestBody);
            #endregion Act

            #region Assert
            Assert.IsTrue(fakeRequestTelemetry.Properties.ContainsKey("Request:Body"));
            Assert.AreEqual(fakeRequestBody, fakeRequestTelemetry.Properties["Request:Body"]);
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldLogInternalException_WhenLogginFails()
        {
            #region Arrange
            var fakeRequestBody = Guid.NewGuid().ToString();
            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose,
                RequestTelemetryEnhanced = true,
                RequestBodyTrackingEnabled = true
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackRequest(It.IsAny<RequestTelemetry>()))
                .Throws(new Exception("Fake Exception"));
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackException(It.IsAny<ExceptionTelemetry>()));

            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Host = new HostString("test.com");
            defaultHttpContext.Request.Scheme = "https";
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(hca => hca.HttpContext)
                .Returns(defaultHttpContext);

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), stubIHttpContextAccessor.Object);
            #endregion Arrange

            #region Act
            logger.LogRequestBody(fakeRequestBody);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify(client => client.TrackRequest(It.IsAny<RequestTelemetry>()), Times.Once);
            stubIAppInsightsTelemetryClientWrapper.Verify(client => client.TrackException(It.IsAny<ExceptionTelemetry>()), Times.Once);
            #endregion Assert
        }

        private IContextPropertyBuilder CreateMockContextPropertyBuilder()
        {
            Mock<IContextPropertyBuilder> mockPropertyBuilder = new Mock<IContextPropertyBuilder>();
            return mockPropertyBuilder.Object;
        }
    }
}
