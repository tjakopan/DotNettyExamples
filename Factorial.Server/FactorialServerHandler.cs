using System;
using System.Numerics;
using DotNetty.Transport.Channels;

namespace Factorial.Server {
  public class FactorialServerHandler : SimpleChannelInboundHandler<BigInteger> {
    private BigInteger _lastMultiplier = new BigInteger(1);
    private BigInteger _factorial = new BigInteger(1);

    protected override void ChannelRead0(IChannelHandlerContext ctx, BigInteger msg) {
      // Calculate the cumulative factorial and send it to the client.
      _lastMultiplier = msg;
      _factorial *= msg;
      ctx.WriteAndFlushAsync(_factorial);
    }

    public override void ChannelInactive(IChannelHandlerContext context) {
      Console.WriteLine($"Factorial of {_lastMultiplier} is: {_factorial}");
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
      Console.Error.WriteLine("Exception: " + exception);
      context.CloseAsync();
    }
  }
}
