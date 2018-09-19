using DotNetty.Common.Internal.Logging;
using NLog.Extensions.Logging;

namespace Examples.Logging {
  public static class LoggingHelper {
    public static void SetNLogLogger() => InternalLoggerFactory.DefaultFactory.AddProvider(new NLogLoggerProvider());
  }
}
