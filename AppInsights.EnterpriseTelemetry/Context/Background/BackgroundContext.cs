using System.Linq;
using System.Collections.Generic;

namespace AppInsights.EnterpriseTelemetry.Context.Background
{
    public static class ContextContainer
    {
        public static readonly List<ContextDetails> Contexts = new List<ContextDetails>();
    }

    public static class BackgroundContext
    {
        public static ContextDetails AddCurrentContext(ICurrentExecutionContextProvider currentExecutionContextProvider, string correlationId, string operationName, string endToEndTrackingId, string transactionId, string parentUserId, string tenantId)
        {
            var executionId = currentExecutionContextProvider.GetCurrentExecutionContextId();
            var context = new ContextDetails()
            {
                ExecutingId = executionId,
                CorrelationId = correlationId,
                TransactionId = transactionId,
                EndToEndTrackingId = endToEndTrackingId,
                UserId = parentUserId,
                TenantId = tenantId,
                OperationName = operationName
            };
            ContextContainer.Contexts.Add(context);
            return context;
        }

        public static void RemoveCurrentContext(ICurrentExecutionContextProvider currentExecutionContextProvider)
        {
            var executionId = currentExecutionContextProvider.GetCurrentExecutionContextId();
            if (ContextContainer.Contexts.Any(context => context.ExecutingId == executionId))
            {
                ContextContainer.Contexts.RemoveAll(context => context.ExecutingId == executionId);
            }
        }

        public static ContextDetails GetCurrentContextByCorrelationId(string correlationId)
        {
            return ContextContainer.Contexts.FirstOrDefault(context => context.CorrelationId == correlationId);
        }
    }
}
