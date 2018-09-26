using System.Threading.Tasks;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Examples.Logging;

namespace WorldClock.Server {
  internal static class Program {
    private const int Port = 8463;

    private static async Task RunServerAsync() {
      LoggingHelper.SetNLogLogger();

      IEventLoopGroup bossGroup = new MultithreadEventLoopGroup(1);
      IEventLoopGroup workerGroup = new MultithreadEventLoopGroup();
      try {
        ServerBootstrap b = new ServerBootstrap();
        b.Group(bossGroup, workerGroup)
          .Channel<TcpServerSocketChannel>()
          .Handler(new LoggingHandler(LogLevel.TRACE))
          .ChildHandler(new WorldClockServerInitializer());

        IChannel channel = await b.BindAsync(Port);

        await channel.CloseCompletion;
      } finally {
        Task.WaitAll(bossGroup.ShutdownGracefullyAsync(), workerGroup.ShutdownGracefullyAsync());
      }
    }

    public static void Main(string[] args) {
      RunServerAsync().Wait();
    }
  }
}
