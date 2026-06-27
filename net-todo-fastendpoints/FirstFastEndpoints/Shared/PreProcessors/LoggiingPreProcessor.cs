using FastEndpoints;

namespace FirstFastEndpoints.Shared.PreProcessors
{
    public class LoggiingPreProcessor<TRequest> : IPreProcessor<TRequest>
    {
        public Task PreProcessAsync(IPreProcessorContext<TRequest> context, CancellationToken ct)
        {
            var logger = context.HttpContext.Resolve<ILogger<TRequest>>();

            logger.LogInformation(
            $"request:{context.HttpContext.Request.GetType().FullName} path: {context.HttpContext.Request.Path}");

            return Task.CompletedTask;
        }
    }
}
