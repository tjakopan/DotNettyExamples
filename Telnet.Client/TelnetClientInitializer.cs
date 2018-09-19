using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Telnet.Client {
  public class TelnetClientInitializer : ChannelInitializer<ISocketChannel> {
    private static readonly StringDecoder Decoder = new StringDecoder();
    private static readonly StringEncoder Encoder = new StringEncoder();

    private static readonly TelnetClientHandler ClientHandler = new TelnetClientHandler();

    protected override void InitChannel(ISocketChannel channel) {
      channel.Pipeline
        // Add the text line codec combination first.
        .AddLast(new DelimiterBasedFrameDecoder(8192, Delimiters.LineDelimiter()))
        .AddLast(Decoder)
        .AddLast(Encoder)
        // Add business logic.
        .AddLast(ClientHandler);
    }
  }
}
