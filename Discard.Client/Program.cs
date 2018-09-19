using System.Net;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Discard.Client {
  internal static class Program {
    private const string Host = "127.0.0.1";
    private const int Port = 8080;
    private const int Size = 256;

    private static async Task RunClientAsync() {
      MultithreadEventLoopGroup group = new MultithreadEventLoopGroup();
      try {
        Bootstrap bootstrap = new Bootstrap();
        bootstrap.Group(group)
          .Channel<TcpSocketChannel>()
          .Handler(new ActionChannelInitializer<ISocketChannel>(channel => {
            channel.Pipeline.AddLast(new DiscardClientHandler(Size));
          }));

        IChannel bootstrapChannel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(Host), Port));

        await bootstrapChannel.CloseCompletion;
      } finally {
        group.ShutdownGracefullyAsync().Wait(1000);
      }
    }

    public static void Main(string[] args) {
      RunClientAsync().Wait();
    }
  }
}
