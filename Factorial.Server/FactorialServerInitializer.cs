using DotNetty.Codecs.Compression;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Factorial.Common;

namespace Factorial.Server {
  /// <summary>
  /// Creates a newly configured <see cref="T:DotNetty.Transport.Channels.IChannelPipeline" /> for a server-side channel.
  /// </summary>
  public class FactorialServerInitializer : ChannelInitializer<ISocketChannel> {
    protected override void InitChannel(ISocketChannel channel) {
      IChannelPipeline pipeline = channel.Pipeline;

      // Enable stream compresion.
      pipeline.AddLast(ZlibCodecFactory.NewZlibEncoder(ZlibWrapper.Gzip));
      pipeline.AddLast(ZlibCodecFactory.NewZlibDecoder(ZlibWrapper.Gzip));

      // Add the number codec first.
      pipeline.AddLast(new BigIntegerDecoder());
      pipeline.AddLast(new BigIntegerEncoder());

      // Add the business logic.
      // Please note, we create a handler for every new channel, because it has stateful properties.
      pipeline.AddLast(new FactorialServerHandler());
    }
  }
}
