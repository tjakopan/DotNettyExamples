using DotNetty.Codecs.Protobuf;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using WorldClock.Common;

namespace WorldClock.Server {
  public class WorldClockServerInitializer : ChannelInitializer<ISocketChannel> {
    protected override void InitChannel(ISocketChannel channel) {
      channel.Pipeline
        .AddLast(new ProtobufVarint32FrameDecoder())
        .AddLast(new ProtobufDecoder(TimeZoneIds.Parser))
        .AddLast(new ProtobufVarint32LengthFieldPrepender())
        .AddLast(new ProtobufEncoder())
        .AddLast(new WorldClockServerHandler());
    }
  }
}
