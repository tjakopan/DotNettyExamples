using System;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Discard.Server {
  public class DiscardServerHandler : SimpleChannelInboundHandler<object> {
    protected override void ChannelRead0(IChannelHandlerContext ctx, object msg) {
      IByteBuffer inMsg = (IByteBuffer) msg;
      while (inMsg.IsReadable()) {
        Console.Write((char) inMsg.ReadByte());
        Console.Out.Flush();
      }
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
      Console.WriteLine("{0}", exception);
      context.CloseAsync();
    }
  }
}
