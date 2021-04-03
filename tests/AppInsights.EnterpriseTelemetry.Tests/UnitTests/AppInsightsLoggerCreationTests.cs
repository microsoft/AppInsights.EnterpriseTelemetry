using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppInsights.EnterpriseTelemetry.Configurations;

namespace AppInsights.EnterpriseTelemetry.Tests.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class AppInsightsLoggerCreationTests
    {
        [TestMethod]
        public void ApplicationInsights_ShouldCreateTelemetryWrapper_WhenConstructorIsCalled()
        {
            #region Arrange
            var fakeAppInsightsConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = "",
                LogLevel = TraceLevel.Verbose
            };
            var fakeAppMetadataConfiguration = new AppMetadataConfiguration()
            {
                BusinessProcessHeaderKey = "x-business-context",
                CorrelationIdHeaderKey = "x-correlationId",
                EndToEndTrackingHeaderKey = "x-e2e-id",
                SubCorrIdHeaderKey = "x-sub-xcv",
                TenantIdHeaderKey = "x-ms-tenants",
                TransactionIdHeaderKey = "x-messageId"
            };
            #endregion Arrange

            #region Act
            var appInsightsLogger = new ApplicationInsightsLogger(fakeAppInsightsConfiguration, fakeAppMetadataConfiguration);
            #endregion Act

            #region Assert
            Assert.IsNotNull(appInsightsLogger);
            #endregion Assert
        }
    }
}
