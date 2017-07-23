using OculiService.Common.Logging;

public class LoggerHelper
{
    private const int RollSizeInKB = 10240;

    public static void Close(ILogger logger)
    {
        if (logger == null)
            return;
        ((TraceSourceLogger)logger).TraceSource.Close();
        ((TraceSourceLogger)logger).TraceSource.Listeners.Clear();
        logger = (ILogger)null;
    }
}
