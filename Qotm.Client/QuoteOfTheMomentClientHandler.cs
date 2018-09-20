using System;
using System.Text;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using NLog;

namespace Qotm.Client {
  public class QuoteOfTheMomentClientHandler : SimpleChannelInboundHandler<DatagramPacket> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket msg) {
      string response = msg.Content.ToString(Encoding.UTF8);
      if (response.StartsWith("QOTM: ")) {
        Logger.Info("Quote of the moment: " + response.Substring(6));
        ctx.CloseAsync();
      }
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
      Logger.Error(exception, "Error in clent handler.");
      context.CloseAsync();
    }
  }
}
