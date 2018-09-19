using System;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Echo.Server {
  public class EchoServerHandler : ChannelHandlerAdapter {
    public override void ChannelRead(IChannelHandlerContext context, object message) {
      IByteBuffer buffer = (IByteBuffer) message;
      if (buffer != null) {
        Console.WriteLine("Received from client: " + buffer.ToString(Encoding.UTF8));
      }

      context.WriteAsync(message);
    }

    public override void ChannelReadComplete(IChannelHandlerContext context) {
      context.Flush();
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
      Console.WriteLine("Exception: " + exception);
      context.CloseAsync();
    }
  }
}
