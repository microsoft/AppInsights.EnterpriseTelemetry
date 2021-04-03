using Moq;
using System;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using AppInsights.EnterpriseTelemetry.Client;
using AppInsights.EnterpriseTelemetry.Context;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppInsights.EnterpriseTelemetry.Configurations;

namespace AppInsights.EnterpriseTelemetry.Tests.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class AppInsightsDependencyLoggerTests
    {
        [TestMethod]
        public void AppInsightsLogger_ShouldLogDependency_WhenDependencyContextIsPassed()
        {
            #region Arrange
            var fakeDepName = "FAKE DEPENDENCY TELEMETY";
            var fakeTarget = "https://test.com";
            var fakeResponseCode = "202";
            var dependencyContext = new DependencyContext()
            {
                DependencyName = fakeDepName,
                TargetSystemName = fakeTarget,
                DependencyType = "Test",
                RequestDetails = Guid.NewGuid().ToString(),
                IsSuccessfull = true,
                ResponseCode = fakeResponseCode
            };

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackEvent(It.IsAny<EventTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(dependencyContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackDependency(It.IsAny<DependencyTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackDependency(It.Is<DependencyTelemetry>(dc =>
                dc.ResultCode == fakeResponseCode &&
                dc.Target == fakeTarget &&
                dc.Name == fakeDepName
                ))), 
                Times.Once, 
                "Telemetry not sent to Application Insight");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldLogFailedDependency_WhenDependencyContextIsPassed()
        {
            #region Arrange
            var fakeDepName = "FAKE DEPENDENCY TELEMETY";
            var fakeTarget = "https://test.com";
            var fakeResponseCode = "500";
            var exception = new Exception("FAKE FAILURE");
            var dependencyContext = new DependencyContext()
            {
                DependencyName = fakeDepName,
                TargetSystemName = fakeTarget,
                DependencyType = "Test",
                RequestDetails = Guid.NewGuid().ToString(),
                IsSuccessfull = false,
                ResponseCode = fakeResponseCode,
                ResponseError = exception
            };

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackEvent(It.IsAny<EventTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(dependencyContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackDependency(It.IsAny<DependencyTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.IsAny<ExceptionTelemetry>())), Times.Once, "Exception Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackDependency(It.Is<DependencyTelemetry>(dc =>
                dc.ResultCode == fakeResponseCode &&
                dc.Target == fakeTarget &&
                dc.Name == fakeDepName
                ))),
                Times.Once,
                "Telemetry not sent to Application Insight");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldLogInternalException_WhenDependencyLoggingFails()
        {
            #region Arrange
            var fakeDepName = "FAKE DEPENDENCY TELEMETY";
            var fakeTarget = "https://test.com";
            var fakeResponseCode = "202";
            var dependencyContext = new DependencyContext()
            {
                DependencyName = fakeDepName,
                TargetSystemName = fakeTarget,
                DependencyType = "Test",
                RequestDetails = Guid.NewGuid().ToString(),
                IsSuccessfull = true,
                ResponseCode = fakeResponseCode
            };

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackDependency(It.IsAny<DependencyTelemetry>()))
                .Throws(new Exception("Fake Exception"));
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackException(It.IsAny<ExceptionTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(dependencyContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackDependency(It.IsAny<DependencyTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.IsAny<ExceptionTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            #endregion Assert
        }

        private IContextPropertyBuilder CreateMockContextPropertyBuilder()
        {
            Mock<IContextPropertyBuilder> mockPropertyBuilder = new Mock<IContextPropertyBuilder>();
            return mockPropertyBuilder.Object;
        }

        private IHttpContextAccessor CreateMockHttpContextAccessor()
        {
            Mock<IHttpContextAccessor> mockContextAccessor = new Mock<IHttpContextAccessor>();
            return mockContextAccessor.Object;
        }
    }
}
