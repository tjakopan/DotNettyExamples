using System;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Discard.Server {
  internal static class Program {
    private static async Task RunServerAsync(int port) {
      MultithreadEventLoopGroup bossGroup = new MultithreadEventLoopGroup(1);
      MultithreadEventLoopGroup workerGroup = new MultithreadEventLoopGroup();
      try {
        ServerBootstrap bootstrap = new ServerBootstrap();
        bootstrap.Group(bossGroup, workerGroup)
          .Channel<TcpServerSocketChannel>()
          .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel => {
            channel.Pipeline.AddLast(new DiscardServerHandler());
          }))
          .Option(ChannelOption.SoBacklog, 128)
          .ChildOption(ChannelOption.SoKeepalive, true);

        IChannel bootstrapChannel = await bootstrap.BindAsync(port);

        await bootstrapChannel.CloseCompletion;
      } finally {
        Task.WaitAll(bossGroup.ShutdownGracefullyAsync(), workerGroup.ShutdownGracefullyAsync());
      }
    }

    public static void Main(string[] args) {
      int port = args.Length > 0 ? int.Parse(args[0]) : 8080;
      RunServerAsync(port).Wait();
    }
  }
}
