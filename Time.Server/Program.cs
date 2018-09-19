using System;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Time.Server {
  internal static class Program {
    private const int Port = 37;

    private static async Task RunServerAsync() {
      IEventLoopGroup bossGroup = new MultithreadEventLoopGroup(1);
      IEventLoopGroup workerGroup = new MultithreadEventLoopGroup();
      try {
        ServerBootstrap bootstrap = new ServerBootstrap()
          .Group(bossGroup, workerGroup)
          .Channel<TcpServerSocketChannel>()
          .ChildHandler(new ActionChannelInitializer<IChannel>(channel => {
            channel.Pipeline.AddLast(new TimeEncoder(), new TimeServerHandler());
          }));

        IChannel boundChannel = await bootstrap.BindAsync(Port);

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
