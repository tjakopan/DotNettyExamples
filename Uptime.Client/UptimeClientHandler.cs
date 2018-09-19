using System;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using NLog;

namespace Uptime.Client {
  /// <summary>
  /// Keep reconnecting to the server while printing out the current uptime and connection attempt getStatus.
  /// </summary>
  public class UptimeClientHandler : SimpleChannelInboundHandler<object> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public long StartTime { private get; set; } = -1;

    public override void ChannelActive(IChannelHandlerContext context) {
      if (StartTime < 0) {
        StartTime = DateTime.Now.Ticks;
      }

      WriteLine($"Connected to: {context.Channel.RemoteAddress}");
    }

    protected override void ChannelRead0(IChannelHandlerContext ctx, object msg) {
      // Discard received data.
    }

    public override void UserEventTriggered(IChannelHandlerContext context, object evt) {
      if (!(evt is IdleStateEvent)) {
        return;
      }

      IdleStateEvent e = (IdleStateEvent) evt;
      if (e.State != IdleState.ReaderIdle) return;

      // The connection was OK, but there was no traffic for last period.
      WriteLine("Disconnecting due to no inbound traffic.");
      context.CloseAsync();
    }

    public override void ChannelInactive(IChannelHandlerContext context) {
      WriteLine($"Disconnected from: {context.Channel.RemoteAddress}");
    }

    public override void ChannelUnregistered(IChannelHandlerContext context) {
      WriteLine($"Sleeping for: {Program.ReconnectDelay.Seconds} s.");

      context.Channel.EventLoop.Schedule(() => {
        WriteLine($"Reconnecting to: {Program.Host}:{Program.Port}");
        Program.Connect();
      }, Program.ReconnectDelay);
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
      Logger.Error(exception, "Exception while handling message");
      context.CloseAsync();
    }

    private void WriteLine(string msg) {
      Logger.Info(StartTime < 0
        ? $"[SERVER IS DOWN] {msg}"
        : $"[UPTIME: {TimeSpan.FromTicks(DateTime.Now.Ticks - StartTime).TotalSeconds} s] {msg}");
    }
  }
}
