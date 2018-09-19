using System;
using DotNetty.Transport.Channels;
using NLog;

namespace Uptime.Server {
  public class UptimeServerHandler : SimpleChannelInboundHandler<object> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    protected override void ChannelRead0(IChannelHandlerContext ctx, object msg) {
      // discard
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
      Logger.Error(exception, "Exception while handling message");
      context.CloseAsync();
    }
  }
}
