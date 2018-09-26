using DotNetty.Codecs.Protobuf;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using WorldClock.Common;

namespace WorldClock.Client {
  public class WorldClockClientInitializer : ChannelInitializer<ISocketChannel> {
    protected override void InitChannel(ISocketChannel channel) {
      channel.Pipeline
        .AddLast(new LoggingHandler(LogLevel.TRACE))
        .AddLast(new ProtobufVarint32FrameDecoder())
        .AddLast(new ProtobufDecoder(LocalTimes.Parser))
        .AddLast(new ProtobufVarint32LengthFieldPrepender())
        .AddLast(new ProtobufEncoder())
        .AddLast(new WorldClockClientHandler());
    }
  }
}
