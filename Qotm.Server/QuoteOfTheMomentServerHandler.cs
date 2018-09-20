using System;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using NLog;

namespace Qotm.Server {
  public class QuoteOfTheMomentServerHandler : SimpleChannelInboundHandler<DatagramPacket> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static readonly Random Random = new Random();

    // Quotes from Mohandas K. Gandhi:
    private static readonly string[] Quotes = {
      "Where there is love there is life.",
      "First they ignore you, then they laugh at you, then they fight you, then you win.",
      "Be the change you want to see in the world.",
      "The weak can never forgive. Forgiveness is the attribute of the strong."
    };

    private static string NextQuote() {
      int quoteId;
      lock (Random) {
        quoteId = Random.Next(Quotes.Length);
      }

      return Quotes[quoteId];
    }

    protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket msg) {
      Logger.Info(msg);
      if ("QOTM?".Equals(msg.Content.ToString(Encoding.UTF8))) {
        ctx.WriteAsync(new DatagramPacket(Unpooled.CopiedBuffer("QOTM: " + NextQuote(), Encoding.UTF8), msg.Sender));
      }
    }

    public override void ChannelReadComplete(IChannelHandlerContext context) {
      context.Flush();
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
      Logger.Error(exception, "Exception in servr handler.");
      // We don't close the channel because we can keep serving requests.
    }
  }
}
