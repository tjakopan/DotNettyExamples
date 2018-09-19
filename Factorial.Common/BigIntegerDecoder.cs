using System.Collections.Generic;
using System.Numerics;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Factorial.Common {
  /// <summary>
  /// Decodes the binary representation of a <see cref="BigInteger"/> prepended with a magic number ('F' or 0x46) and
  /// a 32 bit integer length prefix into a <see cref="BigInteger"/> instance. For example, {'F', 0, 0, 0, 1, 42} will
  /// be decoded into new BigIntere("42").
  /// </summary>
  public class BigIntegerDecoder : ByteToMessageDecoder {
    protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output) {
      // Wait until the length prefix is available.
      if (input.ReadableBytes < 5) {
        return;
      }

      input.MarkReaderIndex();

      // Check the magic number.
      byte magicNumber = input.ReadByte();
      if (magicNumber != 'F') {
        input.ResetReaderIndex();
        throw new CorruptedFrameException("Invalid magic number: " + magicNumber);
      }

      // Wait until the whole data is available.
      int dataLength = input.ReadInt();
      if (input.ReadableBytes < dataLength) {
        input.ResetReaderIndex();
        return;
      }

      // Convert the received data into a new BigIntere.
      byte[] decoded = new byte[dataLength];
      input.ReadBytes(decoded);

      output.Add(new BigInteger(decoded));
    }
  }
}
