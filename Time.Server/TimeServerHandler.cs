using System;
using DotNetty.Transport.Channels;
using Time.Common;

namespace Time.Server {
  public class TimeServerHandler : ChannelHandlerAdapter {
    public override void ChannelActive(IChannelHandlerContext context) {
      context.WriteAndFlushAsync(new UnixTime()).ContinueWith(task => context.CloseAsync());
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
      Console.WriteLine("Exception: " + exception);
      context.CloseAsync();
    }
  }
}
