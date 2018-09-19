using System.Numerics;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Factorial.Common {
  /// <summary>
  /// Encodes <see cref="BigInteger"/> into the binary representation preprended with a magic number ('F' or 0x46) and
  /// a 32 bit length prefix. For example, 42 will be encoded to {'F', 0, 0, 0, 1, 42}.
  /// </summary>
  public class BigIntegerEncoder : MessageToByteEncoder<BigInteger> {
    protected override void Encode(IChannelHandlerContext context, BigInteger message, IByteBuffer output) {
      // Convert the number into a byte array.
      byte[] data = message.ToByteArray();
      int dataLength = data.Length;

      // Write a message.
      output.WriteByte((byte) 'F');
      output.WriteInt(dataLength);
      output.WriteBytes(data);
    }
  }
}
