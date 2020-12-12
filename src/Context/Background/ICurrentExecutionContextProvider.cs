namespace AppInsights.EnterpriseTelemetry.Context.Background
{
    public interface ICurrentExecutionContextProvider
    {
        string GetCurrentExecutionContextId();
    }
}
