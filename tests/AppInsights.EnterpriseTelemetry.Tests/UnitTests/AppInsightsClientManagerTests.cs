using System.Diagnostics.CodeAnalysis;
using AppInsights.EnterpriseTelemetry.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppInsights.EnterpriseTelemetry.Configurations;

namespace AppInsights.EnterpriseTelemetry.Tests.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class AppInsightsClientManagerTests
    {
        [TestMethod]
        public void AppInsightsClientManager_ShouldCreateAppInsightsClientWrapper()
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
                EndToEndTrackingHeaderKey = "x-e2e",
                SubCorrIdHeaderKey = "x-sub-xcv",
                TenantIdHeaderKey = "x-ms-tenants",
                TransactionIdHeaderKey = "x-messageId"
            };
            var clientManager = new AppInsightsClientManager();
            #endregion Arrange

            #region Act
            var clientWrapper = clientManager.CreateClient(fakeAppInsightsConfiguration, fakeAppMetadataConfiguration);
            #endregion Act

            #region Assert
            Assert.IsNotNull(clientWrapper);
            #endregion Assert

        }
    }
}
