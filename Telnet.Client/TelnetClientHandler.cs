using System;
using DotNetty.Transport.Channels;
using NLog;

namespace Telnet.Client {
  public class TelnetClientHandler : SimpleChannelInboundHandler<string> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    protected override void ChannelRead0(IChannelHandlerContext ctx, string msg) {
      Logger.Info(msg);
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
      Logger.Error(exception, "Exception in telnet client handler.");
      context.CloseAsync();
    }

    public override bool IsSharable => true;
  }
}
