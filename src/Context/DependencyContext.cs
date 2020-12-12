using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

namespace AppInsights.EnterpriseTelemetry.Context
{
    public class DependencyContext : LogContext
    {
        public string DependencyName { get; set; }

        public string TargetSystemName { get; set; }
        public string DependencyType { get; set; }
        public string RequestDetails { get; set; }
        public List<DependencyParameter> RequestParameters { get; }

        public bool IsSuccessfull { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public Exception ResponseError { get; set; }
        public List<DependencyParameter> ResponseParameters { get; }

        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public TimeSpan TimeTaken { get; set; }

        private bool _isDepdencyInProgresss;
        private readonly bool _shouldLogPerformance;
        private readonly PerformanceContext _performanceContext;

        public DependencyContext() : base()
        {
            RequestParameters = new List<DependencyParameter>();
            ResponseParameters = new List<DependencyParameter>();
            StartTime = DateTimeOffset.UtcNow;
            _isDepdencyInProgresss = true;
            _shouldLogPerformance = true;
            _performanceContext = new PerformanceContext();
            _performanceContext.Start();
        }

        public DependencyContext(string dependencyName, string targetSystemName, string dependencyType, bool shouldLogPerformance, string requestDetails, string correlationId, string transactionId, string source, string userId, string endToEndTrackingId)
            : this()
        {
            DependencyName = dependencyName;
            TargetSystemName = targetSystemName;
            DependencyType = dependencyType;
            RequestDetails = requestDetails;

            CorrelationId = correlationId;
            TransactionId = transactionId;
            EndToEndTrackingId = endToEndTrackingId;
            Source = source;
            UserId = userId;

            _shouldLogPerformance = shouldLogPerformance;
            _performanceContext.MetricName = targetSystemName;
        }

        public DependencyContext(string dependencyName, string targetSystemName, string dependencyType, string requestDetails, bool shouldLogPerformance)
            : this(dependencyName, targetSystemName, dependencyType, shouldLogPerformance, requestDetails, null, null, null, null, null)
        { }

        public DependencyContext(string dependencyName, string targetSystemName, string dependencyType, string requestDetails)
            : this(dependencyName, targetSystemName, dependencyType, requestDetails, shouldLogPerformance: true)
        { }

        public DependencyContext(DependencyContextMetadata metadata)
            : this(metadata.DependencyName, metadata.TargetSystemName, metadata.DependencyType, metadata.RequestDetails, metadata.ShouldLogPerformance)
        {
        }

        public void AddRequestParameter(string name, string type, string value)
        {
            AddRequestParameter(name, type, value, isSensitive: false);
        }

        public void AddRequestParameter(string name, string type, object value)
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value);
                AddRequestParameter(name, type, serializedValue);
            }
            catch (Exception)
            {
                AddRequestParameter(name, type, "**UNREADABLE**");
            }
        }

        public void AddRequestParameter(string name, string type, string value, bool isSensitive)
        {
            RequestParameters.Add(new DependencyParameter()
            {
                Name = name,
                Type = type,
                Value = value,
                IsSensitive = isSensitive
            });
        }

        public void CompleteDependency()
        {
            if (!_isDepdencyInProgresss)
                return;

            EndTime = DateTimeOffset.UtcNow;
            TimeTaken = EndTime - StartTime;
            _performanceContext.Stop();
            _isDepdencyInProgresss = false;
            IsSuccessfull = true;
            if (string.IsNullOrWhiteSpace(_performanceContext.MetricName))
                _performanceContext.MetricName = TargetSystemName;
        }

        public void CompleteDependency(string responseCode, string responseMessage)
        {
            CompleteDependency();
            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
        }

        public void FailDependency(string responseCode, string responseMessage)
        {
            IsSuccessfull = false;
            _isDepdencyInProgresss = false;
            ResponseError = new Exception(responseMessage);
            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
            _performanceContext.Stop();
        }

        public void FailDependency(Exception responseError, string responseCode)
        {
            IsSuccessfull = false;
            _isDepdencyInProgresss = false;
            ResponseError = responseError;
            ResponseCode = responseCode;
            ResponseMessage = responseError.ToString();
            _performanceContext.Stop();
        }

        public void CreateAdditionalProperties()
        {
            Properties.AddOrUpdate("Custom", bool.TrueString);
            if (RequestParameters.Any())
                Properties.AddOrUpdate(RequestParameters.Select(param => param.GetKeyValuePair()));

            Properties.AddOrUpdate("StartedOn", StartTime.ToString());
            if (!_isDepdencyInProgresss)
            {
                Properties.AddOrUpdate("CompletedOn", EndTime.ToString());
                Properties.AddOrUpdate("TimeTaken", TimeTaken.TotalMilliseconds.ToString());
            }

            Properties.Add("IsSuccesfull", IsSuccessfull.ToString());
            Properties.AddOrUpdate("ResponseCode", ResponseCode);
            if (ResponseParameters.Any())
                Properties.AddOrUpdate(ResponseParameters.Select(param => param.GetKeyValuePair()));
            Properties.Add("ResponseMessage", ResponseMessage);
        }

        public ExceptionContext GetExceptionContext()
        {
            if (IsSuccessfull)
                return null;

            var exceptionContext = new ExceptionContext(ResponseError)
            {
                CorrelationId = CorrelationId,
                TransactionId = TransactionId,
                Source = Source,
                EndToEndTrackingId = EndToEndTrackingId,
                UserId = UserId
            };
            exceptionContext.AddProperties(Properties);
            exceptionContext.AddProperty(nameof(DependencyName), DependencyName);
            exceptionContext.AddProperty(nameof(TargetSystemName), TargetSystemName);
            return exceptionContext;
        }

        public PerformanceContext GetPerformanceContext()
        {
            if (!_shouldLogPerformance)
                return null;

            _performanceContext.AddProperties(Properties);
            _performanceContext.AddProperty(nameof(DependencyName), DependencyName);
            _performanceContext.AddProperty(nameof(TargetSystemName), TargetSystemName);
            return _performanceContext;
        }
    }

    public class DependencyParameter
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public bool IsSensitive { get; set; }

        public KeyValuePair<string, string> GetKeyValuePair() =>
            new KeyValuePair<string, string>($"{Type}:{Name}", IsSensitive ? "**REDACTED**" : Value);
    }


}
