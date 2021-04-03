using System;
using System.Diagnostics.CodeAnalysis;
using AppInsights.EnterpriseTelemetry.Exceptions;

namespace AppInsights.EnterpriseTelemetry.Tests.Mocks
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class CustomAppException : BaseAppException
    {
        public override string Type => "Custom Exception";

        public CustomAppException(string message, string correlationId, string transactionId, string exceptionCode, string failedMethod)
           : base(message, null, correlationId, transactionId, exceptionCode, failedMethod)
        { }

        protected override string CreateDisplayMessage()
        {
            return "Custom Exception";
        }
    }
}
