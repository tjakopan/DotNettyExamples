using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Examples.Logging;

namespace Qotm.Server {
  internal static class Program {
    private const int Port = 7686;

    private static async Task RunServerAsync() {
      LoggingHelper.SetNLogLogger();

      IEventLoopGroup group = new MultithreadEventLoopGroup();
      try {
        Bootstrap b = new Bootstrap();
        b.Group(group)
          .Channel<SocketDatagramChannel>()
          .Option(ChannelOption.SoBroadcast, true)
          .Handler(new QuoteOfTheMomentServerHandler());

        IChannel channel = await b.BindAsync(Port);

        await channel.CloseCompletion;
      } finally {
        await group.ShutdownGracefullyAsync();
      }
    }

    public static void Main(string[] args) {
      RunServerAsync().Wait();
    }
  }
}
