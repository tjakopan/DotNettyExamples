using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Factorial.Server {
  internal static class Program {
    private const int Port = 8322;

    private static async Task RunServerAsync() {
      IEventLoopGroup bossGroup = new MultithreadEventLoopGroup(1);
      IEventLoopGroup workerGroup = new MultithreadEventLoopGroup();
      try {
        ServerBootstrap b = new ServerBootstrap();
        b.Group(bossGroup, workerGroup)
          .Channel<TcpServerSocketChannel>()
          .ChildHandler(new FactorialServerInitializer());

        IChannel bootstrapChannel = await b.BindAsync(Port);

        await bootstrapChannel.CloseCompletion;
      } finally {
        Task.WaitAll(bossGroup.ShutdownGracefullyAsync(), workerGroup.ShutdownGracefullyAsync());
      }
    }

    public static void Main(string[] args) {
      RunServerAsync().Wait();
    }
  }
}
