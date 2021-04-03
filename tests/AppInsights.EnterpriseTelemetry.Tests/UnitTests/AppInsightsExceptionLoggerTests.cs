using Moq;
using System;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using AppInsights.EnterpriseTelemetry.Client;
using AppInsights.EnterpriseTelemetry.Context;
using AppInsights.EnterpriseTelemetry.Tests.Mocks;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppInsights.EnterpriseTelemetry.Configurations;

namespace AppInsights.EnterpriseTelemetry.Tests.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class AppInsightsExceptionLoggerTests
    {
        [TestMethod]
        public void AppInsightsLogger_ShouldTrackException_WhenExceptionContextIsPassed()
        {
            #region Arrange
            var fakeMessage = "FAKE EXCEPTION MESSAGE";
            var exceptionContext = new ExceptionContext()
            {
                Exception = new Exception(fakeMessage)
            };

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackException(It.IsAny<ExceptionTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(exceptionContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.IsAny<ExceptionTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.Is<ExceptionTelemetry>(
                exceptionTelemetry => exceptionTelemetry.Exception.Message == fakeMessage))), "Message not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackException_WhenExceptionContextIsPassed_WithNullProperties()
        {
            #region Arrange
            var fakeMessage = "FAKE EXCEPTION MESSAGE";
            var exceptionContext = new ExceptionContext()
            {
                Exception = new Exception(fakeMessage)
            };
            exceptionContext.Properties.Clear();

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackException(It.IsAny<ExceptionTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(exceptionContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.IsAny<ExceptionTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.Is<ExceptionTelemetry>(
                exceptionTelemetry => exceptionTelemetry.Exception.Message == fakeMessage))), "Message not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackException_WithProperties_WhenMessageContextIsPassed()
        {
            #region Arrange
            var fakeMessage = "FAKE EXCEPTION MESSAGE";
            var fakeCorrelationId = Guid.NewGuid().ToString();
            var fakeTransactionId = Guid.NewGuid().ToString();
            var fakeE2eTrackingId = Guid.NewGuid().ToString();
            var fakeSource = "TEST";
            var fakeUserId = "tester@microsoft.com";
            var exceptionContext = new ExceptionContext(new Exception(fakeMessage), TraceLevel.Error, fakeCorrelationId, fakeTransactionId, fakeSource, fakeUserId, fakeE2eTrackingId)
            {
                Exception = new Exception(fakeMessage)
            };

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackException(It.IsAny<ExceptionTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(exceptionContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.IsAny<ExceptionTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.Is<ExceptionTelemetry>(
                exceptionTelemetry =>
                    exceptionTelemetry.Exception.Message == fakeMessage &&
                    exceptionTelemetry.Properties != null &&
                    exceptionTelemetry.Properties["XCV"] == fakeCorrelationId &&
                    exceptionTelemetry.Properties["MessageId"] == fakeTransactionId &&
                    exceptionTelemetry.Properties["TrackingId"] == fakeE2eTrackingId &&
                    exceptionTelemetry.Properties["UserId"] == fakeUserId &&
                    exceptionTelemetry.Properties["Source"] == fakeSource
                ))), "Message properties not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackException_WhenSystemExceptionIsPassed()
        {
            #region Arrange
            var fakeMessage = "FAKE EXCEPTION MESSAGE";
            var fakeCorrelationId = Guid.NewGuid().ToString();
            var fakeTransactionId = Guid.NewGuid().ToString();
            var fakeE2eTrackingId = Guid.NewGuid().ToString();
            var fakeSource = "TEST";
            var fakeUserId = "tester@microsoft.com";
            var exception = new Exception(fakeMessage);

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackException(It.IsAny<ExceptionTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(exception, fakeCorrelationId, fakeTransactionId, fakeSource, fakeUserId, fakeE2eTrackingId);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.IsAny<ExceptionTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.Is<ExceptionTelemetry>(
                exceptionTelemetry =>
                    exceptionTelemetry.Exception.Message == fakeMessage &&
                    exceptionTelemetry.Properties != null &&
                    exceptionTelemetry.Properties["XCV"] == fakeCorrelationId &&
                    exceptionTelemetry.Properties["MessageId"] == fakeTransactionId &&
                    exceptionTelemetry.Properties["TrackingId"] == fakeE2eTrackingId &&
                    exceptionTelemetry.Properties["UserId"] == fakeUserId &&
                    exceptionTelemetry.Properties["Source"] == fakeSource
                ))), "Message properties not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackException_AsAppException_WhenAppExceptionIsPassed()
        {
            #region Arrange
            var fakeMessage = "FAKE EXCEPTION MESSAGE";
            var fakeCorrelationId = Guid.NewGuid().ToString();
            var fakeTransactionId = Guid.NewGuid().ToString();
            var fakeE2eTrackingId = Guid.NewGuid().ToString();
            var fakeSource = "TEST";
            var fakeUserId = "tester@microsoft.com";
            var exception = new CustomAppException(fakeMessage, fakeCorrelationId, fakeTransactionId, "101", fakeSource);

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackException(It.IsAny<ExceptionTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(exception, correlationId:fakeCorrelationId, transactionId: fakeTransactionId, source: fakeSource, userId: fakeUserId, e2eTrackingId: fakeE2eTrackingId);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.IsAny<ExceptionTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.Is<ExceptionTelemetry>(
                exceptionTelemetry =>
                    exceptionTelemetry.Exception.Message == fakeMessage &&
                    exceptionTelemetry.Properties != null &&
                    exceptionTelemetry.Properties["XCV"] == fakeCorrelationId &&
                    exceptionTelemetry.Properties["MessageId"] == fakeTransactionId &&
                    exceptionTelemetry.Properties["TrackingId"] == fakeE2eTrackingId &&
                    exceptionTelemetry.Properties["UserId"] == fakeUserId &&
                    exceptionTelemetry.Properties["Source"] == fakeSource
                ))), "Message properties not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldTrackException_WhenAppExceptionIsPassed()
        {
            #region Arrange
            var fakeMessage = "FAKE EXCEPTION MESSAGE";
            var fakeCorrelationId = Guid.NewGuid().ToString();
            var fakeTransactionId = Guid.NewGuid().ToString();
            var fakeE2eTrackingId = Guid.NewGuid().ToString();
            var fakeSource = "TEST";
            var fakeUserId = "tester@microsoft.com";
            var exception = new CustomAppException(fakeMessage, fakeCorrelationId, fakeTransactionId, "101", fakeSource);

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackException(It.IsAny<ExceptionTelemetry>()));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(exception, userId: fakeUserId, e2eTrackingId: fakeE2eTrackingId);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.IsAny<ExceptionTelemetry>())), Times.Once, "Telemetry not sent to Application Insight");
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.Is<ExceptionTelemetry>(
                exceptionTelemetry =>
                    exceptionTelemetry.Exception.Message == fakeMessage &&
                    exceptionTelemetry.Properties != null &&
                    exceptionTelemetry.Properties["XCV"] == fakeCorrelationId &&
                    exceptionTelemetry.Properties["MessageId"] == fakeTransactionId &&
                    exceptionTelemetry.Properties["TrackingId"] == fakeE2eTrackingId &&
                    exceptionTelemetry.Properties["UserId"] == fakeUserId &&
                    exceptionTelemetry.Properties["Source"] == fakeSource
                ))), "Message properties not present in telemetry");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldNotTrackException_AndLogInternal_WhenInternalErrorOccurs()
        {
            #region Arrange
            var fakeMessage = "FAKE EXCEPTION MESSAGE";
            var exceptionContext = new ExceptionContext()
            {
                Exception = new Exception(fakeMessage)
            };

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackException(It.IsAny<ExceptionTelemetry>()))
                .Throws(new Exception("Fake"));

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(exceptionContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.IsAny<ExceptionTelemetry>())), Times.Exactly(2), "Telemetry not sent to Application Insight");
            #endregion Assert
        }

        [TestMethod]
        public void AppInsightsLogger_ShouldNotTrackException_WhenLogLevelOfExceptionIsLower()
        {
            #region Arrange
            var fakeMessage = "FAKE EXCEPTION MESSAGE";
            var exceptionContext = new ExceptionContext()
            {
                Exception = new Exception(fakeMessage),
                TraceLevel = TraceLevel.Error
            };

            var fakeConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Critical
            };

            var stubIAppInsightsTelemetryClientWrapper = new Mock<IAppInsightsTelemetryClientWrapper>();
            stubIAppInsightsTelemetryClientWrapper.Setup(client => client.TrackException(It.IsAny<ExceptionTelemetry>()));
                

            var logger = new ApplicationInsightsLogger(stubIAppInsightsTelemetryClientWrapper.Object, fakeConfiguration, CreateMockContextPropertyBuilder(), CreateMockHttpContextAccessor());
            #endregion Arrange

            #region Act
            logger.Log(exceptionContext);
            #endregion Act

            #region Assert
            stubIAppInsightsTelemetryClientWrapper.Verify((stubClient => stubClient.TrackException(It.IsAny<ExceptionTelemetry>())), Times.Never, "Telemetry not sent to Application Insight");
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
