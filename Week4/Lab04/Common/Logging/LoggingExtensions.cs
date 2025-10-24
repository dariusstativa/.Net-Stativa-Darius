namespace Lab04.Common.Logging;

public static class LoggingExtensions
{
    public static void LogProductCreationMetrics(
        this ILogger logger,
        ProductCreationMetrics metrics)
    {
        if (logger == null) return;

        
        logger.LogInformation(
            eventId: LogEvents.ProductCreationCompleted,
            message:
            "Product creation metrics | Name: {ProductName} | SKU: {SKU} | Category: {Category} | " +
            "Validation: {ValidationMs} ms | Database: {DatabaseMs} ms | Total: {TotalMs} ms | " +
            "Success: {Success} | Error: {Error}",
            metrics.ProductName,
            metrics.SKU,
            metrics.Category,
            metrics.ValidationDuration.TotalMilliseconds,
            metrics.DatabaseSaveDuration.TotalMilliseconds,
            metrics.TotalDuration.TotalMilliseconds,
            metrics.Success,
            metrics.ErrorReason ?? "None"
        );
    }
}