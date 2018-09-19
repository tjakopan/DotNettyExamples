using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Time.Client {
  internal static class Program {
    private const string Host = "127.0.0.1";
    private const int Port = 37;

    private static async Task RunClientAsync() {
      IEventLoopGroup group = new MultithreadEventLoopGroup();
      try {
        Bootstrap bootstrap = new Bootstrap();
        bootstrap.Group(group)
          .Channel<TcpSocketChannel>()
          .Option(ChannelOption.TcpNodelay, true)
          .Handler(new ActionChannelInitializer<ISocketChannel>(channel => {
            channel.Pipeline.AddLast(new TimeDecoder(), new TimeClientHandler());
          }));

        IChannel clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(Host), Port));

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
