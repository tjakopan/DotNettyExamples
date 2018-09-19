using System;
using System.Net;
using DotNetty.Transport.Channels;
using NLog;

namespace Telnet.Server {
  public class TelnetServerHandler : SimpleChannelInboundHandler<string> {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public override void ChannelActive(IChannelHandlerContext context) {
      // Send greeting for a new connection.
      context.WriteAsync($"Welcome to {Dns.GetHostName()}!\r\n");
      context.WriteAsync($"It is {DateTime.Now} now.\r\n");
      context.Flush();
    }

    protected override void ChannelRead0(IChannelHandlerContext ctx, string msg) {
      // Generate and write a response.
      string response;
      bool close = false;
      if (msg.Length == 0) {
        response = "Please type something.\r\n";
      } else if ("bye".Equals(msg.ToLower())) {
        response = "Have a good day!\r\n";
        close = true;
      } else {
        response = $"Did you say '{msg}'?\r\n";
      }

      // We do not need to write a ChannelBuffer here.
      // We know the encoder inserted at TelnetPiplineFactory will do the conversion.
      ctx.WriteAsync(response).ContinueWith(task => {
        // Close the connection after sending 'Have a good day!' if the cient has sent 'bye'.
        if (close) {
          ctx.CloseAsync();
        }
      });
    }

    public override void ChannelReadComplete(IChannelHandlerContext context) {
      context.Flush();
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
      Logger.Error(exception, "Exception telnet server handler.");
      context.CloseAsync();
    }

    public override bool IsSharable => true;
  }
}
