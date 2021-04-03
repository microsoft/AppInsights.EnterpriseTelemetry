using Microsoft.ApplicationInsights;
using System.Diagnostics.CodeAnalysis;
using AppInsights.EnterpriseTelemetry.Client;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS0618 // Type or member is obsolete. Used for unit testing only.
namespace AppInsights.EnterpriseTelemetry.Tests.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class AppInsightsClientWrapperTests
    {
        [TestMethod]
        public void AppInsightsClientWrapper_ShouldTrackTrace_WhenTrackTraceIsCalled()
        {
            #region Arrange
            var telemetryClient = new TelemetryClient();
            var clientWrapper = new AppInsightsTelemetryClientWrapper(telemetryClient);
            var traceTelemetry = new TraceTelemetry();
            #endregion Arrange

            #region Act
            clientWrapper.TrackTrace(traceTelemetry);
            #endregion Act

            #region Assert
            //Verification not possible
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsClientWrapper_ShouldTrackException_WhenTrackExceptionIsCalled()
        {
            #region Arrange
            var telemetryClient = new TelemetryClient();
            var clientWrapper = new AppInsightsTelemetryClientWrapper(telemetryClient);
            var exceptionTelemetry = new ExceptionTelemetry();
            #endregion Arrange

            #region Act
            clientWrapper.TrackException(exceptionTelemetry);
            #endregion Act

            #region Assert
            //Verification not possible
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsClientWrapper_ShouldTrackEvent_WhenTrackEventIsCalled()
        {
            #region Arrange
            var telemetryClient = new TelemetryClient();
            var clientWrapper = new AppInsightsTelemetryClientWrapper(telemetryClient);
            var eventTelemetry = new EventTelemetry();
            #endregion Arrange

            #region Act
            clientWrapper.TrackEvent(eventTelemetry);
            #endregion Act

            #region Assert
            //Verification not possible
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsClientWrapper_ShouldTrackMetric_WhenTrackMetricIsCalled()
        {
            #region Arrange
            var telemetryClient = new TelemetryClient();
            var clientWrapper = new AppInsightsTelemetryClientWrapper(telemetryClient);
            var metricTelemetry = new MetricTelemetry();
            #endregion Arrange

            #region Act
            clientWrapper.TrackMetric(metricTelemetry);
            #endregion Act

            #region Assert
            //Verification not possible
            #endregion Assert
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete. Used for unit testing only.