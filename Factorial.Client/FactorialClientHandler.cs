using System;
using System.Collections.Concurrent;
using System.Numerics;
using DotNetty.Transport.Channels;

namespace Factorial.Client {
  /// <summary>
  /// Handler for a client side channel. This handler maintains stateful information which is specific to a certain
  /// channel using member variables. Therefore, an instance of this handler can cover only one channel. You have to
  /// create a new handler instance whenever you create a new channel and insert this handler to avoid a race condition.
  /// </summary>
  public class FactorialClientHandler : SimpleChannelInboundHandler<BigInteger> {

    private IChannelHandlerContext _ctx;
    private int _receivedMessages;
    private int _next = 1;
    private readonly BlockingCollection<BigInteger> _answer = new BlockingCollection<BigInteger>();

    public BigInteger GetFactorial() {
      return _answer.Take();
    }

    public override void ChannelActive(IChannelHandlerContext context) {
      _ctx = context;
      SendNumbers();
    }

    private void SendNumbers() {
      // Do not send more than 4096 numbers.
      for (int i = 0; i < 4096 && _next <= Program.Count; i++) {
        _ctx.WriteAsync(new BigInteger(_next));
        _next++;
      }

      _ctx.Flush();
    }

    protected override void ChannelRead0(IChannelHandlerContext ctx, BigInteger msg) {
      _receivedMessages++;
      if (_receivedMessages == Program.Count) {
        // Offer the answer after closing the connection.
        _ctx.CloseAsync().ContinueWith(t => _answer.Add(msg));
      }
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
      Console.Error.WriteLine("Exception: " + exception);
      _ctx.CloseAsync();
    }
  }
}
