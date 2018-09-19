using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Time.Common;

namespace Time.Server {
  public class TimeEncoder : MessageToByteEncoder<UnixTime> {
    protected override void Encode(IChannelHandlerContext context, UnixTime message, IByteBuffer output) {
      output.WriteLong(message.Ticks);
    }
  }
}
