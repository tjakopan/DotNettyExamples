using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Examples.Logging;
using NLog;

namespace Qotm.Client {
  internal static class Program {
    private const int Port = 7686;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static async Task RunClientAsync() {
      LoggingHelper.SetNLogLogger();

      IEventLoopGroup group = new MultithreadEventLoopGroup();
      try {
        Bootstrap b = new Bootstrap();
        b.Group(group)
          .Channel<SocketDatagramChannel>()
          .Option(ChannelOption.SoBroadcast, true)
          .Handler(new QuoteOfTheMomentClientHandler());

        IChannel ch = await b.BindAsync(0);

        // Broadcast the QOTM request to port 7686.
        await ch.WriteAndFlushAsync(new DatagramPacket(Unpooled.CopiedBuffer("QOTM?", Encoding.UTF8),
          new IPEndPoint(IPAddress.Parse("255.255.255.255"), Port)));

        // QuoteOfTheMomentClientHandler will close the DatagramChannel when a response is received. If the channel is
        // not closed withing 5 seconds, print an error message and quit.
        try {
          await ch.CloseCompletion.TimeoutAfter(TimeSpan.FromMilliseconds(5000));
        } catch (TimeoutException) {
          Logger.Error("QOTM request timed out.");
        }
      } finally {
        await group.ShutdownGracefullyAsync();
      }
    }

    public static void Main(string[] args) {
      RunClientAsync().Wait();
    }
  }
}
