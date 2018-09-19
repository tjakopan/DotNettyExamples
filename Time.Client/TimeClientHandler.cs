using System;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Time.Common;

namespace Time.Client {
  public class TimeClientHandler : ChannelHandlerAdapter {

    public override void ChannelRead(IChannelHandlerContext context, object message) {
      UnixTime m = (UnixTime) message;
      Console.WriteLine(m);
      context.CloseAsync();
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
      Console.WriteLine("Exception: " + exception);
      context.CloseAsync();
    }
  }
}
