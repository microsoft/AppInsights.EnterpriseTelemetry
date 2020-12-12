using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;

#pragma warning disable CA1031 // Do not catch general exception types
namespace AppInsights.EnterpriseTelemetry.AppInsightsInitializers
{
    /// <summary>
    /// Custom application insight initializer which overrides the Success value of a Request log if a client side error (4xx errors) has ocurred
    /// </summary>
    public class ClientSideErrorInitializer : ITelemetryInitializer
    {
        /// <summary>
        /// Overrides the Success property for client side error
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

                if (responseCode >= 400 && responseCode < 500)
                {
                    requestTrace.Success = true;
                    requestTrace.Properties["Overridden400s"] = "true"; //Allow AI to filter property
                }
            }
            catch (Exception)
            {
                //GULP-EXCEPTION-DONT-DO-ANYTING
            }
        }
    }
}
#pragma warning restore CA1031 // Do not catch general exception types
