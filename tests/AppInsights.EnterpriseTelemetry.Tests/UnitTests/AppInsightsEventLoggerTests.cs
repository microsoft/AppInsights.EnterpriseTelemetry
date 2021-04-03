using Moq;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
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
    public class AppInsightsEventLoggerTests
    {
        [TestMethod]
        public void AppInsightsLogger_ShouldTrackEvent_WhenEventContextIsPassed()
        {
            #region Arrange
            var fakeEventName = "FAKE TELEMETY MESSAGE";
            var eventContext = new EventContext()
            {
                EventName = fakeEventName
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
            logger.Log(eventContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackEvent(It.IsAny<EventTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackEvent(It.Is<EventTelemetry>(
                eventTelemetry => eventTelemetry.Name == fakeEventName
                ))), "Wrong event name");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackEvent_WhenEventContextIsPassed_WithNullPropertiesAndMetrics()
        {
            #region Arrange
            var fakeEventName = "FAKE TELEMETY MESSAGE";
            var eventContext = new EventContext()
            {
                EventName = fakeEventName
            };
            eventContext.Properties.Clear();
            eventContext.Metrics = null;

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
            logger.Log(eventContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackEvent(It.IsAny<EventTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackEvent(It.Is<EventTelemetry>(
                eventTelemetry => eventTelemetry.Name == fakeEventName
                ))), "Wrong event name");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackEvent_WithProperties_WhenEventContextIsPassed()
        {
            #region Arrange
            var fakeEventName = "FAKE TELEMETY MESSAGE";
            var fakeCorrelationId = Guid.NewGuid().ToString();
            var fakeTransactionId = Guid.NewGuid().ToString();
            var fakeE2eTrackingId = Guid.NewGuid().ToString();
            var fakeSource = "TEST";
            var fakeUserId = "tester@microsoft.com";

            var eventContext = new EventContext(fakeEventName, fakeCorrelationId, fakeTransactionId, fakeSource, fakeUserId, fakeE2eTrackingId);

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
            logger.Log(eventContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackEvent(It.IsAny<EventTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackEvent(It.Is<EventTelemetry>(
                eventTelemetry =>
                    eventTelemetry.Name == fakeEventName &&
                    eventTelemetry.Properties != null &&
                    eventTelemetry.Properties["XCV"] == fakeCorrelationId &&
                    eventTelemetry.Properties["MessageId"] == fakeTransactionId &&
                    eventTelemetry.Properties["TrackingId"] == fakeE2eTrackingId &&
                    eventTelemetry.Properties["UserId"] == fakeUserId &&
                    eventTelemetry.Properties["Source"] == fakeSource
                ))), "Message properties not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackEvent_WithPropertiesAndMetrics_WhenEventContextIsPassed()
        {
            #region Arrange
            var fakeEventName = "FAKE TELEMETY MESSAGE";
            var fakeCorrelationId = Guid.NewGuid().ToString();
            var fakeTransactionId = Guid.NewGuid().ToString();
            var fakeE2eTrackingId = Guid.NewGuid().ToString();
            var fakeSource = "TEST";
            var fakeUserId = "tester@microsoft.com";
            var fakeMetricName = "TestMetric_Pi";
            var fakeMetricValue = 3.14159;


            var eventContext = new EventContext(fakeEventName, fakeCorrelationId, fakeTransactionId, fakeSource, fakeUserId, fakeE2eTrackingId);
            eventContext.AddMetric(fakeMetricName, fakeMetricValue);

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
            logger.Log(eventContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackEvent(It.IsAny<EventTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackEvent(It.Is<EventTelemetry>(
                eventTelemetry =>
                    eventTelemetry.Name == fakeEventName &&
                    eventTelemetry.Properties != null &&
                    eventTelemetry.Properties["XCV"] == fakeCorrelationId &&
                    eventTelemetry.Properties["MessageId"] == fakeTransactionId &&
                    eventTelemetry.Properties["TrackingId"] == fakeE2eTrackingId &&
                    eventTelemetry.Properties["UserId"] == fakeUserId &&
                    eventTelemetry.Properties["Source"] == fakeSource
                ))), "Event properties not present in telemetry");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackEvent(It.Is<EventTelemetry>(
                eventTelemetry =>
                    eventTelemetry.Name == fakeEventName &&
                    eventTelemetry.Metrics != null &&
                    eventTelemetry.Metrics[fakeMetricName] == fakeMetricValue
                ))), "Event metrics not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldNotTrackEvent_AndLogInternal_WhenInternalErrorOccurs()
        {
            #region Arrange
            var fakeEventName = "FAKE TELEMETY MESSAGE";
            var fakeExceptionMessage = "FAKE EXCEPTION MESSAGE";
            var eventContext = new EventContext()
            {
                EventName = fakeEventName
            };

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackEvent(It.IsAny<EventTelemetry>()))
                .Throws(new Exception(fakeExceptionMessage)); ;

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(eventContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackEvent(It.IsAny<EventTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.Is<ExceptionTelemetry>(
                exceptionTelemetry => exceptionTelemetry.Exception.Message == fakeExceptionMessage
                ))), "Internal message not logged");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldNotTrackEvent_WhenLogLevelOfEventIsLower()
        {
            #region Arrange
            var fakeEventName = "FAKE TELEMETY MESSAGE";
            var eventContext = new EventContext()
            {
                EventName = fakeEventName
            };

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Critical
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackEvent(It.IsAny<EventTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(eventContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackEvent(It.IsAny<EventTelemetry>())), Times.Never, "Telemetry is sent to Application Insight");
            #endregion Assert
        }


        [TestMethod]
        public void AppInsightsLogger_ShouldTrackEvent_WhenEventContextIsPassed_WithAdditionalProperties()
        {
            #region Arrange
            var fakeEventName = "FAKE TELEMETY MESSAGE";
            var eventContext = new EventContext()
            {
                EventName = fakeEventName
            };

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackEvent(It.IsAny<EventTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());

            var additionalProperties = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("Prop_1", "Val_1"),
                new Tuple<string, string>("Prop_1", "Val_1")
            };
            #endregion Arrange

            #region Act
            logger.Log(eventContext, additionalProperties.ToArray());
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackEvent(It.IsAny<EventTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackEvent(It.Is<EventTelemetry>(
                eventTelemetry => eventTelemetry.Name == fakeEventName
                    && eventTelemetry.Properties.ContainsKey(additionalProperties[0].Item1)
                    && eventTelemetry.Properties.ContainsKey(additionalProperties[1].Item1)
                ))), "Additional properties are missing");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldLogInternalException_WhenEventContextFails_WithAdditionalProperties()
        {
            #region Arrange
            var fakeEventName = "FAKE TELEMETY MESSAGE";
            var eventContext = new EventContext()
            {
                EventName = fakeEventName
            };

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackEvent(It.IsAny<EventTelemetry>()))
                .Throws(new Exception("FAKE EXCEPTION"));
            stubIAppInsightsTelemetryClientWrapper.Setup(stubClient => stubClient.TrackException(It.IsAny<ExceptionTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());

            var additionalProperties = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("Prop_1", "Val_1"),
                new Tuple<string, string>("Prop_1", "Val_1")
            };
            #endregion Arrange

            #region Act
            logger.Log(eventContext, additionalProperties.ToArray());
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackEvent(It.IsAny<EventTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
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
