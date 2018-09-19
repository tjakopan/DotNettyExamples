using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Echo.Client {
  internal static class Program {
    private const string Host = "127.0.0.1";
    private const int Port = 7;
    private const int Size = 256;

    private static async Task RunClientAsync() {
      IEventLoopGroup group = new MultithreadEventLoopGroup();
      try {
        Bootstrap bootstrap = new Bootstrap();
        bootstrap.Group(group)
          .Channel<TcpSocketChannel>()
          .Option(ChannelOption.TcpNodelay, true)
          .Handler(new ActionChannelInitializer<ISocketChannel>(channel => {
            channel.Pipeline.AddLast(new LoggingHandler())
              .AddLast(new EchoClientHandler(Size));
          }));

        IChannel clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(Host), Port));

//        Console.ReadLine();

//        await clientChannel.CloseAsync();
        await clientChannel.CloseCompletion;
      } finally {
        await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
      }
    }

    public static void Main(string[] args) {
      RunClientAsync().Wait();
    }
  }
}
