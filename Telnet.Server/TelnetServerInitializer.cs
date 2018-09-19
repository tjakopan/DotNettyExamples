using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Telnet.Server {
  public class TelnetServerInitializer : ChannelInitializer<ISocketChannel> {
    private static readonly StringDecoder Decoder = new StringDecoder();
    private static readonly StringEncoder Encoder = new StringEncoder();

    private static readonly TelnetServerHandler ServerHandler = new TelnetServerHandler();

    protected override void InitChannel(ISocketChannel channel) {
      channel.Pipeline
        // Add the text line codec combination first.
        .AddLast(new DelimiterBasedFrameDecoder(8192, Delimiters.LineDelimiter()))
        // The encoder and decoder are static as these are sharable.
        .AddLast(Decoder)
        .AddLast(Encoder)
        // Add the business logic.
        .AddLast(ServerHandler);
    }
  }
}
