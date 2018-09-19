using DotNetty.Codecs.Compression;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Factorial.Common;

namespace Factorial.Client {
  /// <summary>
  /// Creates a newly configured <see cref="IChannelPipeline"/> for a client side channel.
  /// </summary>
  public class FactorialClientInitializer  : ChannelInitializer<ISocketChannel> {
    protected override void InitChannel(ISocketChannel channel) {
      IChannelPipeline pipeline = channel.Pipeline;

      // Enable stream compression.
      pipeline.AddLast(ZlibCodecFactory.NewZlibEncoder(ZlibWrapper.Gzip));
      pipeline.AddLast(ZlibCodecFactory.NewZlibDecoder(ZlibWrapper.Gzip));

      // Add the number codec first.
      pipeline.AddLast(new BigIntegerDecoder());
      pipeline.AddLast(new BigIntegerEncoder());

      // Add the business logic.
      pipeline.AddLast(new FactorialClientHandler());
    }
  }
}
