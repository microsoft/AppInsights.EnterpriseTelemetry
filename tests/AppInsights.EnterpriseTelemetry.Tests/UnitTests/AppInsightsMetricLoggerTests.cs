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
    public class AppInsightsMetricLoggerTests
    {
        [TestMethod]
        public void AppInsightsLogger_ShouldTrackMetric_WhenMetricContextIsPassed()
        {
            #region Arrange
            var fakeMetricName = "FAKE METRIC NAME";
            var fakeMetricValue = 3.14159;
            var metricContext = new MetricContext()
            {
                MetricName = fakeMetricName,
                Value = fakeMetricValue
            };

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackMetric(It.IsAny<MetricTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(metricContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackMetric(It.IsAny<MetricTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackMetric(It.Is<MetricTelemetry>(
                metricTelemetry =>
                metricTelemetry.Name == fakeMetricName &&
                metricTelemetry.Sum == fakeMetricValue
                ))), "Metric Name not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackMetric_WhenMetricContextIsPassed_WithNullProperties()
        {
            #region Arrange
            var fakeMetricName = "FAKE METRIC NAME";
            var fakeMetricValue = 3.14159;
            var metricContext = new MetricContext()
            {
                MetricName = fakeMetricName,
                Value = fakeMetricValue
            };
            metricContext.Properties.Clear();

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackMetric(It.IsAny<MetricTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(metricContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackMetric(It.IsAny<MetricTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackMetric(It.Is<MetricTelemetry>(
                metricTelemetry =>
                metricTelemetry.Name == fakeMetricName &&
                metricTelemetry.Sum == fakeMetricValue
                ))), "Metric Name not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackMetric_AndProperties_WhenMetricContextIsPassed()
        {
            #region Arrange
            var fakeMetricName = "FAKE METRIC NAME";
            var fakeMetricValue = 3.14159;
            var fakeCorrelationId = Guid.NewGuid().ToString();
            var fakeTransactionId = Guid.NewGuid().ToString();
            var fakeE2eTrackingId = Guid.NewGuid().ToString();
            var fakeSource = "TEST";
            var fakeUserId = "tester@microsoft.com";

            var metricContext = new MetricContext(fakeMetricName, fakeMetricValue, fakeCorrelationId, fakeTransactionId, fakeSource, fakeUserId, fakeE2eTrackingId);

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackMetric(It.IsAny<MetricTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(metricContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackMetric(It.IsAny<MetricTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackMetric(It.Is<MetricTelemetry>(
                metricTelemetry =>
                metricTelemetry.Name == fakeMetricName &&
                metricTelemetry.Sum == fakeMetricValue &&
                metricTelemetry.Properties != null &&
                metricTelemetry.Properties["XCV"] == fakeCorrelationId &&
                metricTelemetry.Properties["MessageId"] == fakeTransactionId &&
                metricTelemetry.Properties["TrackingId"] == fakeE2eTrackingId &&
                metricTelemetry.Properties["UserId"] == fakeUserId &&
                metricTelemetry.Properties["Source"] == fakeSource
                ))), "Metric Properties not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldNotTrackMetric_AndLogInternal_WhenInternalErrorOccurs()
        {
            #region Arrange
            var fakeMetricName = "FAKE METRIC NAME";
            var fakeMetricValue = 3.14159;
            var fakeExceptionMessage = "FAKE EXCEPTION MESSAGE";
            var metricContext = new MetricContext()
            {
                MetricName = fakeMetricName,
                Value = fakeMetricValue
            };

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackMetric(It.IsAny<MetricTelemetry>()))
                .Throws(new Exception(fakeExceptionMessage));
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackException(It.IsAny<ExceptionTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(metricContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackMetric(It.IsAny<MetricTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.Is<ExceptionTelemetry>(
                exceptionTelemetry => exceptionTelemetry.Exception.Message == fakeExceptionMessage
                ))), "Internal message not logged");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldNotTrackMetric_WhenLogLevelOfMessageIsLower()
        {
            #region Arrange
            var fakeMetricName = "FAKE METRIC NAME";
            var fakeMetricValue = 3.14159;
            var metricContext = new MetricContext()
            {
                MetricName = fakeMetricName,
                Value = fakeMetricValue
            };

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Critical
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackMetric(It.IsAny<MetricTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(metricContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackMetric(It.IsAny<MetricTelemetry>())), Times.Never, "Telemetry not sent to Application Insight");
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
