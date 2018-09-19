using System;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Discard.Client {
  public class DiscardClientHandler : SimpleChannelInboundHandler<object> {
    private readonly int _contentSize;
    private byte[] _content;
    private IChannelHandlerContext _ctx;

    public DiscardClientHandler(int contentSize) {
      _contentSize = contentSize;
    }

    public override void ChannelActive(IChannelHandlerContext context) {
      _ctx = context;

      _content = new byte[_contentSize];

      // Send the initial messages.
      GenerateTraffic();
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
      Console.WriteLine("{0}", exception);
      context.CloseAsync();
    }

    private async void GenerateTraffic() {
      try {
        IByteBuffer buffer = Unpooled.WrappedBuffer(_content);
        // Flush the outbound buffer to the socket. Once flushed, generate the same amount of traffic again.
        await _ctx.WriteAndFlushAsync(buffer);
        GenerateTraffic();
      } catch {
        await _ctx.CloseAsync();
      }
    }

    protected override void ChannelRead0(IChannelHandlerContext ctx, object msg) {
      // Server is supposed to send nothing, but if it sends something, discard it.
    }
  }
}
