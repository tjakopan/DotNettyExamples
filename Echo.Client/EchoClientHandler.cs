using System;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Echo.Client {
  public class EchoClientHandler : ChannelHandlerAdapter {
    private readonly IByteBuffer _initialMessage;

    public EchoClientHandler(int messageSize) {
      _initialMessage = Unpooled.Buffer(messageSize);
      byte[] messageBytes = Encoding.UTF8.GetBytes("Hello world");
      _initialMessage.WriteBytes(messageBytes);
    }

    public override void ChannelActive(IChannelHandlerContext context) {
      context.WriteAndFlushAsync(_initialMessage);
    }

    public override void ChannelRead(IChannelHandlerContext context, object message) {
      IByteBuffer byteBuffer = (IByteBuffer) message;
      if (byteBuffer != null) {
        Console.WriteLine("Received from server: " + byteBuffer.ToString(Encoding.UTF8));
      }
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
