using Moq;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppInsights.EnterpriseTelemetry.Configurations;
using AppInsights.EnterpriseTelemetry.Context.Background;
using AppInsights.EnterpriseTelemetry.AppInsightsInitializers;

namespace AppInsights.EnterpriseTelemetry.Tests.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class TrackingInitializerTests
    {
        [TestMethod]
        public void TrackingInitializer_ShouldNotChangeTrackingIds_WhenHttpContextDoesntHaveTrackingIds()
        {
            #region Arrange
            var mockCorrelationId = Guid.NewGuid().ToString();
            var mockTransactionId = Guid.NewGuid().ToString();
            var mockE2Eid = Guid.NewGuid().ToString();
            var mockTenantId = "MOCK-TEN";
            var mockAppInsightsConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = Guid.NewGuid().ToString()
            };
            var mockAppConfiguration = new AppMetadataConfiguration()
            {
                CorrelationIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_CORRELATION_KEY,
                TransactionIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TRANSACTION_KEY,
                EndToEndTrackingHeaderKey = TelemetryConstant.HEADER_DEFAULT_E2E_KEY,
                TenantIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TENANT_ID
            };
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "Mock-Request"
            };
            requestTelemetry.Properties.Add(TelemetryConstant.CORRELATION_KEY, mockCorrelationId);
            requestTelemetry.Properties.Add(TelemetryConstant.TRANSACTION_KEY, mockTransactionId);
            requestTelemetry.Properties.Add(TelemetryConstant.E2E_KEY, mockE2Eid);
            requestTelemetry.Properties.Add(TelemetryConstant.TENANT_KEY, mockTenantId);
            #endregion Arrange

            #region Act
            var initializer = new TrackingInitializer(CreateMockHttpContextAccessor(), mockAppInsightsConfiguration, mockAppConfiguration);
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Asset
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.CORRELATION_KEY], mockCorrelationId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY], mockTransactionId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.E2E_KEY], mockE2Eid);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY], mockTenantId);
            #endregion Asset
        }

        #region HTTP Context Tests
        [TestMethod]
        public void TrackingInitializer_ShouldNotChangeTrackingIds_WhenHttpContextHaveSameTrackingIds()
        {
            #region Arrange
            var mockCorrelationId = Guid.NewGuid().ToString();
            var mockTransactionId = Guid.NewGuid().ToString();
            var mockE2Eid = Guid.NewGuid().ToString();
            var mockTenantId = "MOCK-TEN";
            var mockAppInsightsConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = Guid.NewGuid().ToString()
            };
            var mockAppConfiguration = new AppMetadataConfiguration()
            {
                CorrelationIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_CORRELATION_KEY,
                TransactionIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TRANSACTION_KEY,
                EndToEndTrackingHeaderKey = TelemetryConstant.HEADER_DEFAULT_E2E_KEY,
                TenantIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TENANT_ID
            };
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "Mock-Request"
            };
            requestTelemetry.Properties.Add(TelemetryConstant.CORRELATION_KEY, mockCorrelationId);
            requestTelemetry.Properties.Add(TelemetryConstant.TRANSACTION_KEY, mockTransactionId);
            requestTelemetry.Properties.Add(TelemetryConstant.E2E_KEY, mockE2Eid);
            requestTelemetry.Properties.Add(TelemetryConstant.TENANT_KEY, mockTenantId);

            var mockHttpContext = CreateHttpContextFromTrackingIds(mockAppConfiguration, mockCorrelationId, mockTransactionId, mockE2Eid);
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(accessor => accessor.HttpContext)
                .Returns(mockHttpContext);
            #endregion Arrange

            #region Act
            var initializer = new TrackingInitializer(stubIHttpContextAccessor.Object, mockAppInsightsConfiguration, mockAppConfiguration);
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Asset
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.CORRELATION_KEY], mockCorrelationId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY], mockTransactionId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.E2E_KEY], mockE2Eid);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY], mockTenantId);
            #endregion Asset
        }

        [TestMethod]
        public void TrackingInitializer_ShouldUpdateTrackingIds_WhenHttpContextHaveDifferntTrackingIds()
        {
            #region Arrange
            var mockCorrelationId = Guid.NewGuid().ToString();
            var mockTransactionId = Guid.NewGuid().ToString();
            var mockE2Eid = Guid.NewGuid().ToString();
            var mockTenantId = "MOCK-TEN";
            var mockAppInsightsConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = Guid.NewGuid().ToString()
            };
            var mockAppConfiguration = new AppMetadataConfiguration()
            {
                CorrelationIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_CORRELATION_KEY,
                TransactionIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TRANSACTION_KEY,
                EndToEndTrackingHeaderKey = TelemetryConstant.HEADER_DEFAULT_E2E_KEY,
                TenantIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TENANT_ID
            };
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "Mock-Request"
            };
            requestTelemetry.Properties.Add(TelemetryConstant.CORRELATION_KEY, mockCorrelationId);
            requestTelemetry.Properties.Add(TelemetryConstant.TRANSACTION_KEY, mockTransactionId);
            requestTelemetry.Properties.Add(TelemetryConstant.E2E_KEY, mockE2Eid);
            requestTelemetry.Properties.Add(TelemetryConstant.TENANT_KEY, mockTenantId);

            var contextCorrelationId = Guid.NewGuid().ToString();
            var contextTransactionId = Guid.NewGuid().ToString();
            var contextE2EId = Guid.NewGuid().ToString();
            var contextTenantId = Guid.NewGuid().ToString();
            var mockHttpContext = CreateHttpContextFromTrackingIds(mockAppConfiguration, contextCorrelationId, contextTransactionId, contextE2EId);
            mockHttpContext.Request.Headers.Add(mockAppConfiguration.TenantIdHeaderKey, contextTenantId);
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(accessor => accessor.HttpContext)
                .Returns(mockHttpContext);
            #endregion Arrange

            #region Act
            var initializer = new TrackingInitializer(stubIHttpContextAccessor.Object, mockAppInsightsConfiguration, mockAppConfiguration);
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Asset
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.CORRELATION_KEY], contextCorrelationId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY], contextTransactionId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.E2E_KEY], contextE2EId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY], contextTenantId);

            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.CORRELATION_KEY + ":Alternate"], mockCorrelationId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY + ":Alternate"], mockTransactionId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.E2E_KEY + ":Alternate"], mockE2Eid);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY + ":Alternate"], mockTenantId);
            #endregion Asset
        }

        [TestMethod]
        public void TrackingInitializer_ShouldUpdateTrackingIds_WhenHttpContextHaveTrackings_ButTelemetryContextDontHaveIds()
        {
            #region Arrange
            var mockAppInsightsConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = Guid.NewGuid().ToString()
            };
            var mockAppConfiguration = new AppMetadataConfiguration()
            {
                CorrelationIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_CORRELATION_KEY,
                TransactionIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TRANSACTION_KEY,
                EndToEndTrackingHeaderKey = TelemetryConstant.HEADER_DEFAULT_E2E_KEY,
                TenantIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TENANT_ID
            };
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "Mock-Request"
            };

            var contextCorrelationId = Guid.NewGuid().ToString();
            var contextTransactionId = Guid.NewGuid().ToString();
            var contextE2EId = Guid.NewGuid().ToString();
            var contextTenantId = Guid.NewGuid().ToString();
            var mockHttpContext = CreateHttpContextFromTrackingIds(mockAppConfiguration, contextCorrelationId, contextTransactionId, contextE2EId);
            mockHttpContext.Request.Headers.Add(mockAppConfiguration.TenantIdHeaderKey, contextTenantId);
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(accessor => accessor.HttpContext)
                .Returns(mockHttpContext);
            #endregion Arrange

            #region Act
            var initializer = new TrackingInitializer(stubIHttpContextAccessor.Object, mockAppInsightsConfiguration, mockAppConfiguration);
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Asset
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.CORRELATION_KEY], contextCorrelationId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY], contextTransactionId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.E2E_KEY], contextE2EId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY], contextTenantId);
            #endregion Asset
        }

        [TestMethod]
        public void TrackingInitializer_ShouldUpdateTrackingIdsFromHttpContext_WhenBothBackgroundContextAndHttpContextIsPresent()
        {
            #region Arrange
            var mockCorrelationId = Guid.NewGuid().ToString();
            var mockTransactionId = Guid.NewGuid().ToString();
            var mockE2Eid = Guid.NewGuid().ToString();
            var mockTenantId = "MOCK-TEN";
            var mockAppInsightsConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = Guid.NewGuid().ToString()
            };
            var mockAppConfiguration = new AppMetadataConfiguration()
            {
                CorrelationIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_CORRELATION_KEY,
                TransactionIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TRANSACTION_KEY,
                EndToEndTrackingHeaderKey = TelemetryConstant.HEADER_DEFAULT_E2E_KEY,
                TenantIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TENANT_ID
            };
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "Mock-Request"
            };
            requestTelemetry.Properties.Add(TelemetryConstant.CORRELATION_KEY, mockCorrelationId);
            requestTelemetry.Properties.Add(TelemetryConstant.TRANSACTION_KEY, mockTransactionId);
            requestTelemetry.Properties.Add(TelemetryConstant.E2E_KEY, mockE2Eid);
            requestTelemetry.Properties.Add(TelemetryConstant.TENANT_KEY, mockTenantId);

            var contextExecutionId = Guid.NewGuid().ToString();
            var contextTransactionId = Guid.NewGuid().ToString();
            var contextE2EId = Guid.NewGuid().ToString();
            var contextUserId = "mocker@microsoft.com";
            var contextTenantId = Guid.NewGuid().ToString();
            var mockHttpContext = CreateHttpContextFromTrackingIds(mockAppConfiguration, mockCorrelationId, contextTransactionId, contextE2EId);

            AddBackgroundContext(contextExecutionId, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), contextUserId, Guid.NewGuid().ToString());

            mockHttpContext.Request.Headers.Add(mockAppConfiguration.TenantIdHeaderKey, contextTenantId);
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(accessor => accessor.HttpContext)
                .Returns(mockHttpContext);
            #endregion Arrange

            #region Act
            var initializer = new TrackingInitializer(stubIHttpContextAccessor.Object, mockAppInsightsConfiguration, mockAppConfiguration);
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Asset
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.CORRELATION_KEY], mockCorrelationId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY], contextTransactionId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.E2E_KEY], contextE2EId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY], contextTenantId);

            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY + ":Alternate"], mockTransactionId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.E2E_KEY + ":Alternate"], mockE2Eid);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY + ":Alternate"], mockTenantId);
            #endregion Asset
        }

        [TestMethod]
        public void TrackingInitializer_ShouldAddDefaultTrackingIds_WhenNullIdIsPresentInContextAndTelemetry()
        {
            #region Arrange
            var mockAppConfiguration = new AppMetadataConfiguration()
            {
                CorrelationIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_CORRELATION_KEY,
                TransactionIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TRANSACTION_KEY,
                EndToEndTrackingHeaderKey = TelemetryConstant.HEADER_DEFAULT_E2E_KEY,
                TenantIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TENANT_ID
            };
            var mockAppInsightsConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = Guid.NewGuid().ToString()
            };
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "Mock-Request"
            };
            requestTelemetry.Properties.Add(TelemetryConstant.CORRELATION_KEY, null);
            requestTelemetry.Properties.Add(TelemetryConstant.TRANSACTION_KEY, null);
            requestTelemetry.Properties.Add(TelemetryConstant.E2E_KEY, null);
            requestTelemetry.Properties.Add(TelemetryConstant.TENANT_KEY, null);

            var mockHttpContext = CreateHttpContextFromTrackingIds(mockAppConfiguration, null, null, null);
            mockHttpContext.Request.Headers.Add(mockAppConfiguration.TenantIdHeaderKey, string.Empty);
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(accessor => accessor.HttpContext)
                .Returns(mockHttpContext);
            #endregion Arrange

            #region Act
            var initializer = new TrackingInitializer(stubIHttpContextAccessor.Object, mockAppInsightsConfiguration, mockAppConfiguration);
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Asset
            Assert.IsNotNull(requestTelemetry.Properties[TelemetryConstant.CORRELATION_KEY]);
            Assert.IsNotNull(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY]);
            Assert.IsNotNull(requestTelemetry.Properties[TelemetryConstant.E2E_KEY]);
            Assert.IsNotNull(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY]);
            #endregion Asset
        }

        [TestMethod]
        public void TrackingInitializer_ShouldAddDefaultTrackingIds_WhenKeysAreMissingFromTelemetryAndHttpContext()
        {
            #region Arrange
            var mockAppInsightsConfiguration = new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = Guid.NewGuid().ToString()
            };
            var mockAppConfiguration = new AppMetadataConfiguration()
            {
                CorrelationIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_CORRELATION_KEY,
                TransactionIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TRANSACTION_KEY,
                EndToEndTrackingHeaderKey = TelemetryConstant.HEADER_DEFAULT_E2E_KEY,
                TenantIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TENANT_ID
            };
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "Mock-Request"
            };

            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Headers.Add(mockAppConfiguration.TenantIdHeaderKey, string.Empty);
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(accessor => accessor.HttpContext)
                .Returns(mockHttpContext);
            #endregion Arrange

            #region Act
            var initializer = new TrackingInitializer(stubIHttpContextAccessor.Object, mockAppInsightsConfiguration, mockAppConfiguration);
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Asset
            Assert.IsNotNull(requestTelemetry.Properties[TelemetryConstant.CORRELATION_KEY]);
            Assert.IsNotNull(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY]);
            Assert.IsNotNull(requestTelemetry.Properties[TelemetryConstant.E2E_KEY]);
            Assert.IsNotNull(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY]);
            #endregion Asset
        }
        #endregion HTTP Context Tests

        #region Background Context Tests
        [TestMethod]
        public void TrackingInitializer_ShouldUpdateTrackingIds_WhenBackgroundContextHasDifferentTrackingIds_WithHttpContextBeingNull()
        {
            #region Arrange
            var mockCorrelationId = Guid.NewGuid().ToString();
            var mockTransactionId = Guid.NewGuid().ToString();
            var mockE2Eid = Guid.NewGuid().ToString();
            var mockTenantId = "MOCK-TEN";
            var mockAppConfiguration = new AppMetadataConfiguration()
            {
                CorrelationIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_CORRELATION_KEY,
                TransactionIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TRANSACTION_KEY,
                EndToEndTrackingHeaderKey = TelemetryConstant.HEADER_DEFAULT_E2E_KEY,
                TenantIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TENANT_ID
            };
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "Mock-Request"
            };
            requestTelemetry.Properties.Add(TelemetryConstant.CORRELATION_KEY, mockCorrelationId);
            requestTelemetry.Properties.Add(TelemetryConstant.TRANSACTION_KEY, mockTransactionId);
            requestTelemetry.Properties.Add(TelemetryConstant.E2E_KEY, mockE2Eid);
            requestTelemetry.Properties.Add(TelemetryConstant.TENANT_KEY, mockTenantId);

            var contextExecutionId = Guid.NewGuid().ToString();
            var contextTransactionId = Guid.NewGuid().ToString();
            var contextE2EId = Guid.NewGuid().ToString();
            var contextUserId = "mocker@microsoft.com";
            var contextTenantId = Guid.NewGuid().ToString();
            var mockHttpContext = CreateHttpContextFromTrackingIds(mockAppConfiguration, null, null, null);

            AddBackgroundContext(contextExecutionId, mockCorrelationId, contextTransactionId, contextE2EId, contextUserId, contextTenantId);
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(accessor => accessor.HttpContext)
                .Returns(mockHttpContext);
            #endregion Arrange

            #region Act
            var initializer = new TrackingInitializer(stubIHttpContextAccessor.Object, CreateMockAppInsightsConfiguration(), mockAppConfiguration);
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Asset
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.CORRELATION_KEY], mockCorrelationId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY], contextTransactionId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.E2E_KEY], contextE2EId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY], contextTenantId);

            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY + ":Alternate"], mockTransactionId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.E2E_KEY + ":Alternate"], mockE2Eid);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY + ":Alternate"], mockTenantId);
            #endregion Asset
        }

        [TestMethod]
        public void TrackingInitializer_ShouldUpdateTrackingIds_WhenBackgroundContextHaveTrackingIds_ButTelemetryContextDoesNotHaveTrackingIds()
        {
            #region Arrange
            var mockCorrelationId = Guid.NewGuid().ToString();
            var mockTransactionId = Guid.NewGuid().ToString();
            var mockE2Eid = Guid.NewGuid().ToString();
            var mockAppConfiguration = new AppMetadataConfiguration()
            {
                CorrelationIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_CORRELATION_KEY,
                TransactionIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TRANSACTION_KEY,
                EndToEndTrackingHeaderKey = TelemetryConstant.HEADER_DEFAULT_E2E_KEY,
                TenantIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TENANT_ID
            };
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "Mock-Request"
            };
            requestTelemetry.Properties.Add(TelemetryConstant.CORRELATION_KEY, mockCorrelationId);

            var contextExecutionId = Guid.NewGuid().ToString();
            var contextTransactionId = Guid.NewGuid().ToString();
            var contextE2EId = Guid.NewGuid().ToString();
            var contextUserId = "mocker@microsoft.com";
            var contextTenantId = Guid.NewGuid().ToString();
            var mockHttpContext = CreateHttpContextFromTrackingIds(mockAppConfiguration, null, null, null);

            AddBackgroundContext(contextExecutionId, mockCorrelationId, contextTransactionId, contextE2EId, contextUserId, contextTenantId);

            mockHttpContext.Request.Headers.Add(mockAppConfiguration.TenantIdHeaderKey, contextTenantId);
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(accessor => accessor.HttpContext)
                .Returns(mockHttpContext);
            #endregion Arrange

            #region Act
            var initializer = new TrackingInitializer(stubIHttpContextAccessor.Object, CreateMockAppInsightsConfiguration(), mockAppConfiguration);
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Asset
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.CORRELATION_KEY], mockCorrelationId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY], contextTransactionId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.E2E_KEY], contextE2EId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY], contextTenantId);
            #endregion Asset
        }

        [TestMethod]
        public void TrackingInitializer_ShouldUpdateTrackingIds_WhenBackgroundContextHaveTrackingIds_ButTelemetryContextDoesNotHaveTrackingIds_WithCorrelationIdFromHttpContext()
        {
            #region Arrange
            var mockCorrelationId = Guid.NewGuid().ToString();
            var mockTransactionId = Guid.NewGuid().ToString();
            var mockE2Eid = Guid.NewGuid().ToString();
            var mockAppConfiguration = new AppMetadataConfiguration()
            {
                CorrelationIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_CORRELATION_KEY,
                TransactionIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TRANSACTION_KEY,
                EndToEndTrackingHeaderKey = TelemetryConstant.HEADER_DEFAULT_E2E_KEY,
                TenantIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TENANT_ID
            };
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "Mock-Request"
            };

            var contextExecutionId = Guid.NewGuid().ToString();
            var contextTransactionId = Guid.NewGuid().ToString();
            var contextE2EId = Guid.NewGuid().ToString();
            var contextUserId = "mocker@microsoft.com";
            var contextTenantId = Guid.NewGuid().ToString();
            var mockHttpContext = CreateHttpContextFromTrackingIds(mockAppConfiguration, mockCorrelationId, null, null);

            AddBackgroundContext(contextExecutionId, mockCorrelationId, contextTransactionId, contextE2EId, contextUserId, contextTenantId);

            mockHttpContext.Request.Headers.Add(mockAppConfiguration.TenantIdHeaderKey, contextTenantId);
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(accessor => accessor.HttpContext)
                .Returns(mockHttpContext);
            #endregion Arrange

            #region Act
            var initializer = new TrackingInitializer(stubIHttpContextAccessor.Object, CreateMockAppInsightsConfiguration(), mockAppConfiguration);
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Asset
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.CORRELATION_KEY], mockCorrelationId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY], contextTransactionId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.E2E_KEY], contextE2EId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY], contextTenantId);
            #endregion Asset
        }

        [TestMethod]
        public void TrackingInitializer_ShouldAddDefaultValue_WhenHttpContextIsNull_AndBackgroundContextHasSeparateContext()
        {
            #region Arrange
            var mockAppConfiguration = new AppMetadataConfiguration()
            {
                CorrelationIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_CORRELATION_KEY,
                TransactionIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TRANSACTION_KEY,
                EndToEndTrackingHeaderKey = TelemetryConstant.HEADER_DEFAULT_E2E_KEY,
                TenantIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TENANT_ID
            };
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "Mock-Request"
            };

            var contextExecutionId = Guid.NewGuid().ToString();
            var contextTransactionId = Guid.NewGuid().ToString();
            var contextE2EId = Guid.NewGuid().ToString();
            var contextUserId = "mocker@microsoft.com";
            var contextTenantId = Guid.NewGuid().ToString();
            var mockHttpContext = CreateHttpContextFromTrackingIds(mockAppConfiguration, null, null, null);

            AddBackgroundContext(contextExecutionId, Guid.NewGuid().ToString(), contextTransactionId, contextE2EId, contextUserId, contextTenantId);

            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(accessor => accessor.HttpContext)
                .Returns(mockHttpContext);
            #endregion Arrange

            #region Act
            var initializer = new TrackingInitializer(stubIHttpContextAccessor.Object, CreateMockAppInsightsConfiguration(), mockAppConfiguration);
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Asset
            Assert.IsNotNull(requestTelemetry.Properties[TelemetryConstant.CORRELATION_KEY]);
            Assert.IsNotNull(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY]);
            Assert.AreNotEqual(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY], contextTransactionId);
            Assert.IsNotNull(requestTelemetry.Properties[TelemetryConstant.E2E_KEY]);
            Assert.AreNotEqual(requestTelemetry.Properties[TelemetryConstant.E2E_KEY], contextE2EId);
            Assert.IsNotNull(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY]);
            Assert.AreNotEqual(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY], contextTenantId);
            #endregion Asset
        }

        [TestMethod]
        public void TrackingInitializer_ShouldAddDefaultValue_WhenHttpContextAndTelemetryContextKeysAreMissing()
        {
            #region Arrange
            var mockAppConfiguration = new AppMetadataConfiguration()
            {
                CorrelationIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_CORRELATION_KEY,
                TransactionIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TRANSACTION_KEY,
                EndToEndTrackingHeaderKey = TelemetryConstant.HEADER_DEFAULT_E2E_KEY,
                TenantIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TENANT_ID
            };
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "Mock-Request"
            };

            var contextExecutionId = Guid.NewGuid().ToString();
            var contextTransactionId = Guid.NewGuid().ToString();
            var contextE2EId = Guid.NewGuid().ToString();
            var contextUserId = "mocker@microsoft.com";
            var contextTenantId = Guid.NewGuid().ToString();
            var mockHttpContext = new DefaultHttpContext();

            AddBackgroundContext(contextExecutionId, Guid.NewGuid().ToString(), contextTransactionId, contextE2EId, contextUserId, contextTenantId);

            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(accessor => accessor.HttpContext)
                .Returns(mockHttpContext);
            #endregion Arrange

            #region Act
            var initializer = new TrackingInitializer(stubIHttpContextAccessor.Object, CreateMockAppInsightsConfiguration(), mockAppConfiguration);
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Asset
            Assert.IsNotNull(requestTelemetry.Properties[TelemetryConstant.CORRELATION_KEY]);
            Assert.IsNotNull(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY]);
            Assert.AreNotEqual(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY], contextTransactionId);
            Assert.IsNotNull(requestTelemetry.Properties[TelemetryConstant.E2E_KEY]);
            Assert.AreNotEqual(requestTelemetry.Properties[TelemetryConstant.E2E_KEY], contextE2EId);
            Assert.IsNotNull(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY]);
            Assert.AreNotEqual(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY], contextTenantId);
            #endregion Asset
        }
        #endregion Background Context Tests

        #region User ID
        [TestMethod]
        public void TrackingInitializer_ShouldGetUserName_FromHttpContextClaims()
        {
            #region Arrange
            var mockCorrelationId = Guid.NewGuid().ToString();
            var mockTransactionId = Guid.NewGuid().ToString();
            var mockE2Eid = Guid.NewGuid().ToString();
            var mockTenantId = "MOCK-TEN";
            var mockAppConfiguration = new AppMetadataConfiguration()
            {
                CorrelationIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_CORRELATION_KEY,
                TransactionIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TRANSACTION_KEY,
                EndToEndTrackingHeaderKey = TelemetryConstant.HEADER_DEFAULT_E2E_KEY,
                TenantIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TENANT_ID
            };
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "Mock-Request"
            };
            requestTelemetry.Properties.Add(TelemetryConstant.CORRELATION_KEY, mockCorrelationId);
            requestTelemetry.Properties.Add(TelemetryConstant.TRANSACTION_KEY, mockTransactionId);
            requestTelemetry.Properties.Add(TelemetryConstant.E2E_KEY, mockE2Eid);
            requestTelemetry.Properties.Add(TelemetryConstant.TENANT_KEY, mockTenantId);

            var contextCorrelationId = Guid.NewGuid().ToString();
            var contextTransactionId = Guid.NewGuid().ToString();
            var contextE2EId = Guid.NewGuid().ToString();
            var contextTenantId = Guid.NewGuid().ToString();
            var mockUserId = "tester@microsoft.com";
            var mockHttpContext = CreateHttpContextFromTrackingIdsWithUserDetails(mockAppConfiguration, contextCorrelationId, contextTransactionId, contextE2EId, mockUserId);
            mockHttpContext.Request.Headers.Add(mockAppConfiguration.TenantIdHeaderKey, contextTenantId);
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(accessor => accessor.HttpContext)
                .Returns(mockHttpContext);
            #endregion Arrange

            #region Act
            var initializer = new TrackingInitializer(stubIHttpContextAccessor.Object, CreateMockAppInsightsConfiguration(), mockAppConfiguration);
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Asset
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.CORRELATION_KEY], contextCorrelationId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY], contextTransactionId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.E2E_KEY], contextE2EId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY], contextTenantId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.USER_KEY], mockUserId);

            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.CORRELATION_KEY + ":Alternate"], mockCorrelationId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY + ":Alternate"], mockTransactionId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.E2E_KEY + ":Alternate"], mockE2Eid);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY + ":Alternate"], mockTenantId);
            #endregion Asset
        }

        [TestMethod]
        public void TrackingInitializer_ShouldGetAppId_FromHttpContextClaims()
        {
            #region Arrange
            var mockCorrelationId = Guid.NewGuid().ToString();
            var mockTransactionId = Guid.NewGuid().ToString();
            var mockE2Eid = Guid.NewGuid().ToString();
            var mockTenantId = "MOCK-TEN";
            var mockAppConfiguration = new AppMetadataConfiguration()
            {
                CorrelationIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_CORRELATION_KEY,
                TransactionIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TRANSACTION_KEY,
                EndToEndTrackingHeaderKey = TelemetryConstant.HEADER_DEFAULT_E2E_KEY,
                TenantIdHeaderKey = TelemetryConstant.HEADER_DEFAULT_TENANT_ID
            };
            var requestTelemetry = new RequestTelemetry()
            {
                Name = "Mock-Request"
            };
            requestTelemetry.Properties.Add(TelemetryConstant.CORRELATION_KEY, mockCorrelationId);
            requestTelemetry.Properties.Add(TelemetryConstant.TRANSACTION_KEY, mockTransactionId);
            requestTelemetry.Properties.Add(TelemetryConstant.E2E_KEY, mockE2Eid);
            requestTelemetry.Properties.Add(TelemetryConstant.TENANT_KEY, mockTenantId);

            var contextCorrelationId = Guid.NewGuid().ToString();
            var contextTransactionId = Guid.NewGuid().ToString();
            var contextE2EId = Guid.NewGuid().ToString();
            var contextTenantId = Guid.NewGuid().ToString();
            var mockAppId = Guid.NewGuid().ToString();
            var mockHttpContext = CreateHttpContextFromTrackingIdsWithAppDetails(mockAppConfiguration, contextCorrelationId, contextTransactionId, contextE2EId, mockAppId);
            mockHttpContext.Request.Headers.Add(mockAppConfiguration.TenantIdHeaderKey, contextTenantId);
            var stubIHttpContextAccessor = new Mock<IHttpContextAccessor>();
            stubIHttpContextAccessor.Setup(accessor => accessor.HttpContext)
                .Returns(mockHttpContext);
            #endregion Arrange

            #region Act
            var initializer = new TrackingInitializer(stubIHttpContextAccessor.Object, CreateMockAppInsightsConfiguration(), mockAppConfiguration);
            initializer.Initialize(requestTelemetry);
            #endregion Act

            #region Asset
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.CORRELATION_KEY], contextCorrelationId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY], contextTransactionId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.E2E_KEY], contextE2EId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY], contextTenantId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.USER_KEY], $"SPN:{mockAppId}");

            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.CORRELATION_KEY + ":Alternate"], mockCorrelationId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TRANSACTION_KEY + ":Alternate"], mockTransactionId);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.E2E_KEY + ":Alternate"], mockE2Eid);
            Assert.AreEqual(requestTelemetry.Properties[TelemetryConstant.TENANT_KEY + ":Alternate"], mockTenantId);
            #endregion Asset
        }
        #endregion User ID

        #region Private Helpers
        private ApplicationInsightsConfiguration CreateMockAppInsightsConfiguration()
        {
            return new ApplicationInsightsConfiguration()
            {
                InstrumentationKey = Guid.NewGuid().ToString()
            };
        }
        private IHttpContextAccessor CreateMockHttpContextAccessor()
        {
            Mock<IHttpContextAccessor> mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            return mockHttpContextAccessor.Object;
        }

        private HttpContext CreateHttpContextFromTrackingIdsWithUserDetails(AppMetadataConfiguration configuration, string correlationId, string transactionId, string e2eId, string userId)
        {
            var context = CreateHttpContextFromTrackingIds(configuration, correlationId, transactionId, e2eId);
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.Name, userId));
            context.User = new ClaimsPrincipal(identity);
            return context;
        }

        private HttpContext CreateHttpContextFromTrackingIdsWithAppDetails(AppMetadataConfiguration configuration, string correlationId, string transactionId, string e2eId, string appId)
        {
            var context = CreateHttpContextFromTrackingIds(configuration, correlationId, transactionId, e2eId);
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("appid", appId));
            context.User = new ClaimsPrincipal(identity);
            return context;
        }

        private HttpContext CreateHttpContextFromTrackingIds(AppMetadataConfiguration configuration, string correlationId, string transactionId, string e2eId)
        {
            var context = new DefaultHttpContext();
            context.Request.Headers.Add(configuration.CorrelationIdHeaderKey, correlationId);
            context.Request.Headers.Add(configuration.TransactionIdHeaderKey, transactionId);
            context.Request.Headers.Add(configuration.EndToEndTrackingHeaderKey, e2eId);
            return context;
        }

        private void AddBackgroundContext(string executionId, string correlationId, string transactionId, string e2eId, string userId, string tenantId)
        {
            var stubIExceutionContextProvider = new Mock<ICurrentExecutionContextProvider>();
            stubIExceutionContextProvider.Setup(provider => provider.GetCurrentExecutionContextId())
                .Returns(executionId);
            BackgroundContext.AddCurrentContext(stubIExceutionContextProvider.Object, correlationId, "TEST", e2eId, transactionId, userId, tenantId);
        }
        #endregion Private Helpers
    }
}
