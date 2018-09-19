using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Time.Common;

namespace Time.Client {
  public class TimeDecoder : ByteToMessageDecoder {
    protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output) {
      if (input.ReadableBytes < 8) {
        return;
      }

      output.Add(new UnixTime(input.ReadLong()));
    }
  }
}
