using System;
using System.Net;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;

#pragma warning disable CA1031 // Do not catch general exception types
namespace AppInsights.EnterpriseTelemetry.AppInsightsInitializers
{
    /// <summary>
    /// Custom application insight telemetry initializer to send the HTTP Status description along with status code for all request logs
    /// </summary>
    public class ResponseCodeTranslationIntitializer : ITelemetryInitializer
    {
        /// <summary>
        /// Adds the status description along with HTTP status code
        /// </summary>
        /// <param name="telemetry" cref="ITelemetry">Application insight base telemetry type</param>
        public void Initialize(ITelemetry telemetry)
        {
            try
            {
                if (!(telemetry is RequestTelemetry requestTrace))
                    return;

                if (!int.TryParse(requestTrace.ResponseCode, out int responseCode))
                    return;

                var convertedHttpEnum = ConvertFromInt(responseCode);
                if (!convertedHttpEnum.HasValue)
                    return;

                var httpStatusCode = convertedHttpEnum.Value;
                var httpStatusDescription = httpStatusCode.ToString();
                requestTrace.Properties.Add("Response Code Description", httpStatusDescription);
            }
            catch (Exception exception)
            {
                ((ISupportProperties)telemetry).Properties.Add("RESPONSE_CODE_TRANSLATION_INTITIALIZER_EXCEPTION", exception.ToString());
            }
        }

        private HttpStatusCode? ConvertFromInt(int code)
        {
            if (typeof(HttpStatusCode).IsEnumDefined(code))
                return (HttpStatusCode)code;
            return null;
        }
    }
}
#pragma warning restore CA1031 // Do not catch general exception types
