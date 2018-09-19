using System;
using System.Threading.Tasks;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Echo.Server {
  internal static class Program {
    private const int Port = 7;

    private static async Task RunServerAsync() {
      IEventLoopGroup bossGroup = new MultithreadEventLoopGroup(1);
      IEventLoopGroup workerGroup = new MultithreadEventLoopGroup();
      EchoServerHandler serverHandler = new EchoServerHandler();
      try {
        ServerBootstrap bootstrap = new ServerBootstrap();
        bootstrap.Group(bossGroup, workerGroup)
          .Channel<TcpServerSocketChannel>()
          .Option(ChannelOption.SoBacklog, 100)
          .Handler(new LoggingHandler("SRV-LSTN"))
          .ChildHandler(new ActionChannelInitializer<IChannel>(channel => {
            channel.Pipeline.AddLast(new LoggingHandler("SRV-CONN"))
              .AddLast(serverHandler);
          }));

        IChannel boundChannel = await bootstrap.BindAsync(Port);

//        Console.ReadLine();

//        await boundChannel.CloseAsync();
        await boundChannel.CloseCompletion;
      } finally {
        await Task.WhenAll(bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
          workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
      }
    }

    public static void Main(string[] args) {
      RunServerAsync().Wait();
    }
  }
}
