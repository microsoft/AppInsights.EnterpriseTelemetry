using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppInsights.EnterpriseTelemetry.Client;
using AppInsights.EnterpriseTelemetry.Configurations;
using AppInsights.EnterpriseTelemetry.Context;
using Microsoft.AspNetCore.Http;

namespace AppInsights.EnterpriseTelemetry.Tests.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class AppInsightsMessageLoggerTests
    {   
        [TestMethod]
        public void AppInsightsLogger_ShouldTrackTrace_WhenMessageContextIsPassed()
        {
            #region Arrange
            var fakeMessage = "FAKE TELEMETY MESSAGE";
            var messageContext = new MessageContext()
            {
                Message = fakeMessage
            };

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackTrace(It.IsAny<TraceTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(messageContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.IsAny<TraceTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.Is<TraceTelemetry>(traceTelemetry => traceTelemetry.Message == fakeMessage))), "Message not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackTrace_WhenMessageContextIsPassed_WithNullProperties()
        {
            #region Arrange
            var fakeMessage = "FAKE TELEMETY MESSAGE";
            var messageContext = new MessageContext()
            {
                Message = fakeMessage
            };
            messageContext.Properties.Clear();

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackTrace(It.IsAny<TraceTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(messageContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.IsAny<TraceTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.Is<TraceTelemetry>(traceTelemetry => traceTelemetry.Message == fakeMessage))), "Message not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackTrace_WithProperties_WhenMessageContextIsPassed()
        {
            #region Arrange
            var fakeMessage = "FAKE TELEMETY MESSAGE";
            var fakeCorrelationId = Guid.NewGuid().ToString();
            var fakeTransactionId = Guid.NewGuid().ToString();
            var fakeE2eTrackingId = Guid.NewGuid().ToString();
            var fakeSource = "TEST";
            var fakeUserId = "tester@microsoft.com";

            var messageContext = new MessageContext(fakeMessage, TraceLevel.Information, fakeCorrelationId, fakeTransactionId, fakeSource, fakeUserId, fakeE2eTrackingId);

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var expectedTelemetryObject = new TraceTelemetry(fakeMessage, SeverityLevel.Verbose);

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackTrace(It.IsAny<TraceTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(messageContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.IsAny<TraceTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.Is<TraceTelemetry>(
                traceTelemetry =>
                    traceTelemetry.Message == fakeMessage &&
                    traceTelemetry.Properties != null &&
                    traceTelemetry.Properties["XCV"] == fakeCorrelationId &&
                    traceTelemetry.Properties["MessageId"] == fakeTransactionId &&
                    traceTelemetry.Properties["TrackingId"] == fakeE2eTrackingId &&
                    traceTelemetry.Properties["UserId"] == fakeUserId &&
                    traceTelemetry.Properties["Source"] == fakeSource 
                ))), "Message properties not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackTrace_WhenSimpleMessageIsPassed()
        {
            #region Arrange
            var fakeMessage = "FAKE TELEMETY MESSAGE";
            var fakeCorrelationId = Guid.NewGuid().ToString();
            var fakeTransactionId = Guid.NewGuid().ToString();
            var fakeE2eTrackingId = Guid.NewGuid().ToString();
            var fakeSource = "TEST";
            var fakeUserId = "tester@microsoft.com";

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var expectedTelemetryObject = new TraceTelemetry(fakeMessage, SeverityLevel.Verbose);

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackTrace(It.IsAny<TraceTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(fakeMessage, fakeCorrelationId, fakeTransactionId, fakeSource, fakeUserId, fakeE2eTrackingId);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.IsAny<TraceTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.Is<TraceTelemetry>(
                traceTelemetry =>
                    traceTelemetry.Message == fakeMessage &&
                    traceTelemetry.Properties != null &&
                    traceTelemetry.Properties["XCV"] == fakeCorrelationId &&
                    traceTelemetry.Properties["MessageId"] == fakeTransactionId &&
                    traceTelemetry.Properties["TrackingId"] == fakeE2eTrackingId &&
                    traceTelemetry.Properties["UserId"] == fakeUserId &&
                    traceTelemetry.Properties["Source"] == fakeSource
                ))), "Message properties not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackTrace_AtCriticalLevel_WhenMessageContextIsPassed()
        {
            #region Arrange
            var fakeMessage = "FAKE TELEMETY MESSAGE";
            var messageContext = new MessageContext(fakeMessage, TraceLevel.Critical, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Test", "System", Guid.NewGuid().ToString());

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackTrace(It.IsAny<TraceTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(messageContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.IsAny<TraceTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.Is<TraceTelemetry>(
                traceTelemetry => 
                traceTelemetry.Message == fakeMessage &&
                traceTelemetry.SeverityLevel == SeverityLevel.Critical
                ))), "Message not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackTrace_AtErrorLevel_WhenMessageContextIsPassed()
        {
            #region Arrange
            var fakeMessage = "FAKE TELEMETY MESSAGE";
            var messageContext = new MessageContext(fakeMessage, TraceLevel.Error, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Test", "System", Guid.NewGuid().ToString());

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackTrace(It.IsAny<TraceTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(messageContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.IsAny<TraceTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.Is<TraceTelemetry>(
                traceTelemetry =>
                traceTelemetry.Message == fakeMessage &&
                traceTelemetry.SeverityLevel == SeverityLevel.Error
                ))), "Message not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackTrace_AtWarningLevel_WhenMessageContextIsPassed()
        {
            #region Arrange
            var fakeMessage = "FAKE TELEMETY MESSAGE";
            var messageContext = new MessageContext(fakeMessage, TraceLevel.Warning, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Test", "System", Guid.NewGuid().ToString());

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackTrace(It.IsAny<TraceTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(messageContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.IsAny<TraceTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.Is<TraceTelemetry>(
                traceTelemetry =>
                traceTelemetry.Message == fakeMessage &&
                traceTelemetry.SeverityLevel == SeverityLevel.Warning
                ))), "Message not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackTrace_AtInformationLevel_WhenMessageContextIsPassed()
        {
            #region Arrange
            var fakeMessage = "FAKE TELEMETY MESSAGE";
            var messageContext = new MessageContext(fakeMessage, TraceLevel.Information, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Test", "System", Guid.NewGuid().ToString());

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackTrace(It.IsAny<TraceTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(messageContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.IsAny<TraceTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.Is<TraceTelemetry>(
                traceTelemetry =>
                traceTelemetry.Message == fakeMessage &&
                traceTelemetry.SeverityLevel == SeverityLevel.Information
                ))), "Message not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackTrace_AtVerboseLevel_WhenMessageContextIsPassed()
        {
            #region Arrange
            var fakeMessage = "FAKE TELEMETY MESSAGE";
            var messageContext = new MessageContext(fakeMessage, TraceLevel.Verbose, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Test", "System", Guid.NewGuid().ToString());

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackTrace(It.IsAny<TraceTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(messageContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.IsAny<TraceTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.Is<TraceTelemetry>(
                traceTelemetry =>
                traceTelemetry.Message == fakeMessage &&
                traceTelemetry.SeverityLevel == SeverityLevel.Verbose
                ))), "Message not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldNotTrackTrace_AndLogInternal_WhenInternalErrorOccurs()
        {
            #region Arrange
            var fakeMessage = "FAKE TELEMETY MESSAGE";
            var fakeExceptionMessage = "FAKE EXCEPTION MESSAGE";
            var messageContext = new MessageContext()
            {
                Message = fakeMessage
            };

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var expectedTelemetryObject = new TraceTelemetry(fakeMessage, SeverityLevel.Verbose);

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackTrace(It.IsAny<TraceTelemetry>()))
                .Throws(new Exception(fakeExceptionMessage));
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackException(It.IsAny<ExceptionTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(messageContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.IsAny<TraceTelemetry>())), Times.Once);
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.Is<ExceptionTelemetry>(
                exceptionTelemetry => exceptionTelemetry.Exception.Message == fakeExceptionMessage
                ))), "Internal message not logged");

            #endregion Assert
        }

        

        [TestMethod]
        public void AppInsightsLogger_ShouldNotTrackTrace_WhenLogLevelOfMessageIsLower()
        {
            #region Arrange
            var fakeMessage = "FAKE TELEMETY MESSAGE";
            var messageContext = new MessageContext()
            {
                Message = fakeMessage
            };

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Error
            };

            var expectedTelemetryObject = new TraceTelemetry(fakeMessage, SeverityLevel.Verbose);

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackTrace(It.IsAny<TraceTelemetry>()));
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackException(It.IsAny<ExceptionTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(messageContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackTrace(It.IsAny<TraceTelemetry>())), Times.Never);
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.IsAny<ExceptionTelemetry>())), Times.Never);
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
